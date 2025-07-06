namespace StockQuote.Constants
{
    public class EmailConstants
    {
        public const string STOCK_CODE_PLACEHOLDER = "{STOCK_CODE}";

        public const string STOCK_VALUE_PLACEHOLDER = "{STOCK_VALUE}";

        public const string DATE_TIME_PLACEHOLDER = "{DATE_TIME}";

        public const string EMAIL_TYPE_PLACEHOLDER = "{EMAIL_TYPE}";

        public const string THRESHOLD_VALUE_PLACEHOLDER = "{THRESHOLD_VALUE}";

        public const string SALE_SUBJECT = "[StockQuote] Alerta de venda - {STOCK_CODE}";

        public const string PURCHASE_SUBJECT = "[StockQuote] Alerta de compra - {STOCK_CODE}";

        public const string EMAIL_MESSAGE_PURCHASE = "abaixo do valor de compra";

        public const string EMAIL_MESSAGE_SALE = "acima do valor de venda";

        public const string BODY = """
        <!DOCTYPE html>
        <html lang="pt-BR">
        <head>
            <meta charset="UTF-8">
            <title>Alerta de Cotação</title>
            <style>
                body {
                    font-family: Arial, sans-serif;
                    background-color: #f7f9fc;
                    color: black;
                    margin: 0;
                    padding: 0.2rem;
                }
                .container {
                    background-color: #ffffff;
                    border-radius: 0.1rem;
                    padding: 0.4rem;
                    max-width: 45rem;
                    margin: auto;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                }
                .header {
                    border-bottom: 1px solid #e0e0e0;
                    margin-bottom: 0.4rem;
                }
                .header h2 {
                    margin: 0;
                    color: lightblue;
                }
                .content p {
                    margin: 0.2rem;
                }
                .highlight {
                    font-weight: bold;
                }
                .footer {
                    margin-top: 1rem;
                    font-size: 0.9rem;
                    color: gray;
                    text-align: center;
                }
            </style>
        </head>
        <body>
            <div class="container">
                <div class="header">
                    <h2>Alerta de Cotação</h2>
                </div>
                <div class="content">
                    <p>Prezado(a),</p>
                    <p>O ativo <span class="highlight">{STOCK_CODE}</span> atingiu o valor de <span class="highlight">R${STOCK_VALUE}</span> em <span class="highlight">{DATE_TIME}</span>.</p>
                    <p>O valor atestado encontra-se {EMAIL_TYPE} fornecido de <span class="highlight">R${THRESHOLD_VALUE}</span>.</p>
                    <p>Recomenda-se avaliar a ação necessária com base em sua estratégia.</p>
                </div>
                <div class="footer">
                    Este é um alerta automático. Por favor, não responda a este e-mail.
                </div>
            </div>
        </body>
        </html>
        """;
    }
}