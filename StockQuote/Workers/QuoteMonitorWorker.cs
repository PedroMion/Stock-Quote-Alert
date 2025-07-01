using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockQuote.Data.Dto;
using StockQuote.Services.Interfaces;

public class QuoteMonitorWorker(
    ILogger<QuoteMonitorWorker> logger,
    IQuoteMonitorService monitorService,
    AlertParametersDto alertParameters
) : BackgroundService
{
    private readonly ILogger<QuoteMonitorWorker> _logger = logger;
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
                _logger.LogError(ex, "Erro ao monitorar ativo: {Asset}", _alertParameters.StockCode);
            }

            _logger.LogInformation("Verificação completa");
            await Task.Delay(delayBetweenChecks, stoppingToken);
        }
    }
}