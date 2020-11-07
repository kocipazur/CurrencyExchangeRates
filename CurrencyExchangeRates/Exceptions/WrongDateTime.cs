using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CurrencyExchangeRates.Exceptions
{
    public class WrongDateTime : Exception
    {
        public WrongDateTime() { }
        public WrongDateTime(string message) :base(message) { }
        public WrongDateTime(string message, Exception inner) : base(message, inner) { }
    }
}
