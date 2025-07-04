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

        private T GetMockObjectFromName<T>(string fileName)
        {
            string filePath = Path.Combine("QuoteMonitorService", fileName);

            return TestHelper.LoadObject<T>(filePath);
        }

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

        [Fact]
        public async Task PriceInRangeTest_ShouldNotTriggerEmail()
        {
            AlertParametersDto alertParametersMock = GetMockObjectFromName<AlertParametersDto>("AlertParamsPETR4.json");
            StockInformationDto apiResponseMock = GetMockObjectFromName<StockInformationDto>("StockApiInRangeResponsePETR4.json");

            ConfigureStockApiResponse(apiResponseMock, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            _loggerMock.Verify(l => l.LogWarning(LogConstants.QuoteRequestFailed), Times.Never);

            _mailMock.Verify(m =>
                m.SendEmailToRecipientFromTypeAndStockInformationAsync(
                    It.IsAny<MessageTypeEnum>(),
                    It.IsAny<decimal>(),
                    It.IsAny<AlertParametersDto>()
                ),
                Times.Never
            );
        }

        [Fact]
        public async Task PriceAboveSaleThresholdTest_ShouldTriggerSaleEmail()
        {
            AlertParametersDto alertParametersMock = GetMockObjectFromName<AlertParametersDto>("AlertParamsPETR4.json");
            StockInformationDto apiResponseMock = GetMockObjectFromName<StockInformationDto>("StockApiAboveSellThresholdResponsePETR4.json");

            ConfigureStockApiResponse(apiResponseMock, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            _loggerMock.Verify(l => l.LogWarning(LogConstants.QuoteRequestFailed), Times.Never);

            _mailMock.Verify(m =>
                m.SendEmailToRecipientFromTypeAndStockInformationAsync(
                    MessageTypeEnum.Sale,
                    apiResponseMock.StockPrice,
                    alertParametersMock
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task PriceBelowPurchaseThresholdTest_ShouldTriggerPurchaseEmail()
        {
            AlertParametersDto alertParametersMock = GetMockObjectFromName<AlertParametersDto>("AlertParamsPETR4.json");
            StockInformationDto apiResponseMock = GetMockObjectFromName<StockInformationDto>("StockApiBelowPurchaseThresholdResponsePETR4.json");

            ConfigureStockApiResponse(apiResponseMock, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            _loggerMock.Verify(l => l.LogWarning(LogConstants.QuoteRequestFailed), Times.Never);

            _mailMock.Verify(m =>
                m.SendEmailToRecipientFromTypeAndStockInformationAsync(
                    MessageTypeEnum.Purchase,
                    apiResponseMock.StockPrice,
                    alertParametersMock
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task ApiResponseIsEmptyTest_ShouldLogWarning()
        {
            AlertParametersDto alertParametersMock = GetMockObjectFromName<AlertParametersDto>("AlertParamsPETR4.json");

            ConfigureStockApiResponse(null, alertParametersMock.StockCode);

            var service = new QuoteMonitorService(_loggerMock.Object, _mailMock.Object, _apiMock.Object);
            await service.CheckStockQuoteAndSendEmailAsync(alertParametersMock);

            _loggerMock.Verify(l => l.LogWarning(LogConstants.QuoteRequestFailed), Times.Once);

            _mailMock.Verify(m =>
                m.SendEmailToRecipientFromTypeAndStockInformationAsync(
                    It.IsAny<MessageTypeEnum>(),
                    It.IsAny<decimal>(),
                    It.IsAny<AlertParametersDto>()
                ),
                Times.Never
            );
        }
    }
}