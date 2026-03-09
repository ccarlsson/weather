namespace Weather.Services.Models;

public sealed record CurrentWeather(
    string LocationName,
    string CountryCode,
    DateTimeOffset ObservedAtUtc,
    double Temperature,
    double FeelsLike,
    int Humidity,
    double WindSpeed,
    string Summary,
    string Description);
