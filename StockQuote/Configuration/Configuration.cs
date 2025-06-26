using System.Text.Json.Serialization;

namespace StockQuote.Configuration
{
    public class StockApiConfiguration
    {
        public required string Url { get; set; }

        public required string ApiKey { get; set; }

        public required string Endpoint { get; set; }
    }
}