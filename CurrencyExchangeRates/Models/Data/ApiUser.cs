using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Models.Data
{
    public class ApiUser
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string ApiKey { get; set; }
    }
}
