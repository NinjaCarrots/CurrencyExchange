using CurrencyExchange.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

public class CurrencyExchangeApiClient : ICurrencyExchangeApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public CurrencyExchangeApiClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<decimal> GetCurrencyExchangeRateAsync(string baseCurrency, string targetCurrency)
    {
        string apiUrl = $"{_configuration["ExchangeApi:BaseUrl"]}?base={baseCurrency}&symbols={targetCurrency}&apikey={_configuration["ExchangeApi:ApiKey"]}";

        var response = await _httpClient.GetAsync(apiUrl);
        if (!response.IsSuccessStatusCode)
            throw new Exception("Failed to fetch exchange rate");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<ExchangeRateResponse>(jsonResponse);

        return data?.Rates[targetCurrency] ?? throw new Exception("Invalid response from exchange API");
    }
}

public class ExchangeRateResponse
{
    public Dictionary<string, decimal> Rates { get; set; }
}