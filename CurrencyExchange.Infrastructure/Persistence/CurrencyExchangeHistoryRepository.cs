using CurrencyExchange.Application.Interfaces;
using CurrencyExchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure.Persistence
{
    public class CurrencyExchangeHistoryRepository : ICurrencyExchangeHistoryRepository
    {
        private readonly CurrencyExchangeDbContext _context;

        public CurrencyExchangeHistoryRepository(CurrencyExchangeDbContext context)
        {
            _context = context;
        }

        public async Task SaveExchangeHistoryAsync(CurrencyExchangeHistory history)
        {
            await _context.currencyExchangeHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CurrencyExchangeHistory>> GetExchangeHistoryAsync()
        {
            return await _context.currencyExchangeHistories.OrderByDescending(h => h.TransactionDate).ToListAsync();
        }
    }
}
