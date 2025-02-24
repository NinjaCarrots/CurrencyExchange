using CurrencyExchange.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyExchange.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyExchangeController : ControllerBase
    {
        private readonly ICurrencyExchangeService _currencyExchangeService;

        public CurrencyExchangeController(ICurrencyExchangeService currencyExchangeService)
        {
            _currencyExchangeService = currencyExchangeService;
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency(string baseCurrency, string targetCurrency, decimal amount)
        {
            var result = await _currencyExchangeService.ConvertCurrencyAsync(baseCurrency, targetCurrency, amount);
            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory()
        {
            var history = await _currencyExchangeService.GetConversionHistoryAsync();
            return Ok(history);
        }
    }
}
