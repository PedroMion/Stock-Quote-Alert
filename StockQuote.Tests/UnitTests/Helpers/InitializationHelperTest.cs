using StockQuote.Data.Dto;
using StockQuote.Helpers;
using StockQuote.Tests.MockData;

namespace StockQuote.Tests.Helpers
{
    public class InitializationHelperTest
    {
        private readonly AlertParametersDto _alertParameters = AlertParametersDtoMockData.alertParameters;
        private readonly List<AlertParametersDto> _alertparametersJson = AlertParametersDtoMockData.alertParametersJson;
        private readonly string[] _args = ["PETR4", "23.00", "22.00"];

        private List<AlertParametersDto>? GetParametersFromInitializationHelper(string[] args, List<AlertParametersDto>? jsonParameters)
        {
            return InitializationHelper.GetAlertParametersList(args, jsonParameters ?? new List<AlertParametersDto>());
        }

        private void AssertResponseIsExpected(List<AlertParametersDto>? expected, List<AlertParametersDto>? actual)
        {
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetAlertParametersListWhenOnlyArgsIsProvided_ShouldReturnArgs()
        {
            var response = GetParametersFromInitializationHelper(_args, null);

            AssertResponseIsExpected(expected: [_alertParameters], actual: response);
        }

        [Fact]
        public void GetAlertParametersListWhenOnlyJsonIsProvided_ShouldReturnJson()
        {
            var response = GetParametersFromInitializationHelper([], _alertparametersJson);

            AssertResponseIsExpected(expected: _alertparametersJson, actual: response);
        }

        [Fact]
        public void GetAlertParametersListWhenBothAreProvided_ShouldReturnArgs()
        {
            var response = GetParametersFromInitializationHelper(_args, _alertparametersJson);

            AssertResponseIsExpected(expected: [_alertParameters], actual: response);
        }

        [Fact]
        public void GetAlertParametersListWhithWrongArguments_ShouldReturnNull()
        {
            var response = GetParametersFromInitializationHelper(["abc", "abc", "abc"], _alertparametersJson);

            AssertResponseIsExpected(expected: null, actual: response);
        }

        [Fact]
        public void GetAlertParametersListWithoutBothParameters_ShouldReturnNull()
        {
            var response = GetParametersFromInitializationHelper([], null);

            AssertResponseIsExpected(expected: null, actual: response);
        }
    }
}