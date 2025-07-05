using Moq;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Services;
using StockQuote.Services.Interfaces;
using StockQuote.Tests.Helpers;

namespace StockQuote.Tests.Services
{
    public class QuoteMonitorServiceTests
    {
        private readonly Mock<ILoggerService> _loggerMock = new();
        private readonly Mock<IMailService> _mailMock = new();
        private readonly Mock<IStockApiService> _apiMock = new();
        private readonly AlertParametersDto _alertParameters = new()
        {
            StockCode = "PETR4",
            BuyPrice = 22.00m,
            SellPrice = 23.00m
        };

        private StockInformationDto GetApiResponseMockFromFileName(string fileName)
        {
            return TestHelper.GetMockObjectFromNameAndClass<StockInformationDto>("QuoteMonitorService", fileName);
        }

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
            var apiResponseMock = GetApiResponseMockFromFileName("StockApiInRangeResponsePETR4.json");
            ConfigureStockApiResponse(apiResponseMock);

            await CallCheckStockQuoteAndSendEmailAsync();

            AssertLogWarning(Times.Never());
            AssertMailMock(It.IsAny<MessageTypeEnum>(), It.IsAny<decimal>(), It.IsAny<AlertParametersDto>(), Times.Never());
        }

        [Fact]
        public async Task PriceAboveSaleThresholdTest_ShouldTriggerSaleEmail()
        {
            var apiResponseMock = GetApiResponseMockFromFileName("StockApiAboveSellThresholdResponsePETR4.json");
            ConfigureStockApiResponse(apiResponseMock);

            await CallCheckStockQuoteAndSendEmailAsync();

            AssertLogWarning(Times.Never());
            AssertMailMock(MessageTypeEnum.Sale, apiResponseMock.StockPrice, _alertParameters, Times.Once());
        }

        [Fact]
        public async Task PriceBelowPurchaseThresholdTest_ShouldTriggerPurchaseEmail()
        {
            var apiResponseMock = GetApiResponseMockFromFileName("StockApiBelowPurchaseThresholdResponsePETR4.json");
            ConfigureStockApiResponse(apiResponseMock);

            await CallCheckStockQuoteAndSendEmailAsync();

            AssertLogWarning(Times.Never());
            AssertMailMock(MessageTypeEnum.Purchase, apiResponseMock.StockPrice, _alertParameters, Times.Once());
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