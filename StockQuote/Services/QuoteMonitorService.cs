using Microsoft.Extensions.Logging;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class QuoteMonitorService(ILoggerService loggerService, IMailService mailService, IStockApiService stockApiService) : IQuoteMonitorService
    {
        private readonly ILoggerService _loggerService = loggerService;
        private readonly IMailService _mailService = mailService;
        private readonly IStockApiService _stockApiService = stockApiService;

        private MessageTypeEnum VerifyStockQuote(AlertParametersDto parameters, StockInformationDto stockInformation)
        {
            if (stockInformation.StockPrice >= parameters.SellPrice)
            {
                _loggerService.LogInformation("Valor do ativo {stockCode} ultrapassa o valor definido para venda. Valor de venda: R${valor}.", parameters.StockCode, parameters.SellPrice.ToString("N2"));

                return MessageTypeEnum.Sale;
            }
            else if (stockInformation.StockPrice <= parameters.BuyPrice)
            {
                _loggerService.LogInformation("Valor do ativo {stockCode} abaixo do valor definido para compra. Valor de compra: R${valor}.", parameters.StockCode, parameters.BuyPrice.ToString("N2"));

                return MessageTypeEnum.Purchase;
            }

            return MessageTypeEnum.None;
        }

        public async Task CheckStockQuoteAndSendEmailAsync(AlertParametersDto parameters)
        {
            _loggerService.LogInformation("Recuperando informação de cotação para ativo {stockCode}", parameters.StockCode);

            StockInformationDto? stockInfo = await _stockApiService.GetStockInformationFromStockCodeAsync(parameters.StockCode);

            if (stockInfo != null)
            {
                _loggerService.LogInformation("Informação recuperada com sucesso para ativo {stockCode}. Valor: R${stockPrice}.", parameters.StockCode, stockInfo.StockPrice.ToString("N2"));

                var typeOfEmail = VerifyStockQuote(parameters, stockInfo);

                if (typeOfEmail != MessageTypeEnum.None)
                {
                    _loggerService.LogInformation("Enviando e-mail de alerta para ativo {stockCode}", parameters.StockCode);

                    await _mailService.SendEmailToRecipientFromTypeAndStockInformationAsync(typeOfEmail, stockInfo.StockPrice, parameters);

                    _loggerService.LogInformation("Email enviado com sucesso!");

                    return;
                }

                _loggerService.LogWarning("Erro inesperado ao obter cotação do ativo. Por favor, verifique os parâmetros fornecidos para consulta da API, como chave e código do ativo. Caso o problema persista, entre em contato com o responsável pelo software.");
            }
        }
    }
}