namespace StockQuote.Configuration
{
    public class MailConfiguration
    {
        public required string SmtpServer { get; set; }

        public int Port { get; set; }

        public required string SenderEmail { get; set; }

        public required string SenderPassword { get; set; }

        public required string RecipientEmail { get; set; }
    }
}