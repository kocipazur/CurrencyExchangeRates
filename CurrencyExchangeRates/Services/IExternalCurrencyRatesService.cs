﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyExchangeRates.Models.Responses;

namespace CurrencyExchangeRates.Services
{
    public interface IExternalCurrencyRatesService
    {
        public ExchangeRatesResponse GetExchangeRates(Dictionary<string, string> currencies, DateTime startDate, DateTime endDate);
        public double GetSingleExchangeRate(string baseCurrency, string denominatedCurrency, DateTime date);
    }
}