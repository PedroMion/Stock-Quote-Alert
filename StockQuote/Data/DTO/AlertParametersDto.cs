namespace StockQuote.Data.Dto
{
    public record AlertParametersDto
    {
        public required string StockCode { get; init; }

        public required decimal BuyPrice { get; init; }

        public required decimal SellPrice { get; init; }
    }
}