using StockQuote.Data.Dto;

namespace StockQuote.Tests.MockData
{
    public class AlertParametersDtoMockData
    {
        public static readonly AlertParametersDto alertParameters = new()
        {
            StockCode = "PETR4",
            SellPrice = 23.00m,
            BuyPrice = 22.00m
        };
    }
}