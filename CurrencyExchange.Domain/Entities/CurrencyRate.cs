namespace CurrencyExchange.Domain.Entities
{
    public class CurrencyRate
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public DateTime Timestamp { get; set; }

        public CurrencyRate(decimal amount, string baseCurrency, string targetCurrency, decimal exchangeRate)
        {
            Amount = amount;
            BaseCurrency = baseCurrency;
            TargetCurrency = targetCurrency;
            ExchangeRate = exchangeRate;
            Timestamp = DateTime.UtcNow;
        }
    }
}
