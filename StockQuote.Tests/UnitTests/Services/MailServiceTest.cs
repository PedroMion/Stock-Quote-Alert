using System.ComponentModel;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Moq;
using StockQuote.Configuration;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Services;
using StockQuote.Services.Interfaces;
using StockQuote.Tests.Helpers;

namespace StockQuote.Tests.Services
{
    public class MailServiceTest
    {
        private readonly Mock<IOptions<MailConfiguration>> _optionsMock = new();
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<ISmtpClientService> _smtpMock = new();
        private readonly Mock<IEnvironmentService> _envMock = new();

        private void SetupMailConfigurationAndEnvironment(string mailConfigurationFileName)
        {
            MailConfiguration mailConfigurationMock = TestHelper.GetMockObjectFromNameAndClass<MailConfiguration>("MailService", mailConfigurationFileName);

            _optionsMock.Setup(options => options.Value).Returns(mailConfigurationMock);

            _envMock.Setup(env => env.TerminateProgramExecution())
                    .Throws(new OperationCanceledException());
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
        public async Task MessageTypeSale_ShouldTriggerEmail()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("MailService", "AlertParamsPETR4.json");

            SetupMailConfigurationAndEnvironment("MailConfigurationMock.json");

            var service = new MailService(_optionsMock.Object, _loggerMock.Object, _smtpMock.Object, _envMock.Object);
            await service.SendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum.Sale, Convert.ToDecimal(23.10), alertParametersMock);

            AssertLogErrorFailedToSend(Times.Never());
            AssertSmtpServiceCalls(Times.Once());
            AssertEnvironmentServiceCalls(Times.Never());
        }

        [Fact]
        public async Task MessageTypePurchase_ShouldTriggerEmail()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("MailService", "AlertParamsPETR4.json");

            SetupMailConfigurationAndEnvironment("MailConfigurationMock.json");

            var service = new MailService(_optionsMock.Object, _loggerMock.Object, _smtpMock.Object, _envMock.Object);
            await service.SendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum.Purchase, Convert.ToDecimal(23.10), alertParametersMock);

            AssertLogErrorFailedToSend(Times.Never());
            AssertSmtpServiceCalls(Times.Once());
            AssertEnvironmentServiceCalls(Times.Never());
        }

        [Fact]
        public async Task MessageTypeNone_ShouldLogError()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("MailService", "AlertParamsPETR4.json");

            SetupMailConfigurationAndEnvironment("MailConfigurationMock.json");

            var service = new MailService(_optionsMock.Object, _loggerMock.Object, _smtpMock.Object, _envMock.Object);

            try
            {
                await service.SendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum.None, Convert.ToDecimal(23.10), alertParametersMock);
            }
            catch (Exception ex)
            {
                Assert.IsType<OperationCanceledException>(ex);
            }

            AssertLogErrorImproperSendingCalls(Times.Once());
            AssertSmtpServiceCalls(Times.Never());
            AssertEnvironmentServiceCalls(Times.AtLeastOnce());
        }

        [Fact]
        public async Task InvalidEmailInformation_ShouldTriggerError()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("MailService", "AlertParamsPETR4.json");

            SetupMailConfigurationAndEnvironment("MailConfigurationWithouEmailInformationMock.json");

            var service = new MailService(_optionsMock.Object, _loggerMock.Object, _smtpMock.Object, _envMock.Object);

            try
            {
                await service.SendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum.Purchase, Convert.ToDecimal(23.10), alertParametersMock);
            }
            catch (Exception ex)
            {
                Assert.IsType<OperationCanceledException>(ex);
            }

            AssertLogErrorFailedToSend(Times.Once());
            AssertSmtpServiceCalls(Times.Never());
            AssertEnvironmentServiceCalls(Times.Once());
        }

        [Fact]
        public async Task WhenSmtpFails_ShouldTriggerError()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("MailService", "AlertParamsPETR4.json");

            SetupMailConfigurationAndEnvironment("MailConfigurationMock.json");

            _smtpMock.Setup(smtp =>
                smtp.SendMailAsync(It.IsAny<MailMessage>())
            ).ThrowsAsync(new Exception());

            var service = new MailService(_optionsMock.Object, _loggerMock.Object, _smtpMock.Object, _envMock.Object);

            try
            {
                await service.SendEmailToRecipientFromTypeAndStockInformationAsync(MessageTypeEnum.Purchase, Convert.ToDecimal(23.10), alertParametersMock);
            }
            catch (Exception ex)
            {
                Assert.IsType<OperationCanceledException>(ex);
            }

            AssertLogErrorFailedToSend(Times.Once());
            AssertSmtpServiceCalls(Times.Once());
            AssertEnvironmentServiceCalls(Times.Once());
        }
    }
}