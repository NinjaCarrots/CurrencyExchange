using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Entities;
using CurrencyExchange.Infrastructure.ExternalServices;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CurrencyExchange.Application.Services
{
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly ILogger<CurrencyRate> _logger;
        private readonly ICurrencyExchangeApiClient _apiClient;
        private readonly IDistributedCache _cache;
        private readonly ICurrencyRateRepository _rateRepository;
        private readonly ICurrencyExchangeHistoryRepository _historyRepository;

        public CurrencyExchangeService(
            ILogger<CurrencyRate> logger,
            ICurrencyExchangeApiClient apiClient,
            IDistributedCache cache,
            ICurrencyRateRepository rateRepository,
            ICurrencyExchangeHistoryRepository historyRepository)
        {
            _logger = logger;
            _apiClient = apiClient;
            _cache = cache;
            _rateRepository = rateRepository;
            _historyRepository = historyRepository;
        }

        public async Task<decimal> ConvertCurrencyAsync(string baseCurrency, string targetCurrency, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(baseCurrency) || string.IsNullOrWhiteSpace(targetCurrency))
                throw new ArgumentException("Base currency and target currency must be provided.");

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero.");

            if (baseCurrency.Equals(targetCurrency, StringComparison.OrdinalIgnoreCase))
                return amount; // No conversion needed if currencies are the same.

            string cacheKey = $"{baseCurrency}_{targetCurrency}";
            decimal exchangeRate = 0m;
            var cachedRate = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedRate))
            {
                try
                {
                    exchangeRate = JsonConvert.DeserializeObject<decimal>(cachedRate);
                }
                catch (JsonException)
                {
                    _logger.LogWarning("Failed to deserialize cache for key: {CacheKey}. Fetching from API.", cacheKey);
                }
            }

            // Fetch from API if cache is empty or corrupted
            if (exchangeRate == 0)
            {
                exchangeRate = await _apiClient.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency);

                // Store in cache (15 min expiration)
                await _cache.SetStringAsync(cacheKey, JsonConvert.SerializeObject(exchangeRate),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15) });

                // Persist the rate in the database
                await _rateRepository.SaveRateAsync(new CurrencyRate(amount, baseCurrency, targetCurrency, exchangeRate));
            }

            decimal convertedAmount = amount * exchangeRate;

            // Save conversion history
            await _historyRepository.SaveExchangeHistoryAsync(new CurrencyExchangeHistory(amount, baseCurrency, targetCurrency, exchangeRate, convertedAmount));

            return convertedAmount;
        }

        public async Task<List<CurrencyExchangeHistory>> GetConversionHistoryAsync()
        {
            return await _historyRepository.GetExchangeHistoryAsync();
        }
    }
}