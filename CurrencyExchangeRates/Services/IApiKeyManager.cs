using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Services
{
    public interface IApiKeyManager
    {
        public string GenerateKey(string clientId, string clientSecret);
        public bool IsKeyValid(string key);
    }
}
