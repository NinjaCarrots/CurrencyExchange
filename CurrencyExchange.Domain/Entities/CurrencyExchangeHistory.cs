using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyExchange.Domain.Entities
{
    public class CurrencyExchangeHistory
    {
        public int Id { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ConvertedAmount { get; set; }
        public DateTime TransactionDate { get; set; }

        public CurrencyExchangeHistory(string baseCurrency, string targetCurrency, decimal exchangeRate, decimal convertedAmount)
        {
            BaseCurrency = baseCurrency;
            TargetCurrency = targetCurrency;
            ExchangeRate = exchangeRate;
            ConvertedAmount = convertedAmount;
            TransactionDate = DateTime.UtcNow;
        }
    }
}