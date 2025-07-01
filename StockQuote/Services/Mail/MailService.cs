using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using StockQuote.Configuration;
using StockQuote.Enums;
using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class MailService(IOptions<MailConfiguration> options) : IMailService
    {
        private readonly MailConfiguration _config = options.Value;

        private SmtpClient GetSmtpClientFromConfiguration()
        {
            return new SmtpClient(_config.SmtpServer, _config.Port)
            {
                Credentials = new NetworkCredential(_config.SenderEmail, _config.SenderPassword),
                EnableSsl = true
            };
        }

        private void UseTypeToSetEmailSubjectAndBody(ref MailMessage message, MessageTypeEnum messageType)
        {
            switch (messageType)
            {
                case MessageTypeEnum.Sale:
                    message.Subject = "Mensagem venda teste";
                    message.Body = "Mensagem venda teste";
                    break;

                case MessageTypeEnum.Purchase:
                    message.Subject = "Mensagem compra teste";
                    message.Body = "Mensagem compra teste";
                    break;

                default:
                    message.Subject = "Erro";
                    message.Body = "Algo deu errado";
                    break;
            }
        }

        private MailMessage GetEmailMessageFromType(MessageTypeEnum messageType)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_config.SenderEmail),
                Subject = "",
                Body = "",
                IsBodyHtml = false
            };

            message.To.Add(_config.RecipientEmail);

            UseTypeToSetEmailSubjectAndBody(ref message, messageType);

            return message;
        }

        public async Task SendEmailToRecipientFromTypeAsync(MessageTypeEnum messageType)
        {
            SmtpClient smtpClient = GetSmtpClientFromConfiguration();

            MailMessage message = GetEmailMessageFromType(messageType);

            await smtpClient.SendMailAsync(message);
        }
    }
}