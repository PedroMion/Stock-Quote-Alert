using Moq;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Services;
using StockQuote.Services.Interfaces;
using StockQuote.Tests.MockData;

namespace StockQuote.Tests.Services
{
    public class QuoteMonitorServiceTests
    {
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<IMailService> _mailMock = new();
        private readonly Mock<IStockApiService> _apiMock = new();
        private readonly AlertParametersDto _alertParameters = AlertParametersDtoMockData.alertParameters;
        private readonly StockInformationDto _stockApiInRangeResponse = StockInformationDtoMockData.stockApiInRangeResponse;
        private readonly StockInformationDto _stockApiBelowPurchaseThresholdResponse = StockInformationDtoMockData.stockApiBelowPurchaseThresholdResponse;
        private readonly StockInformationDto _stockApiAboveSellThresholdResponse = StockInformationDtoMockData.stockApiAboveSellThresholdResponse;

        private void ConfigureStockApiResponse(StockInformationDto? apiResponseMock)
        {
            _apiMock.Setup(api => api.GetStockInformationFromStockCodeAsync(_alertParameters.StockCode))
                    .ReturnsAsync(apiResponseMock);
        }

        private void AssertLogWarning(Times times)
        {
            _loggerMock.Verify(l => l.LogWarning(LogConstants.QuoteRequestFailed), times);
        }

        private void AssertMailMock(MessageTypeEnum messageType, decimal value, AlertParametersDto alertParametersDto, Times times)
        {
            _mailMock.Verify(m =>
                m.SendEmailToRecipientFromTypeAndStockInformationAsync(
                    messageType,
                    value,
                    alertParametersDto
                ),
                times
            );
        }

        private async Task CallCheckStockQuoteAndSendEmailAsync()
        {
            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);

            await service.CheckStockQuoteAndSendEmailAsync(_alertParameters);
        }

        [Fact]
        public async Task PriceInRangeTest_ShouldNotTriggerEmail()
        {
            ConfigureStockApiResponse(_stockApiInRangeResponse);

            await CallCheckStockQuoteAndSendEmailAsync();

            AssertLogWarning(Times.Never());
            AssertMailMock(It.IsAny<MessageTypeEnum>(), It.IsAny<decimal>(), It.IsAny<AlertParametersDto>(), Times.Never());
        }

        [Fact]
        public async Task PriceAboveSaleThresholdTest_ShouldTriggerSaleEmail()
        {
            ConfigureStockApiResponse(_stockApiAboveSellThresholdResponse);

            await CallCheckStockQuoteAndSendEmailAsync();

            AssertLogWarning(Times.Never());
            AssertMailMock(MessageTypeEnum.Sale, _stockApiAboveSellThresholdResponse.StockPrice, _alertParameters, Times.Once());
        }

        [Fact]
        public async Task PriceBelowPurchaseThresholdTest_ShouldTriggerPurchaseEmail()
        {
            ConfigureStockApiResponse(_stockApiBelowPurchaseThresholdResponse);

            await CallCheckStockQuoteAndSendEmailAsync();

            AssertLogWarning(Times.Never());
            AssertMailMock(MessageTypeEnum.Purchase, _stockApiBelowPurchaseThresholdResponse.StockPrice, _alertParameters, Times.Once());
        }

        [Fact]
        public async Task ApiResponseIsEmptyTest_ShouldLogWarning()
        {
            ConfigureStockApiResponse(null);

            await CallCheckStockQuoteAndSendEmailAsync();

            AssertLogWarning(Times.Once());
            AssertMailMock(It.IsAny<MessageTypeEnum>(), It.IsAny<decimal>(), It.IsAny<AlertParametersDto>(), Times.Never());
        }
    }
}