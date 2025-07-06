using StockQuote.Configuration;

namespace StockQuote.Tests.MockData
{
    public class MailConfigurationMockData
    {
        public static readonly MailConfiguration mailConfiguration = new()
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            SenderEmail = "abc@gmail.com",
            SenderPassword = "123",
            RecipientEmail = "abc@gmail.com"
        };
        
        public static readonly MailConfiguration mailConfigurationWithouEmailInformation = new()
        {
            SmtpServer = "smtp.gmail.com",
            SmtpPort = 587,
            SenderEmail = "",
            SenderPassword = "",
            RecipientEmail = ""
        };
    }
}