using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CurrencyExchangeRates.Models.Requests;
using CurrencyExchangeRates.Models.Responses;
using CurrencyExchangeRates.Services;
using System.Text.Json;
using CurrencyExchangeRates.Exceptions;
using CurrencyExchangeRates.Attributes;
using System.Threading;

namespace CurrencyExchangeRates.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ApiKey]
    [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
    public class GetExchangeRatesController : ControllerBase
    {

        private readonly ILogger<GetExchangeRatesController> _logger;
        private readonly IExternalCurrencyRatesService _currencyRatesService;

        public GetExchangeRatesController(ILogger<GetExchangeRatesController> logger,
            IExternalCurrencyRatesService currencyRatesService
            )
        {
            _logger = logger;
            _currencyRatesService = currencyRatesService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]ExchangeRatesRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.currencyCodes.Values.Where(x => x != "EUR").Count() > 0)
                    return BadRequest("Denominated currency has to be EUR!");

                DateTime endDate = request.endDate.GetValueOrDefault(request.startDate);

                if (request.startDate > DateTime.Now || request.endDate > DateTime.Now
                    || request.startDate > request.endDate)
                    throw new InvalidDateTime("Startdate has to be earlier or equal to enddate " +
                        "and earlier or equal to current date!");

                var response = await _currencyRatesService.GetExchangeRatesAsync(request.currencyCodes, 
                        request.startDate, endDate, cancellationToken);

                return Ok(response);
            }
            catch (InvalidDateTime ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
