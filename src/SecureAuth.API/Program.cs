using SecureAuth.API.Configurations;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Configurações e serviços
builder.Configuration.AddEnvironmentVariables();
builder.AddLoggingConfig();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddDependencies();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRateLimiting();
builder.Services.AddControllers();
builder.Services.AddJwtOptions(builder.Configuration);
builder.Services.AddSwaggerSetup();

var app = builder.Build();

// 🔹 Middlewares e Swagger
app.UseGlobalMiddlewares();
app.UseSwaggerSetup();

// 🔹 Rotas
app.MapControllers();

// 🔹 Migrações automáticas
if (!app.Environment.IsEnvironment("Testing"))
{
    app.ApplyMigrationsSafe();
}

app.Run();

public partial class Program { }