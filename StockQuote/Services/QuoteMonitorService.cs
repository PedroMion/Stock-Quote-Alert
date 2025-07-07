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
                _loggerService.LogInformation(LogConstants.VALUE_ABOVE_THRESHOLD, parameters.StockCode, parameters.SellPrice.ToString("N2"));

                return MessageTypeEnum.Sale;
            }
            else if (stockInformation.StockPrice <= parameters.BuyPrice)
            {
                _loggerService.LogInformation(LogConstants.VALUE_BELOW_THRESHOLD, parameters.StockCode, parameters.BuyPrice.ToString("N2"));

                return MessageTypeEnum.Purchase;
            }

            return MessageTypeEnum.None;
        }

        public async Task CheckStockQuoteAndSendEmailAsync(AlertParametersDto parameters)
        {
            _loggerService.LogInformation(LogConstants.RETRIEVING_ASSET_INFORMATION, parameters.StockCode);

            StockInformationDto? stockInfo = await _stockApiService.GetStockInformationFromStockCodeAsync(parameters.StockCode);

            if (stockInfo != null)
            {
                _loggerService.LogInformation(LogConstants.ASSET_INFORMATION_RETRIEVED, parameters.StockCode, stockInfo.StockPrice.ToString("N2"));

                var typeOfEmail = VerifyStockQuote(parameters, stockInfo);

                if (typeOfEmail != MessageTypeEnum.None)
                {
                    _loggerService.LogInformation(LogConstants.SENDING_EMAIL, parameters.StockCode);

                    await _mailService.SendEmailToRecipientFromTypeAndStockInformationAsync(typeOfEmail, stockInfo.StockPrice, parameters);

                    _loggerService.LogInformation(LogConstants.EMAIL_SENT);
                }

                return;
            }

            _loggerService.LogWarning(LogConstants.QUOTE_REQUEST_FAILED);
        }
    }
}