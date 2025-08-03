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
        private readonly ILogger<FMPService> logger;

        public FMPService(HttpClient httpClient, IConfiguration config, ILogger<FMPService> logger)
        {
            this.httpClient = httpClient;
            this.config = config;
            this.logger = logger;
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
                    logger.LogError("FMP API error {StatusCode}: {Error}", resp.StatusCode, err);
                    return null;
                }

                var json = await resp.Content.ReadAsStringAsync();

                var items = JsonConvert.DeserializeObject <FMPStock[]>(json);
                if (items == null)
                {
                    logger.LogWarning("Failed to deserialize FMP response for symbol: {Symbol}", symbol);
                    return null;
                }

                logger.LogInformation("Successfully retrieved stock data for symbol: {Symbol}", symbol);
                return items[0].ToStockFromFMP();

            } catch (Exception ex)
            {
                logger.LogError(ex, "Exception occurred while fetching stock data for symbol: {Symbol}", symbol);
                return null;
            }
        }
    }
}
