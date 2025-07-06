using Microsoft.Extensions.Options;
using Moq;
using StockQuote.Configuration;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Services.Interfaces;
using StockQuote.Tests.MockData;

namespace StockQuote.Tests.Workers
{
    public class QuoteMonitorWorkerTests
    {
        private readonly Mock<IOptions<GeneralConfiguration>> _optionsMock = new();
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<IQuoteMonitorService> _monitorMock = new();
        private readonly Mock<IEnvironmentService> _envMock = new();
        private readonly AlertParametersDto _alertParameters = AlertParametersDtoMockData.alertParameters;
        private readonly GeneralConfiguration _configuration = GeneralConfigurationMockData.generalConfiguration;

        private QuoteMonitorWorker GetWorkerInstance()
        {
            _optionsMock.Setup(options => options.Value).Returns(_configuration);

            return new QuoteMonitorWorker(
                _optionsMock.Object,
                _loggerMock.Object,
                _monitorMock.Object,
                [_alertParameters],
                _envMock.Object
            );
        }

        private void SetupQuoteServiceThrowException()
        {
            _monitorMock
                .Setup(m => m.CheckStockQuoteAndSendEmailAsync(_alertParameters))
                .ThrowsAsync(new Exception(""));
        }

        private async Task ExecuteWorkerMethodAsync(int cancellationTimeInMilliseconds)
        {
            var worker = GetWorkerInstance();

            CancellationTokenSource token = new CancellationTokenSource();
            token.CancelAfter(cancellationTimeInMilliseconds);

            await worker.StartAsync(token.Token);
        }

        private void AssertLogInformation()
        {
            _loggerMock.Verify(l => l.LogInformation(LogConstants.ASSET_MONITORING_COMPLETED, _alertParameters.StockCode), Times.AtLeastOnce);
        }

        private void AssertLogError()
        {
            _loggerMock.Verify(l => l.LogError(It.IsAny<Exception>(), LogConstants.ASSET_MONITORING_ERROR, _alertParameters.StockCode), Times.AtLeastOnce);
        }

        private void AssertMonitorService()
        {
            _monitorMock.Verify(m => m.CheckStockQuoteAndSendEmailAsync(_alertParameters), Times.AtLeastOnce);
        }

        private void AssertEnvironmentTermination()
        {
            _envMock.Verify(e => e.TerminateProgramExecution(), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ExecuteWithoutException_ShouldNotProduceError()
        {
            await ExecuteWorkerMethodAsync(cancellationTimeInMilliseconds: 100);

            AssertLogInformation();
            AssertMonitorService();
        }

        [Fact]
        public async Task ExecuteWithException_ShouldLogErrorAndTerminate()
        {
            SetupQuoteServiceThrowException();
            await ExecuteWorkerMethodAsync(cancellationTimeInMilliseconds: 100);

            AssertLogError();
            AssertEnvironmentTermination();
        }
    }
}
