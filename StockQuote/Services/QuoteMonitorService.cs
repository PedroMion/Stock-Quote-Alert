using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class QuoteMonitorService(IMailService mailService, IStockApiService stockApiService) : IQuoteMonitorService
    {
        private readonly IMailService _mailService = mailService;
        private readonly IStockApiService _stockApiService = stockApiService;

        private MessageTypeEnum VerifyStockQuote(AlertParametersDto parameters, StockInformationDto stockInformation)
        {
            if (stockInformation.StockPrice >= parameters.SellPrice)
            {
                return MessageTypeEnum.Sale;
            }
            else if (stockInformation.StockPrice <= parameters.BuyPrice)
            {
                return MessageTypeEnum.Purchase;
            }

            return MessageTypeEnum.None;
        }

        public async Task CheckStockQuoteAndSendEmailAsync(AlertParametersDto parameters)
        {
            StockInformationDto? stockInfo = await _stockApiService.GetStockInformationFromStockCodeAsync(parameters.StockCode);

            if (stockInfo != null)
            {
                var typeOfEmail = VerifyStockQuote(parameters, stockInfo);

                if (typeOfEmail != MessageTypeEnum.None)
                {
                    await _mailService.SendEmailToRecipientFromTypeAndStockInformationAsync(typeOfEmail, stockInfo.StockPrice, parameters);
                }
            }
        }
    }
}