using StockQuote.Data.Dto;

namespace StockQuote.Services.Interfaces
{
    public interface IQuoteMonitorService
    {
        public Task CheckStockQuoteAndSendEmailAsync(AlertParametersDto parameters);
    }
}