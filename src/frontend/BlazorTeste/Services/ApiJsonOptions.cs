using System.Text.Json;
using System.Text.Json.Serialization;

namespace BlazorTeste.Services;

internal static class ApiJsonOptions
{
    internal static readonly JsonSerializerOptions Default = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
}
