using System.Net.Mail;

namespace StockQuote.Services.Interfaces
{
    public interface ISmtpClientService
    {
        public Task SendMailAsync(MailMessage message);
    }
}