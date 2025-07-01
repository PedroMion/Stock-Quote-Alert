using StockQuote.Enums;

namespace StockQuote.Services.Interfaces
{
    public interface IMailService
    {
        public Task SendEmailToRecipientFromTypeAsync(MessageTypeEnum messageType);
    }
}