using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockQuote.Configuration;
using StockQuote.Data.Dto;
using StockQuote.Helpers;
using StockQuote.Services;
using StockQuote.Services.Interfaces;

namespace StockQuote
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool error = false;

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
                    services.AddScoped<ISmtpClientService, SmtpClientService>();
                    services.AddScoped<IEnvironmentService, EnvironmentService>();

                    var monitoredAssetsFromJson = context.Configuration
                                                 .GetSection("MonitoredAssets")
                                                 .Get<List<AlertParametersDto>>() ?? [];

                    var alertParameters = InitializationHelper.GetAlertParametersList(args, monitoredAssetsFromJson);

                    if (alertParameters != null)
                    {
                        services.AddSingleton(alertParameters);
                    }
                    else
                    {
                        error = true;
                    }

                    services.AddHostedService<QuoteMonitorWorker>();
                })
                .Build();

            if (error)
            {
                return;
            }

            host.Run();
        }
    }
}