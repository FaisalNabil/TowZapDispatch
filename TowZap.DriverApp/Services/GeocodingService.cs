using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TowZap.DriverApp.Services
{
    public class GeocodingService
    {
        private readonly HttpClient _http;

        public GeocodingService()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("User-Agent", "TowZapApp");
        }
        public async Task<string> ReverseGeocodeAsync(double lat, double lon)
        {
            try
            {
                var url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat}&lon={lon}";
                var response = await _http.GetStringAsync(url);
                using var json = JsonDocument.Parse(response);
                if (json.RootElement.TryGetProperty("display_name", out var name))
                    return name.GetString() ?? $"{lat}, {lon}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Reverse geocode failed: {ex.Message}");
            }

            return $"{lat}, {lon}";
        }
    }
}
