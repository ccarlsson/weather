namespace Weather.Services;

public sealed record WeatherRequest
{
    public string? CityName { get; init; }

    public double? Latitude { get; init; }

    public double? Longitude { get; init; }

    public static WeatherRequest ForCity(string cityName)
    {
        if (string.IsNullOrWhiteSpace(cityName))
        {
            throw new ArgumentException("City name is required.", nameof(cityName));
        }

        return new WeatherRequest { CityName = cityName.Trim() };
    }

    public static WeatherRequest ForCoordinates(double latitude, double longitude)
    {
        return new WeatherRequest { Latitude = latitude, Longitude = longitude };
    }

    public void Validate()
    {
        var hasCity = !string.IsNullOrWhiteSpace(CityName);
        var hasCoordinates = Latitude.HasValue && Longitude.HasValue;

        if (hasCity == hasCoordinates)
        {
            throw new ArgumentException(
                "Provide either a city name or both coordinates.",
                nameof(WeatherRequest));
        }

        if (hasCoordinates)
        {
            if (Latitude is < -90 or > 90)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(Latitude),
                    "Latitude must be between -90 and 90.");
            }

            if (Longitude is < -180 or > 180)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(Longitude),
                    "Longitude must be between -180 and 180.");
            }
        }
    }
}
