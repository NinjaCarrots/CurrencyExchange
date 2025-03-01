using CurrencyExchange.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        if (string.IsNullOrWhiteSpace(baseCurrency))
            throw new ArgumentException("Base currency cannot be null or empty.", nameof(baseCurrency));

        if (string.IsNullOrWhiteSpace(targetCurrency))
            throw new ArgumentException("Target currency cannot be null or empty.", nameof(targetCurrency));

        string apiUrl = $"{_configuration["CurrencyExchangeApi:BaseUrl"]}?base={baseCurrency}&symbols={targetCurrency}&access_key={_configuration["CurrencyExchangeApi:ApiKey"]}";

        var response = await _httpClient.GetAsync(apiUrl);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Failed to fetch exchange rate. Status Code: {response.StatusCode}");

        var jsonResponse = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(jsonResponse))
            throw new Exception("Exchange API returned an empty response.");

        var data = JsonSerializer.Deserialize<ExchangeRateResponse>(
                    jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (data == null || !data.Success)
            throw new Exception("Invalid response from exchange API.");

        if (!data.Rates.TryGetValue(targetCurrency, out var rate))
            throw new Exception($"Exchange rate for {targetCurrency} not found.");

        return rate;
    }
}
public class ExchangeRateResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("base")]
    public string BaseCurrency { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("rates")]
    public Dictionary<string, decimal> Rates { get; set; }
}