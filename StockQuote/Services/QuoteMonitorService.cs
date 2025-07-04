using Microsoft.Extensions.Logging;
using StockQuote.Constants;
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
                _loggerService.LogInformation(LogConstants.ValueAboveThreshold, parameters.StockCode, parameters.SellPrice.ToString("N2"));

                return MessageTypeEnum.Sale;
            }
            else if (stockInformation.StockPrice <= parameters.BuyPrice)
            {
                _loggerService.LogInformation(LogConstants.ValueBelowThreshold, parameters.StockCode, parameters.BuyPrice.ToString("N2"));

                return MessageTypeEnum.Purchase;
            }

            return MessageTypeEnum.None;
        }

        public async Task CheckStockQuoteAndSendEmailAsync(AlertParametersDto parameters)
        {
            _loggerService.LogInformation(LogConstants.RetrievingAssetInformation, parameters.StockCode);

            StockInformationDto? stockInfo = await _stockApiService.GetStockInformationFromStockCodeAsync(parameters.StockCode);

            if (stockInfo != null)
            {
                _loggerService.LogInformation(LogConstants.AssetInformationRetrieved, parameters.StockCode, stockInfo.StockPrice.ToString("N2"));

                var typeOfEmail = VerifyStockQuote(parameters, stockInfo);

                if (typeOfEmail != MessageTypeEnum.None)
                {
                    _loggerService.LogInformation(LogConstants.SendingEmail, parameters.StockCode);

                    await _mailService.SendEmailToRecipientFromTypeAndStockInformationAsync(typeOfEmail, stockInfo.StockPrice, parameters);

                    _loggerService.LogInformation(LogConstants.EmailSent);
                }

                return;
            }

            _loggerService.LogWarning(LogConstants.QuoteRequestFailed);
        }
    }
}