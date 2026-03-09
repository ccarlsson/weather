namespace Weather.Services.Models;

public sealed record WeatherForecast(
    string LocationName,
    string CountryCode,
    IReadOnlyList<ForecastEntry> Entries);

public sealed record ForecastEntry(
    DateTimeOffset ForecastAtUtc,
    double Temperature,
    double FeelsLike,
    int Humidity,
    double WindSpeed,
    double PrecipitationProbability,
    string Summary,
    string Description);
