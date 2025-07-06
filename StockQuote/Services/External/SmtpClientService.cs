using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using StockQuote.Configuration;
using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class SmtpClientService(IOptions<MailConfiguration> options): ISmtpClientService
    {
        private readonly MailConfiguration _config = options.Value;

        public SmtpClient GetSmtpClientFromConfiguration()
        {
            return new SmtpClient(_config.SmtpServer, _config.SmtpPort)
            {
                Credentials = new NetworkCredential(_config.SenderEmail, _config.SenderPassword),
                EnableSsl = true
            };
        }

        public async Task SendMailAsync(MailMessage message)
        {
            SmtpClient client = GetSmtpClientFromConfiguration();

            await client.SendMailAsync(message);
        }
    }
}
