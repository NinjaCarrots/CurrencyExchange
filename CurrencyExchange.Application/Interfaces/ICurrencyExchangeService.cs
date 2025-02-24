using CurrencyExchange.Domain.Entities;

namespace CurrencyExchange.Application.Interfaces;

public interface ICurrencyExchangeService
{
    Task<decimal> ConvertCurrencyAsync(string baseCurrency, string targetCurrency, decimal amount);
    Task<List<CurrencyExchangeHistory>> GetConversionHistoryAsync();
}