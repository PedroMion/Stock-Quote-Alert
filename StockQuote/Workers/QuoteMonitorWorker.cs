using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StockQuote.Configuration;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Services.Interfaces;

public class QuoteMonitorWorker(
    IOptions<GeneralConfiguration> options,
    ILoggerService loggerService,
    IQuoteMonitorService monitorService,
    List<AlertParametersDto> alertParameters,
    IEnvironmentService environmentService
) : BackgroundService
{
    private readonly GeneralConfiguration _config = options.Value;
    private readonly ILoggerService _loggerService = loggerService;
    private readonly IQuoteMonitorService _monitorService = monitorService;
    private readonly List<AlertParametersDto> _alertParameters = alertParameters;
    private readonly IEnvironmentService _environmentService = environmentService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var delayBetweenChecks = TimeSpan.FromSeconds(_config.DelayBetweenChecksInSeconds);
        int maxParallelChecks = _config.MaxParallelChecks;

        using var semaphore = new SemaphoreSlim(maxParallelChecks);

        _loggerService.LogInformation(LogConstants.STARTED_MONITORING_ASSETS, _alertParameters);

        while (!stoppingToken.IsCancellationRequested)
        {
            var tasks = _alertParameters.Select(async asset =>
            {
                await semaphore.WaitAsync(stoppingToken);
                try
                {
                    await _monitorService.CheckStockQuoteAndSendEmailAsync(asset);
                    _loggerService.LogInformation(LogConstants.ASSET_MONITORING_COMPLETED, asset.StockCode);
                }
                catch (Exception ex)
                {
                    _loggerService.LogError(ex, LogConstants.ASSET_MONITORING_ERROR, asset.StockCode);
                    _environmentService.TerminateProgramExecution();
                }
                finally
                {
                    semaphore.Release();
                }
            });

            await Task.WhenAll(tasks);

            _loggerService.LogInformation(LogConstants.WAITING_FOR_NEXT_ROUND, _config.DelayBetweenChecksInSeconds);

            await Task.Delay(delayBetweenChecks, stoppingToken);
        }
    }
}