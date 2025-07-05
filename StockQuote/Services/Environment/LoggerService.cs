using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockQuote.Configuration;
using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class LoggerService(IOptions<GeneralConfiguration> options, ILogger<LoggerService> logger) : ILoggerService
    {
        private readonly GeneralConfiguration _config = options.Value;
        private readonly ILogger<LoggerService> _logger = logger;

        public void LogInformation(string? message, params object?[] args)
        {
            if (_config.LogInformation)
            {
                _logger.LogInformation(message, args);
            }
        }

        public void LogError(Exception? ex, string? message, params object?[] args)
        {
            
            _logger.LogError(ex, message, args);
        }

        public void LogWarning(string? message, params object?[] args)
        {
            _logger.LogWarning(message, args);
        }
    }
}