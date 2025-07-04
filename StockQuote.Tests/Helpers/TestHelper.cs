using System.Text.Json;

namespace StockQuote.Tests.Helpers
{
    public static class TestHelper
    {
        public static T GetMockObjectFromNameAndClass<T>(string testClass, string fileName)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, "UnitTests", "MockData", testClass, fileName);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Arquivo de mock não encontrado: {fullPath}");

            var json = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? throw new Exception("Erro ao recuperar mock JSON.");
        }
    }
}