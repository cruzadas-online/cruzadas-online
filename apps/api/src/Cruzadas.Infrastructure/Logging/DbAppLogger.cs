using System.Diagnostics;
using System.Text.Json;
using System.Threading.Channels;
using Cruzadas.Application.Interfaces;
using Cruzadas.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Cruzadas.Infrastructure.Logging;

public class DbAppLogger : IAppLogger
{
    private readonly Channel<AppLog> _channel;
    private readonly ILogger<DbAppLogger> _logger;

    public ChannelReader<AppLog> Reader => _channel.Reader;

    public DbAppLogger(ILogger<DbAppLogger> logger)
    {
        _logger = logger;
        _channel = Channel.CreateBounded<AppLog>(new BoundedChannelOptions(10000)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true
        });
    }

    public void LogInformation(string category, string message, string? eventName = null, object? properties = null)
    {
        Enqueue("Information", category, message, eventName, properties, null);
        _logger.LogInformation("[{EventName}] {Message}", eventName ?? category, message);
    }

    public void LogWarning(string category, string message, string? eventName = null, object? properties = null, Exception? exception = null)
    {
        Enqueue("Warning", category, message, eventName, properties, exception);
        _logger.LogWarning(exception, "[{EventName}] {Message}", eventName ?? category, message);
    }

    public void LogError(string category, string message, Exception? exception = null, string? eventName = null, object? properties = null)
    {
        Enqueue("Error", category, message, eventName, properties, exception);
        _logger.LogError(exception, "[{EventName}] {Message}", eventName ?? category, message);
    }

    private void Enqueue(
        string level,
        string category,
        string message,
        string? eventName,
        object? properties,
        Exception? exception)
    {
        string? propertiesJson = null;
        if (properties != null)
        {
            try
            {
                propertiesJson = JsonSerializer.Serialize(properties, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
            }
            catch
            {
                propertiesJson = properties.ToString();
            }
        }

        string? traceId = Activity.Current?.TraceId.ToString() ?? Activity.Current?.Id;

        var log = new AppLog(
            level: level,
            category: category,
            message: message,
            eventName: eventName,
            propertiesJson: propertiesJson,
            exception: exception?.ToString(),
            traceId: traceId,
            timestamp: DateTimeOffset.UtcNow);

        _channel.Writer.TryWrite(log);
    }
}
