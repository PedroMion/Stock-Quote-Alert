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

        public static readonly List<AlertParametersDto> alertParametersJson =
        [
            new AlertParametersDto {
                StockCode = "ITUB4",
                SellPrice = 30.00m,
                BuyPrice = 28.00m
            },
            new AlertParametersDto {
                StockCode = "VALE3",
                SellPrice = 65.00m,
                BuyPrice = 61.00m
            }
        ];
    }
}