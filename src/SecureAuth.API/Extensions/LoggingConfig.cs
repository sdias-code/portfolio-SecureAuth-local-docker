using Serilog;

namespace SecureAuth.API.Configurations
{
    public static class LoggingConfig
    {
        public static void AddLoggingConfig(this WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "SecureAuth.API")
                .WriteTo.Console(new Serilog.Formatting.Json.JsonFormatter())
                .CreateLogger();

            builder.Host.UseSerilog();
           
        }
    }
}