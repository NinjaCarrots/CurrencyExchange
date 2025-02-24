using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.Persistence
{
    public class CurrencyRateRepository : ICurrencyRateRepository
    {
        private readonly CurrencyExchangeDbContext _context;

        public CurrencyRateRepository(CurrencyExchangeDbContext context)
        {
            _context = context;
        }

        public async Task SaveRateAsync(CurrencyRate rate)
        {
            await _context.currencyRates.AddAsync(rate);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CurrencyRate>> GetHistoricalRatesAsync()
        {
            return await _context.currencyRates.OrderByDescending(r => r.Timestamp).ToListAsync();
        }
    }
}
