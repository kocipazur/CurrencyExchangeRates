# Currency Exchange Rates

### Proxy microservice for European Central Bank exchange rates against EUR

- More user-friendly request and response

- Short term response data caching

- Implementation of Api Key authentication and generation (using clientId and secret)

  - ApiKey will generate only for user/secret already existing in db (LiteDB)
  
    normally it should be populated by independent mechanism
  
  - For test purposes you can use header with existing key:
    "ApiKey": " C17DA4283D388C0835FB77D73C6AF0D15A822A08"
  
  ##### Example data query:
  
  ```http
  https://{server}/api/GetExchangeRates?CurrencyCodes[0].key=GBP&CurrencyCodes[0].value=EUR&CurrencyCodes[1].key=USD&CurrencyCodes[1].value=EUR&startdate=2020-04-27&endDate=2020-05-10
  ```
  
  Translates to:
  Give me all exchange rates of GBP and USD against EUR from 2020-04-27 to 2020-05-10
  
  Returns:
  
  ```json
  {
      "exchangeRates": [
          {
              "baseCurrency": "GBP",
              "denominatedCurrency": "EUR",
              "exchangeRatesByDays": [
                  {
                      "day": "2020-04-26",
                      "rate": 0.87263
                  },
                  {
                      "day": "2020-04-27",
                      "rate": 0.87078
                  },
  				...
              ]
          },
          {
              "baseCurrency": "USD",
              "denominatedCurrency": "EUR",
              "exchangeRatesByDays": [
                  {
                      "day": "2020-04-26",
                      "rate": 1.0852
                  },
                  {
                      "day": "2020-04-27",
                      "rate": 1.0877
                  },
  				...
              ]
          }
      ]
  }
  ```
  
  ##### ApiKey generation:
  
  ```http
  https://{server}/api/GenerateKey
  ```
  
  with authentication data in headers:
  	"clientId" : {client id string}
  	"secret" : {secret string}
  
  ApiKey is returned in response header "ApiKey"