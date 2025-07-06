using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using StockQuote.Configuration;
using StockQuote.Services;

namespace StockQuote.Tests.Services
{
    public class LoggerServiceTest
    {
        Mock<IOptions<GeneralConfiguration>> _optionsMock = new();
        Mock<ILogger<LoggerService>> _loggerMock = new();

        private void SetupUserConfiguration(bool logInformation)
        {
            GeneralConfiguration config = new GeneralConfiguration
            {
                LogInformation = logInformation
            };

            _optionsMock.Setup(options => options.Value).Returns(config);
        }

        private void CallLogInformationOnInstance(string message)
        {
            var service = new LoggerService(_optionsMock.Object, _loggerMock.Object);
            service.LogInformation(message);
        }

        private void AssertLogInformation(Times times)
        {
            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Teste")),
                    It.IsAny<Exception?>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
                ),
                times
            );
        }

        [Fact]
        public void LogWarningConfigurationFalseTest_ShouldNotLogInformation()
        {
            SetupUserConfiguration(logInformation: false);

            CallLogInformationOnInstance("Teste");

            AssertLogInformation(Times.Never());
        }

        [Fact]
        public void LogWarningConfigurationTrueTest_ShouldLogInformation()
        {
            SetupUserConfiguration(logInformation: true);

            CallLogInformationOnInstance("Teste");

            AssertLogInformation(Times.Once());
        }
    }
}