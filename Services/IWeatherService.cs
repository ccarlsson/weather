using Weather.Services.Models;

namespace Weather.Services;

public interface IWeatherService
{
    Task<CurrentWeather> GetCurrentWeatherAsync(
        WeatherRequest request,
        CancellationToken cancellationToken = default);

    Task<WeatherForecast> GetFiveDayForecastAsync(
        WeatherRequest request,
        CancellationToken cancellationToken = default);
}
