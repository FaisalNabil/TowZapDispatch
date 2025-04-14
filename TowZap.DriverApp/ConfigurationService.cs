using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TowZap.DriverApp
{
    public static class ConfigurationService
    {
        private static Dictionary<string, string> _config;

        public static async Task InitializeAsync()
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("config.json");
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();
            _config = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        public static string Get(string key)
        {
            return _config.TryGetValue(key, out var value) ? value : null;
        }
    }
}
