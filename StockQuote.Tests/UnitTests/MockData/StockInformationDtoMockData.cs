using StockQuote.Data.Dto;

namespace StockQuote.Tests.MockData
{
    public class StockInformationDtoMockData
    {
        public static readonly StockInformationDto stockApiAboveSellThresholdResponse = new()
        {
            StockCode = "PETR4.SA",
            StockPrice = 23.10m
        };

        public static readonly StockInformationDto stockApiBelowPurchaseThresholdResponse = new()
        {
            StockCode = "PETR4.SA",
            StockPrice = 21.90m
        };

        public static readonly StockInformationDto stockApiInRangeResponse = new()
        {
            StockCode = "PETR4.SA",
            StockPrice = 22.50m
        };
    }
}