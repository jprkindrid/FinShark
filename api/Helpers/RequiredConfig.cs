using Microsoft.Extensions.Configuration;

namespace api.Helpers
{
    public static class RequiredConfig
    {
        public static string Get(IConfiguration config, string key)
        {
            var value = config[key];
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException($"Configuration key '{key}' is missing or empty.");
            }
            return value;
        }
    }
}