namespace Cruzadas.Domain.Entities;

public class AppLog
{
    public long Id { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }
    public string Level { get; private set; } = null!;
    public string Category { get; private set; } = null!;
    public string Message { get; private set; } = null!;
    public string? EventName { get; private set; }
    public string? PropertiesJson { get; private set; }
    public string? Exception { get; private set; }
    public string? TraceId { get; private set; }

    protected AppLog() { }

    public AppLog(
        string level,
        string category,
        string message,
        string? eventName = null,
        string? propertiesJson = null,
        string? exception = null,
        string? traceId = null,
        DateTimeOffset? timestamp = null)
    {
        Level = level;
        Category = category;
        Message = message;
        EventName = eventName;
        PropertiesJson = propertiesJson;
        Exception = exception;
        TraceId = traceId;
        Timestamp = timestamp ?? DateTimeOffset.UtcNow;
    }
}
