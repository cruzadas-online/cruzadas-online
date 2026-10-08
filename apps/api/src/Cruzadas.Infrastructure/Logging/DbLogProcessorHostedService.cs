using Cruzadas.Domain.Entities;
using Cruzadas.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cruzadas.Infrastructure.Logging;

public class DbLogProcessorHostedService : BackgroundService
{
    private readonly DbAppLogger _dbAppLogger;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<DbLogProcessorHostedService> _logger;

    public DbLogProcessorHostedService(
        DbAppLogger dbAppLogger,
        IServiceScopeFactory scopeFactory,
        ILogger<DbLogProcessorHostedService> logger)
    {
        _dbAppLogger = dbAppLogger;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando processador de escrita de logs no banco de dados.");

        var batch = new List<AppLog>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Wait for an item or periodic timeout to flush
                if (await _dbAppLogger.Reader.WaitToReadAsync(stoppingToken))
                {
                    while (_dbAppLogger.Reader.TryRead(out var logItem))
                    {
                        batch.Add(logItem);
                        if (batch.Count >= 50)
                        {
                            break;
                        }
                    }

                    if (batch.Count > 0)
                    {
                        using var flushCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                        await FlushBatchAsync(batch, flushCts.Token);
                        batch.Clear();
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no loop de processamento de logs para o banco de dados.");
                await Task.Delay(1000, CancellationToken.None);
            }
        }

        // Flush any remaining logs before shutdown
        while (_dbAppLogger.Reader.TryRead(out var remaining))
        {
            batch.Add(remaining);
        }

        if (batch.Count > 0)
        {
            using var shutdownCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await FlushBatchAsync(batch, shutdownCts.Token);
            batch.Clear();
        }
    }

    private async Task FlushBatchAsync(List<AppLog> logs, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CruzadasDbContext>();

            context.AppLogs.AddRange(logs);
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao gravar lote de {Count} logs na tabela app_logs.", logs.Count);
        }
    }
}
