using api.DTOs.Stock;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Newtonsoft.Json;

namespace api.Service
{
    public class FMPService : IFMPService
    {
        private readonly HttpClient httpClient;
        private readonly IConfiguration config;

        public FMPService(HttpClient httpClient, IConfiguration config)
        {
            this.httpClient = httpClient;
            this.config = config;
        }
        public async Task<Stock?> FindStockBySymbolAsync(string symbol)
        {
            try
            {
                var apiKey = RequiredConfig.Get(config, "FMPKey");
                var url =
                    $"https://financialmodelingprep.com/api/v3/profile/{symbol}?apikey={apiKey}";
                var resp = await httpClient.GetAsync(url);

                if (!resp.IsSuccessStatusCode)
                {
                    var err = await resp.Content.ReadAsStringAsync();
                    Console.WriteLine($"FMP error {resp.StatusCode}: {err}");
                    return null;
                }

                var json = await resp.Content.ReadAsStringAsync();

                var items = JsonConvert.DeserializeObject <FMPStock[]>(json);
                if (items == null)
                    return null;

                return items[0].ToStockFromFMP();

            } catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }
    }
}
