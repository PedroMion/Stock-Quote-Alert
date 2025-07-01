using StockQuote.Data.Dto;

namespace StockQuote.Services.Interfaces
{
    public interface IStockApiService
    {
        public Task<StockInformationDto?> GetStockInformationFromStockCodeAsync(String symbol);
    }
}