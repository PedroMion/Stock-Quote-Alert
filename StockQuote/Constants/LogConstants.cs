namespace StockQuote.Constants
{
    public class LogConstants
    {
        public const string RetrievingAssetInformation = "Recuperando informação de cotação para ativo {stockCode}";

        public const string AssetInformationRetrieved = "Informação recuperada com sucesso para ativo {stockCode}. Valor: R${stockPrice}.";

        public const string SendingEmail = "Enviando e-mail de alerta para ativo {stockCode}";

        public const string EmailSent = "Email enviado com sucesso!";

        public const string FailedToSendEmail = "Erro ao enviar email de {senderEmail} para {recipientEmail}. Por favor, verifique os dados fornecidos. Caso o problema persista, entre em contato com o responsável pelo software.";

        public const string ImproperEmailSending = "Envio de email indevido. Por favor, entre em contato com o responsável pelo software.";

        public const string QuoteRequestFailed = "Erro inesperado ao obter cotação do ativo. Por favor, verifique os parâmetros fornecidos para consulta da API, como chave e código do ativo. Caso o problema persista, entre em contato com o responsável pelo software.";

        public const string ValueAboveThreshold = "Valor do ativo {stockCode} ultrapassa o valor definido para venda. Valor de venda: R${valor}.";

        public const string ValueBelowThreshold = "Valor do ativo {stockCode} abaixo do valor definido para compra. Valor de compra: R${valor}.";

        public const string AssetMonitoringError = "Erro ao monitorar ativo: {Asset}. Por favor, entre em contato com o responsável pelo software.";

        public const string AssetMonitoringCompleted = "Verificação completa para ativo: {Asset}. Aguardando próxima rodada.";
    }
}