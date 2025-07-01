using Microsoft.Extensions.Options;
using StockQuote.Configuration;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Services.Interfaces;
using System.Net.Http.Json;

namespace StockQuote.Services
{
    public class StockApiService(IOptions<StockApiConfiguration> options) : IStockApiService
    {
        private readonly StockApiConfiguration _config = options.Value;
        private readonly HttpClient _httpClient = new HttpClient();

        private string GetStockInformationEndpointUrl(string stockCode)
        {
            return _config.Url
                        .Replace(AlphaVantageConstants.ENDPOINT_PLACEHOLDER, _config.Endpoint)
                        .Replace(AlphaVantageConstants.SYMBOL_PLACEHOLDER, stockCode)
                        + _config.ApiKey;
        }

        public async Task<StockInformationDto?> GetStockInformationFromStockCodeAsync(string stockCode)
        {
            string url = GetStockInformationEndpointUrl(stockCode);

            var response = await _httpClient.GetFromJsonAsync<GlobalQuoteDto>(url);

            return response?.GlobalQuote;
        }
    }
}