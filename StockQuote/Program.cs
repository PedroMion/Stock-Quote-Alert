using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StockQuote.Configuration;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Services;
using StockQuote.Services.Interfaces;

namespace StockQuote
{

    public class Program
    {
        public static void Main(string[] args)
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

            if (!decimal.TryParse(args[1], CultureInfo.InvariantCulture, out var sellPrice) ||
                !decimal.TryParse(args[2], CultureInfo.InvariantCulture, out var buyPrice))
            {
                Console.WriteLine("Os valores monetários fornecidos devem ser números decimais válidos, com ponto como separador.");
                return;
            }

            var alertParameters = new AlertParametersDto()
            {
                StockCode = symbol,
                BuyPrice = buyPrice,
                SellPrice = sellPrice
            };


            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                    config.AddJsonFile("appsettings.User.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<StockApiConfiguration>(context.Configuration.GetSection("StockApi"));
                    services.Configure<MailConfiguration>(context.Configuration.GetSection("MailConfiguration"));
                    services.Configure<GeneralConfiguration>(context.Configuration.GetSection("General"));

                    services.AddScoped<IStockApiService, StockApiService>();
                    services.AddScoped<IMailService, MailService>();
                    services.AddScoped<IQuoteMonitorService, QuoteMonitorService>();
                    services.AddScoped<ILoggerService, LoggerService>();
                    services.AddSingleton(alertParameters);

                    services.AddHostedService<QuoteMonitorWorker>();
                })
                .Build();

            host.Run();
        }
    }
}