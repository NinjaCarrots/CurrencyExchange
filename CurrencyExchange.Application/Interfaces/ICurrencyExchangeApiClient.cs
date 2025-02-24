namespace CurrencyExchange.Infrastructure.ExternalServices
{
    public interface ICurrencyExchangeApiClient
    {
        Task<decimal> GetCurrencyExchangeRateAsync(string baseCurrency, string targetCurrency);
    }
}
