using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CurrencyExchangeRates.Services;

namespace CurrencyExchangeRates.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GenerateKeyController : ControllerBase
    {
        private readonly ILogger<GenerateKeyController> _logger;
        private readonly IApiKeyManager _apiKeyManager;
        public GenerateKeyController(ILogger<GenerateKeyController> logger, IApiKeyManager apiKeyManager)
        {
            _logger = logger;
            _apiKeyManager = apiKeyManager;
        }
        [HttpGet]
        public IActionResult Get([FromHeader] string clientId,[FromHeader] string secret)
        {
            try
            {
                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(secret))
                    return BadRequest("clientId or secret parameters not provided!");

                string apiKey =  _apiKeyManager.GenerateKey(clientId, secret);

                Response.Headers.Add("ApiKey", apiKey);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
