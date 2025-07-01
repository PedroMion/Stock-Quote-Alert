using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StockQuote.Configuration;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Helpers;
using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class MailService(IOptions<MailConfiguration> options, ILogger<IMailService> logger) : IMailService
    {
        private readonly MailConfiguration _config = options.Value;
        private readonly ILogger<IMailService> _logger = logger;

        private SmtpClient GetSmtpClientFromConfiguration()
        {
            return new SmtpClient(_config.SmtpServer, _config.Port)
            {
                Credentials = new NetworkCredential(_config.SenderEmail, _config.SenderPassword),
                EnableSsl = true
            };
        }

        private void UseTypeAndStockInformationToSetEmailSubjectAndBody(ref MailMessage message, MessageTypeEnum messageType, decimal receivedStockQuote, AlertParametersDto parameters)
        {
            if (messageType == MessageTypeEnum.None)
            {
                _logger.LogError("Envio de email indevido.");
            }

            message.Subject = EmailHelper.GetEmailSubjectFromTypeAndStockInformation(messageType, parameters);
            message.Body = EmailHelper.GetEmailBodyFromTypeAndStockInformation(messageType, receivedStockQuote, parameters);
        }

        private MailMessage GetEmailMessageFromTypeAndStockInformation(MessageTypeEnum messageType, decimal receivedStockQuote, AlertParametersDto parameters)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_config.SenderEmail),
                Subject = "",
                Body = "",
                IsBodyHtml = true,
                BodyEncoding = Encoding.UTF8,
                SubjectEncoding = Encoding.UTF8
            };

            message.To.Add(_config.RecipientEmail);

            UseTypeAndStockInformationToSetEmailSubjectAndBody(ref message, messageType, receivedStockQuote, parameters);

            return message;
        }

        public async Task SendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum messageType, decimal receivedStockQuote, AlertParametersDto parameters)
        {
            SmtpClient smtpClient = GetSmtpClientFromConfiguration();

            MailMessage message = GetEmailMessageFromTypeAndStockInformation(messageType, receivedStockQuote, parameters);

            await smtpClient.SendMailAsync(message);
        }
    }
}