using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Helpers;
using StockQuote.Services.Interfaces;

public class QuoteMonitorWorker(
    ILoggerService loggerService,
    IQuoteMonitorService monitorService,
    AlertParametersDto alertParameters
) : BackgroundService
{
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IQuoteMonitorService _monitorService = monitorService;
    private readonly AlertParametersDto _alertParameters = alertParameters;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delayBetweenChecks = TimeSpan.FromSeconds(20);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _monitorService.CheckStockQuoteAndSendEmailAsync(_alertParameters);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, LogConstants.AssetMonitoringError, _alertParameters.StockCode);

                EnvironmentHelper.TerminateProgramExecution();
            }

            _loggerService.LogInformation(LogConstants.AssetMonitoringCompleted, _alertParameters.StockCode);
            await Task.Delay(delayBetweenChecks, stoppingToken);
        }
    }
}