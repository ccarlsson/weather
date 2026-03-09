using System.Text.Json.Serialization;

namespace Weather.Services.Dtos;

public sealed class OpenWeatherMapForecastDto
{
    [JsonPropertyName("city")]
    public OpenWeatherMapCityDto? City { get; set; }

    [JsonPropertyName("list")]
    public List<OpenWeatherMapForecastItemDto>? List { get; set; }
}

public sealed class OpenWeatherMapCityDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}

public sealed class OpenWeatherMapForecastItemDto
{
    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("main")]
    public OpenWeatherMapMainDto? Main { get; set; }

    [JsonPropertyName("weather")]
    public List<OpenWeatherMapWeatherDto>? Weather { get; set; }

    [JsonPropertyName("wind")]
    public OpenWeatherMapWindDto? Wind { get; set; }

    [JsonPropertyName("pop")]
    public double Pop { get; set; }
}
