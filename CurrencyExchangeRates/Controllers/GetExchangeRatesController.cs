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

namespace CurrencyExchangeRates.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GetExchangeRatesController : ControllerBase
    {

        private readonly ILogger<GetExchangeRatesController> _logger;
        private readonly IExternalCurrencyRatesService _currencyRatesService;

        public GetExchangeRatesController(ILogger<GetExchangeRatesController> logger, IExternalCurrencyRatesService currencyRatesService)
        {
            _logger = logger;
            _currencyRatesService = currencyRatesService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromBody]ExchangeRatesRequest request)
        {
            try
            {
                return await Task.Run(() =>
                {
                    if (request.currencyCodes.Values.Where(x => x != "EUR").Count() > 0)
                        throw new Exception("Denominated currency has to be EUR!");

                    DateTime endDate = request.endDate.GetValueOrDefault(request.startDate);

                    if (request.startDate > DateTime.Now || request.endDate > DateTime.Now
                        || request.startDate > request.endDate)
                        throw new WrongDateTime("Startdate has to be earlier or equal to enddate and earlier or equal to current date!");

                    var response = _currencyRatesService.GetExchangeRates(request.currencyCodes, request.startDate, endDate);

                    return Ok(response);
                });
            }
            catch (WrongDateTime ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
