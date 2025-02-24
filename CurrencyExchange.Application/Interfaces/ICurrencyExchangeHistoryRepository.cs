using CurrencyExchange.Domain.Entities;

namespace CurrencyExchange.Application.Interfaces
{
    public interface ICurrencyExchangeHistoryRepository
    {
        Task SaveExchangeHistoryAsync(CurrencyExchangeHistory history);
        Task<List<CurrencyExchangeHistory>> GetExchangeHistoryAsync();
    }
}