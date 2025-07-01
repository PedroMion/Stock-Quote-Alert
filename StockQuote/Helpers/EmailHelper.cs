using System.Globalization;
using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Enums;

namespace StockQuote.Helpers
{
    public class EmailHelper
    {
        private static string GetSpecificMailMessage(MessageTypeEnum emailType)
        {
            if (emailType == MessageTypeEnum.Purchase) return "abaixo do valor de compra";
            else return "acima do valor de venda";
        }

        private static string GetTresholdValueFromParametersBasedOnType(MessageTypeEnum type, AlertParametersDto parameters)
        {
            if (type == MessageTypeEnum.Purchase)
            {
                return parameters.BuyPrice.ToString("N2");
            }
            else
            {
                return parameters.SellPrice.ToString("N2");   
            }
        }

        public static string GetEmailSubjectFromTypeAndStockInformation(MessageTypeEnum emailType, AlertParametersDto parameters)
        {
            if (emailType == MessageTypeEnum.Sale) return EmailConstants.SALE_SUBJECT.Replace(EmailConstants.STOCK_CODE_PLACEHOLDER, parameters.StockCode);
            else return EmailConstants.PURCHASE_SUBJECT.Replace(EmailConstants.STOCK_CODE_PLACEHOLDER, parameters.StockCode);
        }

        public static string GetEmailBodyFromTypeAndStockInformation(MessageTypeEnum emailType, decimal stockValue, AlertParametersDto parameters)
        {
            return EmailConstants.BODY.Replace(EmailConstants.STOCK_CODE_PLACEHOLDER, parameters.StockCode)
                .Replace(EmailConstants.DATE_TIME_PLACEHOLDER, DateTime.Now.ToString(new CultureInfo("pt-BR")))
                .Replace(EmailConstants.EMAIL_TYPE_PLACEHOLDER, GetSpecificMailMessage(emailType))
                .Replace(EmailConstants.TRESHOLD_VALUE_PLACEHOLDER, GetTresholdValueFromParametersBasedOnType(emailType, parameters))
                .Replace(EmailConstants.STOCK_VALUE_PLACEHOLDER, stockValue.ToString("N2"));
        }
    }
}