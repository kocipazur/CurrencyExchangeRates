using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Models.Responses
{
    public class ExchangeRatesResponse
    {
        public List<ExchangeRates> ExchangeRates { get; set; }
    }
    public class ExchangeRates
    {
        public string BaseCurrency { get; set; }
        public string DenominatedCurrency { get; set; }
        public List<RateByDay> ExchangeRatesByDays { get; set; }
    }
    public class RateByDay
    {
        public DateTime Day { get; set; }
        public decimal Rate { get; set; }
    }
}
