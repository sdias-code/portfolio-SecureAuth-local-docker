namespace SecureAuth.Application.Common.Behaviors
{
    using MediatR;
    using Microsoft.Extensions.Logging;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Http;

    public class LoggingPerformanceBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogger<LoggingPerformanceBehavior<TRequest, TResponse>> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingPerformanceBehavior(
            ILogger<LoggingPerformanceBehavior<TRequest, TResponse>> logger, 
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;

            // 🔗 Correlation ID (simples)
            var correlationId = _httpContextAccessor.HttpContext?.TraceIdentifier;

            _logger.LogInformation(
                "➡️ Handling {RequestName} | CorrelationId: {CorrelationId} | Request: {@Request}",
                requestName,
                correlationId,
                request);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await next();

                stopwatch.Stop();

                _logger.LogInformation(
                    "✅ Handled {RequestName} | CorrelationId: {CorrelationId} | Duration: {ElapsedMilliseconds}ms",
                    requestName,
                    correlationId,
                    stopwatch.ElapsedMilliseconds);

                // ⚠️ Log de performance lenta
                if (stopwatch.ElapsedMilliseconds > 500)
                {
                    _logger.LogWarning(
                        "⚠️ Slow Request: {RequestName} | {ElapsedMilliseconds}ms | CorrelationId: {CorrelationId}",
                        requestName,
                        stopwatch.ElapsedMilliseconds,
                        correlationId);
                }

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "❌ Error in {RequestName} | CorrelationId: {CorrelationId} | Duration: {ElapsedMilliseconds}ms",
                    requestName,
                    correlationId,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}