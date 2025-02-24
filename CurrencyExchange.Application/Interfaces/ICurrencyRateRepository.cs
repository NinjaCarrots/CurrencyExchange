using CurrencyExchange.Domain.Entities;
namespace CurrencyExchange.Application.Interfaces
{
    public interface ICurrencyRateRepository
    {
        Task SaveRateAsync(CurrencyRate rate);
        Task<List<CurrencyRate>> GetHistoricalRatesAsync();
    }
}
