using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;
using CurrencyExchangeRates.Models.Data;
using CurrencyExchangeRates.Exceptions;
using Microsoft.Extensions.Logging;
using CurrencyExchangeRates.Models.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace CurrencyExchangeRates.Services
{
    public class DBApiKeyManager : IApiKeyManager
    {
        private readonly ILogger<DBApiKeyManager> _logger;
        private readonly DatabaseConfiguration _configuration;
        public DBApiKeyManager(ILogger<DBApiKeyManager> logger, DatabaseConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public string GenerateKey(string clientId, string clientSecret)
        {
            using LiteDatabase db = new LiteDatabase(_configuration.DatabaseFileName);
            var apiUsers = db.GetCollection<ApiUser>("apiUsers");

            var resultUser = apiUsers.Find(x => x.ClientId == clientId & x.Secret == clientSecret).FirstOrDefault();

            //apiUsers.Insert(new ApiUser() { ClientId = clientId, Secret = clientSecret }); //test user

            if (resultUser is null)
                throw new InvalidCredentials("Invalid clientId/clientSecret pair!");

            string apiKey = resultUser.ApiKey is null ? GenerateKeyForUser(resultUser) : resultUser.ApiKey;

            resultUser.ApiKey = apiKey;
            apiUsers.Update(resultUser);

            return apiKey;
        }

        public bool IsKeyValid(string key)
        {
            using LiteDatabase db = new LiteDatabase(_configuration.DatabaseFileName);
            var apiUsers = db.GetCollection<ApiUser>("apiUsers");

            return apiUsers.Exists(x => x.ApiKey == key);
        }

        private string GenerateKeyForUser(ApiUser resultUser)
        {
            using SHA1Managed sha = new SHA1Managed();
            var hash = sha.ComputeHash(
                Encoding.UTF8.GetBytes($"{resultUser.ClientId}{resultUser.Secret}{DateTime.UnixEpoch}"
                ));
            return string.Concat(hash.Select(b => b.ToString("X2")));
        }
    }
}
