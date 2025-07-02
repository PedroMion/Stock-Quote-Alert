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
    public class MailService(IOptions<MailConfiguration> options, ILoggerService loggerService) : IMailService
    {
        private readonly MailConfiguration _config = options.Value;
        private readonly ILoggerService _loggerService = loggerService;

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
                _loggerService.LogError(null, "Envio de email indevido. Por favor, entre em contato com o responsável pelo Software.");

                EnvironmentHelper.TerminateProgramExecution();
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

            try
            {
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, "Erro ao enviar email de {senderEmail} para {recipientEmail}. Por favor, verifique os dados fornecidos. Caso o problema persista, entre em contato com o responsável pelo Software.", _config.SenderEmail, _config.RecipientEmail);

                EnvironmentHelper.TerminateProgramExecution();
            }
        }
    }
}