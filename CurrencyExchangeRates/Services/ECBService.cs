using CurrencyExchangeRates.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CurrencyExchangeRates.Models.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;
using CsvHelper;
using System.Net;
using System.Globalization;
using System.Threading;

namespace CurrencyExchangeRates.Services
{
    public class ECBService : IExternalCurrencyRatesService
    {
        private readonly ILogger<ECBService> _logger;
        private readonly ECBServiceConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ECBService(ILogger<ECBService> logger, 
            ECBServiceConfiguration configuration,
            HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }
        public async Task<ExchangeRatesResponse> GetExchangeRatesAsync(Dictionary<string, string> currencies, 
            DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            List<ECBCsvResponseModel> records;

            string denominatedCurrency = currencies.Values.First();

            string query = $"{_configuration.Address}" +
                $"D.{string.Join('+', currencies.Keys)}" +
                $".{denominatedCurrency}.SP00.A?" +
                $"startPeriod={startDate:yyyy-MM-dd}&" +
                $"endPeriod={endDate:yyyy-MM-dd}";

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(query)
            };

            request.Headers.Add("Accept", _configuration.DataFormatHeader);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            records = GetAgregatedCurriencesResponse(await response.Content.ReadAsStringAsync());

            var ecbResponse = MapECBResposne(records);

            ecbResponse = FillSkippedDays(ecbResponse, startDate, endDate);

            return ecbResponse;

        }

        public double GetSingleExchangeRate(string baseCurrency, string denominatedCurrency, DateTime date)
        {
            throw new NotImplementedException();
        }

        private List<ECBCsvResponseModel> GetAgregatedCurriencesResponse(string data)
        {
            List<ECBCsvResponseModel> tempResponse = new List<ECBCsvResponseModel>();

            using TextReader reader = new StringReader(data);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Configuration.Delimiter = ",";
            csv.Configuration.RegisterClassMap<ECBCsvResponseModelMap>();
            tempResponse.AddRange(csv.GetRecords<ECBCsvResponseModel>().ToList());
            return tempResponse;
        }

        private ExchangeRatesResponse MapECBResposne(List<ECBCsvResponseModel> ecbCsbRows) {
            ExchangeRatesResponse preparedResponse = new ExchangeRatesResponse
            {
                ExchangeRates = new List<ExchangeRates>()
            };
            preparedResponse.ExchangeRates.AddRange((from r
                             in ecbCsbRows
                             group r by (r.CURRENCY, r.CURRENCY_DENOM)
                             into rs
                             select new ExchangeRates()
                             {
                                 BaseCurrency = rs.Key.CURRENCY,
                                 DenominatedCurrency = rs.Key.CURRENCY_DENOM,
                                 ExchangeRatesByDays = new List<RateByDay>(
                                     (from e in ecbCsbRows
                                      where e.CURRENCY == rs.Key.CURRENCY & e.CURRENCY_DENOM == rs.Key.CURRENCY_DENOM
                                      select new RateByDay()
                                      {
                                          Day = e.TIME_PERIOD.Date,
                                          Rate = e.OBS_VALUE
                                      })
                                 )
                             }).ToList());
            return preparedResponse;
        }

        private ExchangeRatesResponse FillSkippedDays(ExchangeRatesResponse ecbResponse, DateTime start, DateTime end)
        {
            List<DateTime> fullDatesSequence = Enumerable.Range(0, 1 + end.Subtract(start).Days)
                .Select(offset => start.AddDays(offset))
                .ToList();

            List<DateTime> responseDays = ecbResponse.ExchangeRates.First()
                .ExchangeRatesByDays.Select(x => x.Day)
                .ToList();

            List<DateTime> lackingDays = fullDatesSequence
                .Where(f => !responseDays.Any(d => d.Date == f.Date))
                .ToList();

            foreach (ExchangeRates exchangeRate in ecbResponse.ExchangeRates)
            {
                foreach (DateTime date in lackingDays)
                {
                    exchangeRate.ExchangeRatesByDays.Add(GetRateForEarlierDay(date, exchangeRate.ExchangeRatesByDays));
                }
                exchangeRate.ExchangeRatesByDays.Sort((x, y) => {
                    if (x.Day.Date < y.Day.Date)
                        return -1;
                    if (x.Day.Date > y.Day.Date)
                        return 1;
                    return 0;

                });
            }

            return ecbResponse;
        }

        private RateByDay GetRateForEarlierDay(DateTime date, List<RateByDay> exchangeRatesByDays)
        {
            try
            {
                return new RateByDay() { 
                    Day = date, 
                    Rate = exchangeRatesByDays.Where(rate => rate.Day.Date == date.AddDays(-1).Date)
                                                .First()
                                                .Rate 
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.InnerException}{Environment.NewLine}{ex.StackTrace}");
                throw new Exception("First day of asked sequence has to have avaible rate!");
            }
        }
    }
}