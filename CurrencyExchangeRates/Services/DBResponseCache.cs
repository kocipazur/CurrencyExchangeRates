using CurrencyExchangeRates.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MonkeyCache.LiteDB;
using System.Reflection;
using Microsoft.Extensions.Logging;
using CurrencyExchangeRates.Models.Configuration;

namespace CurrencyExchangeRates.Services
{
    public class DBResponseCache : ICacheManager
    {
        private readonly ILogger<DBResponseCache> _logger;
        private readonly PersistentDBCacheConfiguration _configuration;
        public DBResponseCache(ILogger<DBResponseCache> logger, PersistentDBCacheConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            Barrel.ApplicationId = "DBCache";
        }
        public void CacheResponse(string key, ExchangeRatesResponse response)
        {
            _logger.LogInformation("Write response to dbcache.");
            Barrel.Current.Add(key: key, data: response, expireIn: TimeSpan.FromDays(_configuration.CacheTTLinDays));
        }
        public ExchangeRatesResponse GetCachedResponse(string cacheKey)
        {
            _logger.LogInformation("Response read from dbcache.");
            return Barrel.Current.Get<ExchangeRatesResponse>(key: cacheKey);
        }
        public bool HasValidResponse(string cacheKey)
        {
            return Barrel.Current.Exists(cacheKey) && !Barrel.Current.IsExpired(cacheKey);
        }
    }
}
