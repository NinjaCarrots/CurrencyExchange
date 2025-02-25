using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Application.Services;
using CurrencyExchange.Infrastructure.ExternalServices;
using FluentAssertions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using System.Text.Json;

namespace CurrencyExchange.Tests.Services
{
    public class CurrencyExchangeServiceTests
    {
        private readonly Mock<ICurrencyExchangeApiClient> _mockApiClient;
        private readonly Mock<IDistributedCache> _mockCache;
        private readonly Mock<ICurrencyRateRepository> _mockRateRepository;
        private readonly Mock<ICurrencyExchangeHistoryRepository> _mockHistoryRepository;
        private readonly CurrencyExchangeService _currencyExchangeService;

        public CurrencyExchangeServiceTests()
        {
            _mockApiClient = new Mock<ICurrencyExchangeApiClient>();
            _mockCache = new Mock<IDistributedCache>();
            _mockRateRepository = new Mock<ICurrencyRateRepository>();
            _mockHistoryRepository = new Mock<ICurrencyExchangeHistoryRepository>();
            _currencyExchangeService = new CurrencyExchangeService(
                _mockApiClient.Object,
                _mockCache.Object,
                _mockRateRepository.Object,
                _mockHistoryRepository.Object
            );
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldReturnCorrectAmount_WhenExchangeRateIsValid()
        {
            // Arrange
            string baseCurrency = "USD";
            string targetCurrency = "EUR";
            decimal amount = 100;
            decimal exchangeRate = 0.85m;
            string cacheKey = $"exchange_rate_{baseCurrency}_{targetCurrency}";

            _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[]?)null);  // Explicitly setting a nullable byte array

            _mockApiClient
                .Setup(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency))
                .ReturnsAsync(exchangeRate);

            // Act
            var result = await _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount);

            // Assert
            result.Should().Be(amount * exchangeRate);
            _mockApiClient.Verify(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency), Times.Once);
            _mockCache.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldReturnCachedRate_IfAvailable()
        {
            // Arrange
            string baseCurrency = "USD";
            string targetCurrency = "EUR";
            decimal amount = 100;
            decimal cachedExchangeRate = 0.90m;
            string cacheKey = $"exchange_rate_{baseCurrency}_{targetCurrency}";

            byte[] cachedRateBytes = JsonSerializer.SerializeToUtf8Bytes(cachedExchangeRate);
            _mockCache.Setup(c => c.GetAsync(cacheKey, default))
                      .ReturnsAsync(cachedRateBytes);  // Simulate cache hit

            // Act
            var result = await _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount);

            // Assert
            result.Should().Be(amount * cachedExchangeRate);
            _mockApiClient.Verify(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency), Times.Never);
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldThrowException_WhenApiFails()
        {
            // Arrange
            string baseCurrency = "USD";
            string targetCurrency = "EUR";
            decimal amount = 100;
            string cacheKey = $"exchange_rate_{baseCurrency}_{targetCurrency}";

            _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[]?)null);  // Explicitly setting a nullable byte array

            _mockApiClient
                .Setup(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency))
                .ThrowsAsync(new Exception("API failure"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount));

            _mockApiClient.Verify(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency), Times.Once);
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldUpdateCache_WhenRateFetchedFromApi()
        {
            // Arrange
            string baseCurrency = "USD";
            string targetCurrency = "EUR";
            decimal amount = 100;
            decimal exchangeRate = 0.85m;
            string cacheKey = $"exchange_rate_{baseCurrency}_{targetCurrency}";

            _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync((byte[]?)null);

            _mockApiClient.Setup(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency))
                          .ReturnsAsync(exchangeRate);

            // Act
            await _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount);

            // Assert
            _mockCache.Verify(c => c.SetAsync(cacheKey, It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(), default), Times.Once);
        }

        [Theory]
        [InlineData(null, "EUR")]
        [InlineData("USD", null)]
        [InlineData("", "EUR")]
        [InlineData("USD", "")]
        public async Task ConvertCurrencyAsync_ShouldThrowException_WhenCurrencyCodeIsInvalid(string baseCurrency, string targetCurrency)
        {
            // Arrange
            decimal amount = 100;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount));
        }

        [Theory]
        [InlineData(-100)]
        [InlineData(0)]
        public async Task ConvertCurrencyAsync_ShouldThrowException_WhenAmountIsZeroOrNegative(decimal amount)
        {
            // Arrange
            string baseCurrency = "USD";
            string targetCurrency = "EUR";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount));
        }

        [Fact]
        public async Task ConvertCurrencyAsync_ShouldReturnSameAmount_WhenBaseAndTargetCurrencyAreSame()
        {
            // Arrange
            string currency = "USD";
            decimal amount = 100;

            // Act
            var result = await _currencyExchangeService.ConvertCurrencyAsync(currency, currency, amount);

            // Assert
            result.Should().Be(amount);
            _mockApiClient.Verify(api => api.GetCurrencyExchangeRateAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    
        [Fact]
        public async Task ConvertCurrencyAsync_ShouldHandleCacheDeserializationErrors_Gracefully()
        {
            // Arrange
            string baseCurrency = "USD";
            string targetCurrency = "EUR";
            decimal amount = 100;
            string cacheKey = $"exchange_rate_{baseCurrency}_{targetCurrency}";

            byte[] invalidCacheData = new byte[] { 1, 2, 3, 4, 5 }; // Corrupt cache data
            _mockCache.Setup(c => c.GetAsync(cacheKey, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(invalidCacheData);

            _mockApiClient.Setup(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency))
                          .ReturnsAsync(0.89m);

            // Act
            var result = await _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount);

            // Assert
            result.Should().Be(amount * 0.89m);
            _mockApiClient.Verify(api => api.GetCurrencyExchangeRateAsync(baseCurrency, targetCurrency), Times.Once);
        }

    }
}