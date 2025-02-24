namespace CurrencyExchange.Domain.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime Timestamp { get; set; }

        public CurrencyRate(string baseCurrency, string targetCurrency, decimal exchangeRate)
        {
            BaseCurrency = baseCurrency;
            TargetCurrency = targetCurrency;
            ExchangeRate = exchangeRate;
            Timestamp = DateTime.UtcNow;
        }
    }
}
