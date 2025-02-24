using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Entities;
using CurrencyExchange.Infrastructure.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace CurrencyExchange.Application.Services
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly ICurrencyExchangeApiClient _apiClient;
        private readonly IDistributedCache _cache;
        private readonly ICurrencyRateRepository _rateRepository;
        private readonly ICurrencyExchangeHistoryRepository _historyRepository;

        public CurrencyExchangeService(
            ICurrencyExchangeApiClient apiClient,
            IDistributedCache cache,
            ICurrencyRateRepository rateRepository,
            ICurrencyExchangeHistoryRepository historyRepository)
        {
            _apiClient = apiClient;
            _cache = cache;
            _rateRepository = rateRepository;
            _historyRepository = historyRepository;
        }

        public async Task<decimal> ConvertCurrencyAsync(string baseCurrency, string targetCurrency, decimal amount)
        {
            string cacheKey = $"{baseCurrency}_{targetCurrency}";
            var cachedRate = await _cache.GetStringAsync(cacheKey);
            decimal exchangeRate;

            if (!string.IsNullOrEmpty(cachedRate))
            {
                exchangeRate = JsonConvert.DeserializeObject<decimal>(cachedRate);
            }
            else
            {
                exchangeRate = await _apiClient.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency);
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(exchangeRate),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

                await _rateRepository.SaveRateAsync(new CurrencyRate(baseCurrency, targetCurrency, exchangeRate));
            }

            decimal convertedAmount = amount * exchangeRate;
            await _historyRepository.SaveExchangeHistoryAsync(new CurrencyExchangeHistory(baseCurrency, targetCurrency, exchangeRate, convertedAmount));

            return convertedAmount;
        }

        public async Task<List<CurrencyExchangeHistory>> GetConversionHistoryAsync()
        {
            return await _historyRepository.GetExchangeHistoryAsync();
        }
    }
}