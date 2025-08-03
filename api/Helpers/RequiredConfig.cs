using Microsoft.Extensions.Configuration;

namespace api.Helpers
{
    public static class RequiredConfig
    {
        public static string Get(IConfiguration config, string key)
        {
            return config[key] ?? throw new InvalidOperationException($"{key} is not configured");
        }
    }
}