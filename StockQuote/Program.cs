using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockQuote.Configuration;
using StockQuote.Data.Dto;
using StockQuote.Services;
using StockQuote.Services.Interfaces;

namespace StockQuote
{

    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("""
                    Padrão de uso esperado: ./StockQuote <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>
                    Ex.: ./StockQuote PETR4 22.67 22.59
                    """);
                return;
            }

            string symbol = args[0];

            if (!decimal.TryParse(args[1], CultureInfo.InvariantCulture, out var buyPrice) ||
                !decimal.TryParse(args[2], CultureInfo.InvariantCulture, out var sellPrice))
            {
                Console.WriteLine("Os valores monetários fornecidos devem ser números decimais válidos, com ponto como separador.");
                return;
            }

            var alertParams = new AlertParametersDto()
            {
                StockCode = symbol,
                BuyPrice = buyPrice,
                SellPrice = sellPrice
            };


            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<StockApiConfiguration>(context.Configuration.GetSection("StockApi"));

                    services.AddScoped<IStockApiService, StockApiService>();
                })
                .Build();

            using var scope = host.Services.CreateScope();
            var stockApiService = scope.ServiceProvider.GetRequiredService<IStockApiService>();

            try
            {
                var response = await stockApiService.GetStockInformationFromStockCodeAsync(alertParams.StockCode);

                Console.WriteLine($"Símbolo: {response.StockCode}");
                Console.WriteLine($"Preço  : {response.StockPrice}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consultar {alertParams.StockCode}: {ex.Message}");
            }
        }
    }
}