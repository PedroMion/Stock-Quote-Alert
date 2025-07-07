namespace StockQuote.Constants
{
    public class LogConstants
    {
        public const string RETRIEVING_ASSET_INFORMATION = "Recuperando informação de cotação para ativo {stockCode}";

        public const string ASSET_INFORMATION_RETRIEVED = "Informação recuperada com sucesso para ativo {stockCode}. Valor: R${stockPrice}.";

        public const string SENDING_EMAIL = "Enviando e-mail de alerta para ativo {stockCode}";

        public const string EMAIL_SENT = "Email enviado com sucesso!";

        public const string FAILED_TO_SEND_EMAIL = "Erro ao enviar email de {senderEmail} para {recipientEmail}. Por favor, verifique os dados fornecidos. Caso o problema persista, entre em contato com o responsável pelo software.";

        public const string IMPROPER_EMAIL_SENDING = "Envio de email indevido. Por favor, entre em contato com o responsável pelo software.";

        public const string QUOTE_REQUEST_FAILED = "Erro inesperado ao obter cotação do ativo. Por favor, verifique os parâmetros fornecidos para consulta da API, como chave e código do ativo. Caso o problema persista, entre em contato com o responsável pelo software.";

        public const string VALUE_ABOVE_THRESHOLD = "Valor do ativo {stockCode} ultrapassa o valor definido para venda. Valor de venda: R${valor}.";

        public const string VALUE_BELOW_THRESHOLD = "Valor do ativo {stockCode} abaixo do valor definido para compra. Valor de compra: R${valor}.";

        public const string STARTED_MONITORING_ASSETS = "Iniciando monitoramento de ativos: {Assets}.";

        public const string WAITING_FOR_NEXT_ROUND = "Verificação concluída para todos os ativos! Próxima rodada em {delayBetweenChecks} segundos.";

        public const string ASSET_MONITORING_ERROR = "Erro ao monitorar ativo: {Asset}. Por favor, verifique os parâmetros fornecidos. Caso o erro persista, entre em contato com o responsável pelo software.";

        public const string ASSET_MONITORING_COMPLETED = "Verificação completa para ativo: {Asset}. Aguardando próxima rodada.";

        public const string BAD_PARAMETERS_ERROR = "Os valores monetários fornecidos devem ser números decimais válidos, com ponto como separador, e o valor de venda deve ser maior do que o valor de compra.";

        public const string HOW_TO_USE_PROGRAM_ERROR = """
                Padrão de uso esperado: ./StockQuote <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>
                Ex.: ./StockQuote PETR4 22.67 22.59
                Caso esse padrão não seja observado, o programa tentará extrair os ativos a serem monitorados das configurações de usuário.
                Ex.: "MonitoredAssets": [
                        { "StockCode": "PETR4.SA", "SellPrice": 22.59, "BuyPrice": 22.67 }
                     ]
                Por favor, forneça as informações de uma das formas válidas.
        """;
    }
}