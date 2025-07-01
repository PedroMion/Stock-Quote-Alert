using StockQuote.Data.Dto;
using StockQuote.Enums;

namespace StockQuote.Services.Interfaces
{
    public interface IMailService
    {
        public Task SendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum messageType, decimal receivedStockQuote, AlertParametersDto parameters);
    }
}