using StockQuote.Services.Interfaces;

namespace StockQuote.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public void TerminateProgramExecution()
        {
            Environment.Exit(1);
        }
    }
}