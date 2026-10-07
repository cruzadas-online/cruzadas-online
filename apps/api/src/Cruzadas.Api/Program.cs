using Cruzadas.Api.Endpoints;
using Cruzadas.Api.Middleware;
using Cruzadas.Application;
using Cruzadas.Infrastructure;
using Cruzadas.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Application & Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5175", "http://localhost:5173", "http://127.0.0.1:5175"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("CruzadasCorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 3. Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CruzadasDbContext>(
        name: "postgres",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["db", "postgres"]);

// 4. OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

var app = builder.Build();

// 5. Middleware Pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("CruzadasCorsPolicy");

// 6. OpenAPI & Scalar UI
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Cruzadas.online API";
    options.Theme = ScalarTheme.Mars;
});

// 7. Health Check
app.MapGet("/health", async (CruzadasDbContext dbContext, CancellationToken ct) =>
{
    try
    {
        var canConnect = await dbContext.Database.CanConnectAsync(ct);
        return canConnect
            ? Results.Ok(new { status = "Healthy", database = "Connected", timestamp = DateTimeOffset.UtcNow })
            : Results.Json(new { status = "Degraded", database = "Unreachable", timestamp = DateTimeOffset.UtcNow }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
    catch (Exception ex)
    {
        return Results.Json(new { status = "Unhealthy", error = ex.Message, timestamp = DateTimeOffset.UtcNow }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }
})
.WithName("HealthCheck")
.WithTags("Health");

// 8. Domain Endpoints
app.MapGamesEndpoints();
app.MapQuizEndpoints();

// 9. Database Auto-Migration & Seed (Development / Container startup)
var autoMigrate = builder.Configuration.GetValue<bool>("Database:AutoMigrate", false);
var autoSeed = builder.Configuration.GetValue<bool>("Database:AutoSeed", false);

if (autoMigrate || autoSeed)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<CruzadasDbContext>();

    try
    {
        if (autoMigrate)
        {
            logger.LogInformation("Aplicando migrations pendentes do EF Core...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migrations aplicadas com sucesso.");
        }

        if (autoSeed)
        {
            logger.LogInformation("Executando seed de dados do Cruzadas.online...");
            await CruzadasDataSeeder.SeedAsync(dbContext, logger);
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Aviso durante migração/seed do banco. Se o PostgreSQL ainda estiver subindo, o serviço tentará novamente.");
    }
}

app.Run();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
