using CurrencyExchange.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchange.Infrastructure
{
    public class CurrencyExchangeDbContext : DbContext
    {
        public DbSet<CurrencyRate> currencyRates { get; set; }
        public DbSet<CurrencyExchangeHistory> currencyExchangeHistories { get; set; }

        public CurrencyExchangeDbContext(DbContextOptions<CurrencyExchangeDbContext> options) : base(options) { }
    }
}