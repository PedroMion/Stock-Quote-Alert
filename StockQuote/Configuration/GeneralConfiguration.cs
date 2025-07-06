namespace StockQuote.Configuration
{
    public class GeneralConfiguration
    {
        public bool LogInformation { get; set; } = false;

        public int DelayBetweenChecksInSeconds { get; set; } = 30;

        public int MaxParallelChecks { get; set; } = 4;
    }
}