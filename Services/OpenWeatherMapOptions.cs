using System.ComponentModel.DataAnnotations;

namespace Weather.Services;

public sealed class OpenWeatherMapOptions
{
    public const string SectionName = "OpenWeatherMap";

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string BaseUrl { get; set; } = "https://api.openweathermap.org";

    [Required]
    public string Units { get; set; } = "metric";

    public string Language { get; set; } = "en";

    [Range(1, 120)]
    public int TimeoutSeconds { get; set; } = 10;
}
