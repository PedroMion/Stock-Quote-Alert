namespace StockQuote.Services.Interfaces
{
    public interface ILoggerService
    {
        public void LogInformation(string? message, params object?[] args);

        public void LogError(Exception? ex, string? message, params object?[] args);

        public void LogWarning(string? message, params object?[] args);
    }
}