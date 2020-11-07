using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Services
{
    public interface IClientVerifier
    {
        public bool IsUserValid(string clientId, string clientSecret);
    }
}
