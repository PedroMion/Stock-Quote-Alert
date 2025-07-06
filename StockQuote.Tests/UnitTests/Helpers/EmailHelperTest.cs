using StockQuote.Constants;
using StockQuote.Data.Dto;
using StockQuote.Enums;
using StockQuote.Helpers;
using StockQuote.Tests.MockData;

namespace StockQuote.Tests.Helpers
{
    public class EmailHelperTest
    {
        private readonly AlertParametersDto _alertParameters = AlertParametersDtoMockData.alertParameters;

        private void GetEmailSubjectAndCompareToExpected(MessageTypeEnum messageType, string expectedSubject)
        {
            string subject = EmailHelper.GetEmailSubjectFromTypeAndStockInformation(messageType, _alertParameters);

            Assert.Equal(subject, expectedSubject.Replace(EmailConstants.STOCK_CODE_PLACEHOLDER, _alertParameters.StockCode));
        }

        [Fact]
        public void GetPurchaseEmailSubjectTest_ShouldReturnPurchaseSubject()
        {
            GetEmailSubjectAndCompareToExpected(MessageTypeEnum.Purchase, EmailConstants.PURCHASE_SUBJECT);
        }
        
        [Fact]
        public void GetSaleEmailSubjectTest_ShouldReturnSaleSubject()
        {
           GetEmailSubjectAndCompareToExpected(MessageTypeEnum.Sale, EmailConstants.SALE_SUBJECT);
        }
    }
}