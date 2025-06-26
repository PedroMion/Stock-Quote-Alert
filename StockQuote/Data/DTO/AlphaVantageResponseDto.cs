using System.Text.Json.Serialization;

namespace StockQuote.Data.Dto
{
    public class GlobalQuoteDto
    {
        [JsonPropertyName("Global Quote")]
        public required StockInformationDto GlobalQuote { get; set; }
    }

    public class StockInformationDto
    {
        [JsonPropertyName("01. symbol")]
        public required string StockCode { get; set; }

        [JsonPropertyName("05. price")]
        public required decimal StockPrice { get; set; }
    }
}