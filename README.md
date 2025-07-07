# Stock Quote Alert

## 📖 Sobre este projeto
Este projeto consiste em uma aplicação que monitora ativos da bolsa brasileira e envia e-mails quando o valor ultrapassa os limites de compra ou venda fornecidos pelo usuário. A aplicação é controlada via console e por meio de arquivos de configuração, **não possuindo interface gráfica**.

## 📺 Tecnologias
- Aplicação: C#  
- API de cotações: Alpha Vantage

## 🔧 Configurações de usuário
Para que a aplicação funcione corretamente, é necessário preencher alguns dados no arquivo de configuração **`appsettings.User.json`**.

### ⚠️ Obrigatórios

**Seção `StockApi`:**  
- `ApiKey`: Preencha com a sua chave da API Alpha Vantage. Devido às particularidades do retorno, não é possível substituir por outra API diretamente.

**Seção `MailConfiguration`:**  
- `SmtpServer`: Servidor SMTP para envio do e-mail.  
- `SmtpPort`: Porta de envio do e-mail.  
- `SenderEmail`: Endereço de e-mail do remetente.  
- `SenderPassword`: Senha real ou senha de aplicativo do e-mail de envio.  
- `RecipientEmail`: Endereço de e-mail do destinatário.

### 🛠️ Opcionais

**Seção `General`:**  
- `LogInformation`: Valor booleano que determina se as informações serão exibidas no console.  
- `DelayBetweenChecksInSeconds`: Intervalo de tempo (em segundos) entre as checagens.  
- `MaxParallelChecks`: Número máximo de threads utilizadas para checagem em paralelo. **Não é recomendado utilizar um valor elevado.**

**Seção `MonitoredAssets`:**  
Você pode preencher essa lista com os ativos a serem monitorados caso não deseje passar os atributos via linha de comando ou deseje monitorar mais de um ativo simultaneamente. Cada objeto deve seguir o formato abaixo:

```json
{ "StockCode": "CódigoDoAtivo", "SellPrice": LimiteParaVenda "BuyPrice": LimiteParaCompra }
```

Exemplo:

```json
"MonitoredAssets": [
    { "StockCode": "PETR4", "SellPrice": 24.00, "BuyPrice": 22.50 },
    { "StockCode": "VALE3", "SellPrice": 65.00, "BuyPrice": 61.00 },
    { "StockCode": "ITUB4", "SellPrice": 30.00, "BuyPrice": 28.00 }
]
```

## 📰 Modo de uso
Clone o repositório. Na pasta raiz do projeto, execute o seguinte comando para gerar o build da aplicação:

```bash
dotnet publish
```

Após o publish, vá até a pasta de saída:

```bash
cd "./bin/Release/net9.0/publish"
```

## ✅ Execução com parâmetros via console
Execute o programa com os seguintes argumentos:

```bash
./StockQuote <ATIVO> <PRECO_VENDA> <PRECO_COMPRA>
```

Exemplo:

```bash
./StockQuote PETR4 22.67 22.59
```

## ✅ Execução múltiplos ativos via configuração
Caso deseje monitorar múltiplos ativos, configure a lista no arquivo de configuração de usuário no padrão descrito na seção de configuração e execute sem os parâmetros.
Exemplo:

```bash
./StockQuote
```
Note que a aplicação sempre dará preferência para o método via console. Portanto, caso queira utilizar o método via json, execute sem os argumentos.

A partir disso, é só aproveitar!