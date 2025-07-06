using StockQuote.Configuration;

namespace StockQuote.Tests.MockData
{
    public class GeneralConfigurationMockData
    {
        public static readonly GeneralConfiguration generalConfiguration = new()
        {
            LogInformation = false,
            DelayBetweenChecksInSeconds = 30,
            MaxParallelChecks = 4,
        };
    }
}