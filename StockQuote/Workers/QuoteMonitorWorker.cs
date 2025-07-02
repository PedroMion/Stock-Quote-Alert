using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
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
                _loggerService.LogError(ex, "Erro ao monitorar ativo: {Asset}. Por favor, entre em contato com o responsável pelo software.", _alertParameters.StockCode);

                EnvironmentHelper.TerminateProgramExecution();
            }

            _loggerService.LogInformation("Verificação completa");
            await Task.Delay(delayBetweenChecks, stoppingToken);
        }
    }
}