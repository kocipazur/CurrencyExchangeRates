using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Models.Requests
{
    public class ExchangeRatesRequest
    {
        public Dictionary<string, string> currencyCodes { get; set; }
        public DateTime startDate { get; set; }
        public DateTime? endDate { get; set; }
        public string apiKey { get; set; }
    }
}
