using System.Text.Json.Serialization;
using TowZap.DriverApp.Models;


namespace TowZap.DriverApp
{
    [JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(LoginRequest))]
    [JsonSerializable(typeof(LoginResponse))]
    [JsonSerializable(typeof(JobResponse))]
    public partial class AppJsonContext : JsonSerializerContext
    {
    }
}
