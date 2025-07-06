using System.Globalization;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Services;
using StockQuote.Services.Interfaces;

namespace StockQuote.Helpers
{
    public static class InitializationHelper
    {
        private static List<AlertParametersDto>? ParseAlertParametersFromConsole(string[] consoleArgs)
        {
            string symbol = consoleArgs[0];

            if (!decimal.TryParse(consoleArgs[1], CultureInfo.InvariantCulture, out var sellPrice) ||
                !decimal.TryParse(consoleArgs[2], CultureInfo.InvariantCulture, out var buyPrice))
            {
                return null;
            }

            var alertParameters = new AlertParametersDto()
            {
                StockCode = symbol,
                BuyPrice = buyPrice,
                SellPrice = sellPrice
            };

            return [alertParameters];
        }

        public static List<AlertParametersDto>? GetAlertParametersList(string[] consoleArgs, List<AlertParametersDto> alertParametersFromJson)
        {
            if (consoleArgs.Length == 3)
            {
                var parsedParams = ParseAlertParametersFromConsole(consoleArgs);

                if (parsedParams == null)
                {
                    Console.WriteLine(LogConstants.BAD_PARAMETERS_ERROR);
                }

                return parsedParams;
            }
            else if (alertParametersFromJson.Count > 0)
            {
                return alertParametersFromJson;
            }
            else
            {
                Console.WriteLine(LogConstants.HOW_TO_USE_PROGRAM_ERROR);

                return null;
            }
        }
    }
}