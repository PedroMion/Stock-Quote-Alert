using System.Net.Mail;
using Microsoft.Extensions.Options;
using Moq;
using StockQuote.Configuration;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Services;
using StockQuote.Services.Interfaces;
using StockQuote.Tests.MockData;

namespace StockQuote.Tests.Services
{
    public class MailServiceTest
    {
        private readonly Mock<IOptions<MailConfiguration>> _optionsMock = new();
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<ISmtpClientService> _smtpMock = new();
        private readonly Mock<IEnvironmentService> _envMock = new();
        private readonly AlertParametersDto _alertParameters = AlertParametersDtoMockData.alertParameters;
        private readonly MailConfiguration _mailConfiguration = MailConfigurationMockData.mailConfiguration;
        private readonly MailConfiguration _mailConfigurationWithoutEmailInformation = MailConfigurationMockData.mailConfigurationWithouEmailInformation;

        private void SetupMailConfigurationAndEnvironment(MailConfiguration configurationObject)
        {
            _optionsMock.Setup(options => options.Value).Returns(configurationObject);

            _envMock.Setup(env => env.TerminateProgramExecution())
                    .Throws(new OperationCanceledException());
        }

        private void SetupSmtpThrowException()
        {
            _smtpMock.Setup(smtp =>
                smtp.SendMailAsync(It.IsAny<MailMessage>())
            ).ThrowsAsync(new Exception());
        }

        private async Task CallSendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum messageType, decimal stockQuote)
        {
            var service = new MailService(_optionsMock.Object, _loggerMock.Object, _smtpMock.Object, _envMock.Object);

            await service.SendEmailToRecipientFromTypeAndStockInformationAsync(messageType, stockQuote, _alertParameters);
        }

        private async Task SendEmailTestWithErrorAndAssertExceptionAsync(MessageTypeEnum messageType, decimal stockQuote)
        {
            var service = new MailService(_optionsMock.Object, _loggerMock.Object, _smtpMock.Object, _envMock.Object);

            try
            {
                await service.SendEmailToRecipientFromTypeAndStockInformationAsync(messageType, stockQuote, _alertParameters);
            }
            catch (Exception ex)
            {
                Assert.IsType<OperationCanceledException>(ex);
            }
        }

        private void AssertLogErrorImproperSendingCalls(Times times)
        {
            _loggerMock.Verify(l => l.LogError(null, LogConstants.ImproperEmailSending), times);
        }

        private void AssertLogErrorFailedToSend(Times times)
        {
            _loggerMock.Verify(l => l.LogError(It.IsAny<Exception>(), LogConstants.FailedToSendEmail, It.IsAny<string>(), It.IsAny<string>()), times);
        }

        private void AssertSmtpServiceCalls(Times times)
        {
            _smtpMock.Verify(s =>
                s.SendMailAsync(
                    It.IsAny<MailMessage>()
                ),
                times
            );
        }

        private void AssertEnvironmentServiceCalls(Times times)
        {
            _envMock.Verify(e =>
                e.TerminateProgramExecution(),
                times
            );
        }

        [Fact]
        public async Task MessageTypeSaleTest_ShouldTriggerEmail()
        {
            SetupMailConfigurationAndEnvironment(_mailConfiguration);

            await CallSendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum.Sale, Convert.ToDecimal(23.10));

            AssertLogErrorFailedToSend(Times.Never());
            AssertSmtpServiceCalls(Times.Once());
            AssertEnvironmentServiceCalls(Times.Never());
        }

        [Fact]
        public async Task MessageTypePurchaseTest_ShouldTriggerEmail()
        {
            SetupMailConfigurationAndEnvironment(_mailConfiguration);

            await CallSendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum.Purchase, Convert.ToDecimal(23.10));

            AssertLogErrorFailedToSend(Times.Never());
            AssertSmtpServiceCalls(Times.Once());
            AssertEnvironmentServiceCalls(Times.Never());
        }

        [Fact]
        public async Task MessageTypeNoneTest_ShouldLogError()
        {
            SetupMailConfigurationAndEnvironment(_mailConfiguration);

            await SendEmailTestWithErrorAndAssertExceptionAsync(MessageTypeEnum.None, Convert.ToDecimal(23.10));

            AssertLogErrorImproperSendingCalls(Times.Once());
            AssertSmtpServiceCalls(Times.Never());
            AssertEnvironmentServiceCalls(Times.AtLeastOnce());
        }

        [Fact]
        public async Task InvalidEmailInformationTest_ShouldTriggerError()
        {
            SetupMailConfigurationAndEnvironment(_mailConfigurationWithoutEmailInformation);

            await SendEmailTestWithErrorAndAssertExceptionAsync(MessageTypeEnum.Purchase, Convert.ToDecimal(23.10));

            AssertLogErrorFailedToSend(Times.Once());
            AssertSmtpServiceCalls(Times.Never());
            AssertEnvironmentServiceCalls(Times.Once());
        }

        [Fact]
        public async Task WhenSmtpFailsTest_ShouldTriggerError()
        {
            SetupMailConfigurationAndEnvironment(_mailConfiguration);

            SetupSmtpThrowException();

            await SendEmailTestWithErrorAndAssertExceptionAsync(MessageTypeEnum.Purchase, Convert.ToDecimal(23.10));

            AssertLogErrorFailedToSend(Times.Once());
            AssertSmtpServiceCalls(Times.Once());
            AssertEnvironmentServiceCalls(Times.Once());
        }
    }
}