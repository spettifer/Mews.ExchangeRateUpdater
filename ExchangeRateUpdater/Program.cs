using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRateUpdater.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace ExchangeRateUpdater;

public static class Program
{
    private static readonly IEnumerable<Currency> _currencies = new[]
    {
        new Currency("USD"),
        new Currency("EUR"),
        new Currency("CZK"),
        new Currency("JPY"),
        new Currency("KES"),
        new Currency("RUB"),
        new Currency("THB"),
        new Currency("TRY"),
        new Currency("XYZ")
    };

    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appSettings.json")
            .Build();

        // The following adds the HttpClientFactory  and registers an http client for the ExchangeRateProvider, and registers the provider as a transient to the service collection.
        var services = new ServiceCollection()
            .AddHttpClient<IExchangeRateProvider, ExchangeRateProvider>()
            .AddPolicyHandler(GetRetryPolicy())
            .Services;

        // Make config injectable
        services.Configure<ApiSettings>(config.GetSection("apiSettings"));

        var serviceProvider = services.BuildServiceProvider();

        var provider = serviceProvider.GetService<IExchangeRateProvider>();

        try
        {
            var validDate = false;
            var parsedDate = DateTime.MinValue;
            
            Console.WriteLine("Input date to get exchange rates for (yyyy-MM-dd):");
                
            while (!validDate)
            {
                var date = Console.ReadLine();
                validDate = DateTime.TryParse(date, out parsedDate);
                        
                if(!validDate)
                {
                    Console.WriteLine($"'{date}' is not a valid date. Please try again.");
                }
            }
            var rates = (await provider.GetExchangeRates(_currencies, parsedDate)).ToList();

            Console.WriteLine($"Successfully retrieved {rates.Count()} exchange rates:");
            foreach (var rate in rates)
            {
                Console.WriteLine(rate.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Could not retrieve exchange rates: '{e.Message}'.");
        }

        Console.ReadLine();
    }
    
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        // Could add a timeout policy here too, or a circuit breaker or any other useful policy, but for now we'll just retry a few times in case of transient errors.
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(4, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}