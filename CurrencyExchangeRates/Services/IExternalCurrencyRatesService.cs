using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CurrencyExchangeRates.Models.Responses;

namespace CurrencyExchangeRates.Services
{
    public interface IExternalCurrencyRatesService
    {
        public Task<ExchangeRatesResponse> GetExchangeRatesAsync(Dictionary<string, string> currencies, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}
