using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CurrencyExchangeRates.Extensions;
using CurrencyExchangeRates.Models;
using CurrencyExchangeRates.Models.Configuration;
using CurrencyExchangeRates.Services;
using CurrencyExchangeRates.Utils;
using CurrencyExchangeRates.Middlewares;
using Microsoft.AspNetCore.Http;

namespace CurrencyExchangeRates
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddConfiguration<ECBServiceConfiguration>(Configuration, "ECBService");
            services.AddConfiguration<DatabaseConfiguration>(Configuration, "Database");
            services.AddConfiguration<PersistentDBCacheConfiguration>(Configuration, "DBCache");
            services.AddSingleton<ICacheManager, DBResponseCache>();
            services.AddTransient<IExternalCurrencyRatesService, ECBService>();
            services.AddTransient<IApiKeyManager, DBApiKeyManager>();
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
                });
            services.AddResponseCaching();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<LoggingMiddleware>();

            app.UseResponseCaching();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
