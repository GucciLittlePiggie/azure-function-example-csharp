using System.Net.Http.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Function.Domain.Models;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

namespace Function.Domain.Services.HttpClients
{
    public class FinhubHttpClient
    {
        public HttpClient _client { get; }
        private readonly IConfiguration _configuration;

        public FinhubHttpClient(HttpClient client, IConfiguration configuration){
            _configuration = configuration;
            var finhubBaseUrl = _configuration.GetValue<string>("finhub-api-baseUrl");
            client.BaseAddress = new Uri(finhubBaseUrl);
            var apiToken = _configuration.GetValue<string>("finhub-api-token");
            client.DefaultRequestHeaders.Add("X-Finnhub-Token", apiToken);
            _client = client;
        }

        public async Task<FinhubStockData> GetStockDataForSymbolAsync(string symbol){
            var quotePath = $"quote?symbol={symbol}";

            var response = await _client.GetAsync(quotePath);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<FinhubStockData>();
        }
    }
}