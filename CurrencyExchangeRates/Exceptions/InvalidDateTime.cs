using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Exceptions
{
    public class InvalidDateTime : Exception
    {
        public InvalidDateTime() { }
        public InvalidDateTime(string message) :base(message) { }
        public InvalidDateTime(string message, Exception inner) : base(message, inner) { }
    }
}
