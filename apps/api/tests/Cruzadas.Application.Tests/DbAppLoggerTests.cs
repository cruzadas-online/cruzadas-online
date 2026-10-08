using Cruzadas.Application.Interfaces;
using Cruzadas.Domain.Entities;
using Cruzadas.Infrastructure.Logging;
using Cruzadas.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Cruzadas.Application.Tests;

public class DbAppLoggerTests
{
    [Fact]
    public async Task DbAppLogger_And_Processor_Should_Persist_Logs_To_Database()
    {
        // Arrange
        var dbOptions = new DbContextOptionsBuilder<CruzadasDbContext>()
            .UseInMemoryDatabase(databaseName: $"CruzadasLogTest_{Guid.NewGuid():N}")
            .Options;

        var services = new ServiceCollection();
        services.AddScoped(_ => new CruzadasDbContext(dbOptions));
        var serviceProvider = services.BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();

        var logger = new DbAppLogger(NullLogger<DbAppLogger>.Instance);
        var hostedService = new DbLogProcessorHostedService(logger, scopeFactory, NullLogger<DbLogProcessorHostedService>.Instance);

        using var cts = new CancellationTokenSource();

        // Start hosted service in background
        _ = hostedService.StartAsync(cts.Token);

        // Act: log structured events
        logger.LogInformation(
            category: "TestCategory",
            message: "Test info log message",
            eventName: "QuizAttemptStarted",
            properties: new { QuizSlug = "fundamentos-da-fe", TotalQuestions = 10 });

        logger.LogWarning(
            category: "TestCategory",
            message: "Test warning log message",
            eventName: "AttemptAlreadyCompleted",
            properties: new { AttemptId = Guid.NewGuid() });

        logger.LogError(
            category: "TestCategory",
            message: "Test error log message",
            exception: new InvalidOperationException("Erro simulado para teste"),
            eventName: "UnhandledException",
            properties: new { StatusCode = 500 });

        // Stop hosted service - this cancels loop and flushes remaining items
        await hostedService.StopAsync(CancellationToken.None);

        // Assert
        using var assertContext = new CruzadasDbContext(dbOptions);
        var logs = await assertContext.AppLogs.OrderBy(l => l.Id).ToListAsync();

        var events = string.Join(", ", logs.Select(l => l.EventName));
        Assert.True(logs.Count == 3, $"Esperado 3 logs, mas obteve {logs.Count}. Eventos encontrados: {events}");

        // Verify Info log
        Assert.Equal("Information", logs[0].Level);
        Assert.Equal("TestCategory", logs[0].Category);
        Assert.Equal("Test info log message", logs[0].Message);
        Assert.Equal("QuizAttemptStarted", logs[0].EventName);
        Assert.Contains("fundamentos-da-fe", logs[0].PropertiesJson);

        // Verify Warning log
        Assert.Equal("Warning", logs[1].Level);
        Assert.Equal("AttemptAlreadyCompleted", logs[1].EventName);

        // Verify Error log
        Assert.Equal("Error", logs[2].Level);
        Assert.Equal("UnhandledException", logs[2].EventName);
        Assert.NotNull(logs[2].Exception);
        Assert.Contains("Erro simulado para teste", logs[2].Exception);
    }
}
