using System.Text.Json.Serialization;

namespace Weather.Services.Dtos;

public sealed class OpenWeatherMapCurrentWeatherDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("dt")]
    public long Dt { get; set; }

    [JsonPropertyName("sys")]
    public OpenWeatherMapSysDto? Sys { get; set; }

    [JsonPropertyName("main")]
    public OpenWeatherMapMainDto? Main { get; set; }

    [JsonPropertyName("wind")]
    public OpenWeatherMapWindDto? Wind { get; set; }

    [JsonPropertyName("weather")]
    public List<OpenWeatherMapWeatherDto>? Weather { get; set; }
}

public sealed class OpenWeatherMapMainDto
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }

    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public sealed class OpenWeatherMapWindDto
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
}

public sealed class OpenWeatherMapWeatherDto
{
    [JsonPropertyName("main")]
    public string Main { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}

public sealed class OpenWeatherMapSysDto
{
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
}
