using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using CsvHelper.Configuration;

namespace CurrencyExchangeRates.Models.Responses
{
    public class ECBCsvResponseModel
    {
        public string CURRENCY { get; set; }
        public string CURRENCY_DENOM { get; set; }
        public DateTime TIME_PERIOD { get; set; }
        public decimal OBS_VALUE { get; set; }
    }

    public class ECBCsvResponseModelMap : ClassMap<ECBCsvResponseModel>
    {
        public ECBCsvResponseModelMap()
        {
            Map(m => m.CURRENCY).Name("CURRENCY");
            Map(m => m.CURRENCY_DENOM).Name("CURRENCY_DENOM");
            Map(m => m.TIME_PERIOD).Name("TIME_PERIOD");
            Map(m => m.OBS_VALUE).Name("OBS_VALUE");
        }
    }

}
