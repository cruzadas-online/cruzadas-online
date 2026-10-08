namespace Cruzadas.Application.Interfaces;

public interface IAppLogger
{
    void LogInformation(string category, string message, string? eventName = null, object? properties = null);
    void LogWarning(string category, string message, string? eventName = null, object? properties = null, Exception? exception = null);
    void LogError(string category, string message, Exception? exception = null, string? eventName = null, object? properties = null);
}
