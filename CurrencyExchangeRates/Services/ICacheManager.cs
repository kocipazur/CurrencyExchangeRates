using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyExchangeRates.Models.Responses;

namespace CurrencyExchangeRates.Services
{
    public interface ICacheManager
    {
        public bool HasValidResponse(string key);
        public ExchangeRatesResponse GetCachedResponse(string key);
        public void CacheResponse(string key, ExchangeRatesResponse response);
    }
}
