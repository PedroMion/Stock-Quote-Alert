using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using StockQuote.Configuration;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Helpers;
using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class MailService(
        IOptions<MailConfiguration> options,
        ILoggerService loggerService,
        ISmtpClientService smtpClientService,
        IEnvironmentService environmentService
    ) : IMailService
    {
        private readonly MailConfiguration _config = options.Value;
        private readonly ILoggerService _loggerService = loggerService;
        private readonly ISmtpClientService _smtpClientService = smtpClientService;
        private readonly IEnvironmentService _environmentService = environmentService;

        private void UseTypeAndStockInformationToSetEmailSubjectAndBody(ref MailMessage message, MessageTypeEnum messageType, decimal receivedStockQuote, AlertParametersDto parameters)
        {
            if (messageType == MessageTypeEnum.None)
            {
                _loggerService.LogError(null, LogConstants.IMPROPER_EMAIL_SENDING);

                _environmentService.TerminateProgramExecution();
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
            try
            {
                MailMessage message = GetEmailMessageFromTypeAndStockInformation(messageType, receivedStockQuote, parameters);

                await _smtpClientService.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex, LogConstants.FAILED_TO_SEND_EMAIL, _config.SenderEmail, _config.RecipientEmail);

                _environmentService.TerminateProgramExecution();
            }
        }
    }
}