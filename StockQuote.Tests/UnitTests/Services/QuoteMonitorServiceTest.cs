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

        private void ConfigureStockApiResponse(StockInformationDto? response, String? symbol)
        {
            if (symbol != null)
            {
                _apiMock.Setup(api => api.GetStockInformationFromStockCodeAsync(symbol))
                        .ReturnsAsync(response);
            }
            else
            {
                _apiMock.Setup(api => api.GetStockInformationFromStockCodeAsync(It.IsAny<string>()))
                         .ReturnsAsync(response);
            }
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

        [Fact]
        public async Task PriceInRangeTest_ShouldNotTriggerEmail()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("QuoteMonitorService", "AlertParamsPETR4.json");
            StockInformationDto apiResponseMock = TestHelper.GetMockObjectFromNameAndClass<StockInformationDto>("QuoteMonitorService", "StockApiInRangeResponsePETR4.json");

            ConfigureStockApiResponse(apiResponseMock, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            AssertLogWarning(Times.Never());
            AssertMailMock(It.IsAny<MessageTypeEnum>(), It.IsAny<decimal>(), It.IsAny<AlertParametersDto>(), Times.Never());
        }

        [Fact]
        public async Task PriceAboveSaleThresholdTest_ShouldTriggerSaleEmail()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("QuoteMonitorService", "AlertParamsPETR4.json");
            StockInformationDto apiResponseMock = TestHelper.GetMockObjectFromNameAndClass<StockInformationDto>("QuoteMonitorService", "StockApiAboveSellThresholdResponsePETR4.json");

            ConfigureStockApiResponse(apiResponseMock, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            AssertLogWarning(Times.Never());
            AssertMailMock(MessageTypeEnum.Sale, apiResponseMock.StockPrice, alertParametersMock, Times.Once());
        }

        [Fact]
        public async Task PriceBelowPurchaseThresholdTest_ShouldTriggerPurchaseEmail()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("QuoteMonitorService", "AlertParamsPETR4.json");
            StockInformationDto apiResponseMock = TestHelper.GetMockObjectFromNameAndClass<StockInformationDto>("QuoteMonitorService", "StockApiBelowPurchaseThresholdResponsePETR4.json");

            ConfigureStockApiResponse(apiResponseMock, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            AssertLogWarning(Times.Never());
            AssertMailMock(MessageTypeEnum.Purchase, apiResponseMock.StockPrice, alertParametersMock, Times.Once());
        }

        [Fact]
        public async Task ApiResponseIsEmptyTest_ShouldLogWarning()
        {
            AlertParametersDto alertParametersMock = TestHelper.GetMockObjectFromNameAndClass<AlertParametersDto>("QuoteMonitorService", "AlertParamsPETR4.json");

            ConfigureStockApiResponse(null, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            AssertLogWarning(Times.Once());
            AssertMailMock(It.IsAny<MessageTypeEnum>(), It.IsAny<decimal>(), It.IsAny<AlertParametersDto>(), Times.Never());
        }
    }
}