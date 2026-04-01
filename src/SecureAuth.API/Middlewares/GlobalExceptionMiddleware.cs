using SecureAuth.API.Middlewares;
using System.Net;
using System.Text.Json;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 🔗 1. Captura ou cria CorrelationId
        var correlationId = GetOrCreateCorrelationId(context);

        // 🔗 2. Adiciona no TraceIdentifier (padrão do ASP.NET)
        context.TraceIdentifier = correlationId;

        // 🔗 3. Retorna no header da resposta
        context.Response.Headers[CorrelationConstants.HeaderName] = correlationId;

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro não tratado. Path: {Path} | CorrelationId: {CorrelationId}",
                context.Request.Path,
                correlationId);

            await HandleExceptionAsync(context, ex, correlationId);
        }
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(CorrelationConstants.HeaderName, out var correlationId))
        {
            return correlationId!;
        }

        return Guid.NewGuid().ToString();
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex, string correlationId)
    {
        var statusCode = ex switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            KeyNotFoundException => HttpStatusCode.NotFound,
            _ => HttpStatusCode.InternalServerError
        };

        var response = new
        {
            type = $"https://httpstatuses.com/{(int)statusCode}",
            title = statusCode.ToString(),
            status = (int)statusCode,
            detail = GetMessage(ex, statusCode),
            traceId = correlationId // 🔥 usa o mesmo ID
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private static string GetMessage(Exception ex, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.BadRequest ||
            statusCode == HttpStatusCode.NotFound ||
            statusCode == HttpStatusCode.Unauthorized)
        {
            return ex.Message;
        }

        return "Erro interno do servidor";
    }
}