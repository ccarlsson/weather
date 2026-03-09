using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Weather.Services.Dtos;
using Weather.Services.Models;

namespace Weather.Services;

public sealed class OpenWeatherMapService : IWeatherService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly OpenWeatherMapOptions _options;
    private readonly ILogger<OpenWeatherMapService> _logger;

    public OpenWeatherMapService(
        HttpClient httpClient,
        IOptions<OpenWeatherMapOptions> options,
        ILogger<OpenWeatherMapService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<CurrentWeather> GetCurrentWeatherAsync(
        WeatherRequest request,
        CancellationToken cancellationToken = default)
    {
        request.Validate();

        var route = BuildRoute("/data/2.5/weather", request);
        var dto = await SendAsync<OpenWeatherMapCurrentWeatherDto>(route, cancellationToken);

        var weather = dto.Weather?.FirstOrDefault();
        if (dto.Main is null || dto.Wind is null || dto.Sys is null || weather is null)
        {
            throw new WeatherServiceException(
                WeatherServiceErrorKind.InvalidResponse,
                "Current weather response is missing expected fields.");
        }

        return new CurrentWeather(
            dto.Name,
            dto.Sys.Country,
            DateTimeOffset.FromUnixTimeSeconds(dto.Dt),
            dto.Main.Temp,
            dto.Main.FeelsLike,
            dto.Main.Humidity,
            dto.Wind.Speed,
            weather.Main,
            weather.Description);
    }

    public async Task<WeatherForecast> GetFiveDayForecastAsync(
        WeatherRequest request,
        CancellationToken cancellationToken = default)
    {
        request.Validate();

        var route = BuildRoute("/data/2.5/forecast", request);
        var dto = await SendAsync<OpenWeatherMapForecastDto>(route, cancellationToken);

        if (dto.City is null || dto.List is null)
        {
            throw new WeatherServiceException(
                WeatherServiceErrorKind.InvalidResponse,
                "Forecast response is missing expected fields.");
        }

        var entries = new List<ForecastEntry>();

        foreach (var item in dto.List)
        {
            var weather = item.Weather?.FirstOrDefault();
            if (item.Main is null || item.Wind is null || weather is null)
            {
                continue;
            }

            entries.Add(new ForecastEntry(
                DateTimeOffset.FromUnixTimeSeconds(item.Dt),
                item.Main.Temp,
                item.Main.FeelsLike,
                item.Main.Humidity,
                item.Wind.Speed,
                item.Pop,
                weather.Main,
                weather.Description));
        }

        return new WeatherForecast(dto.City.Name, dto.City.Country, entries);
    }

    private async Task<TDto> SendAsync<TDto>(string route, CancellationToken cancellationToken)
        where TDto : class
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, route);

        try
        {
            _logger.LogInformation("Calling OpenWeatherMap endpoint: {Route}", route);

            using var response = await _httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            var payload = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw CreateServiceException(response.StatusCode, payload);
            }

            var dto = JsonSerializer.Deserialize<TDto>(payload, SerializerOptions);
            if (dto is null)
            {
                throw new WeatherServiceException(
                    WeatherServiceErrorKind.InvalidResponse,
                    "OpenWeatherMap returned an empty payload.");
            }

            return dto;
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            throw new WeatherServiceException(
                WeatherServiceErrorKind.UpstreamFailure,
                "OpenWeatherMap request timed out.");
        }
        catch (JsonException ex)
        {
            throw new WeatherServiceException(
                WeatherServiceErrorKind.InvalidResponse,
                "OpenWeatherMap returned malformed JSON.",
                innerException: ex);
        }
    }

    private WeatherServiceException CreateServiceException(HttpStatusCode statusCode, string payload)
    {
        var kind = statusCode switch
        {
            HttpStatusCode.BadRequest => WeatherServiceErrorKind.BadRequest,
            HttpStatusCode.Unauthorized => WeatherServiceErrorKind.Unauthorized,
            HttpStatusCode.NotFound => WeatherServiceErrorKind.NotFound,
            HttpStatusCode.TooManyRequests => WeatherServiceErrorKind.RateLimited,
            _ => WeatherServiceErrorKind.UpstreamFailure
        };

        var message = $"OpenWeatherMap request failed with status code {(int)statusCode} ({statusCode}).";

        _logger.LogWarning(
            "OpenWeatherMap request failed: StatusCode={StatusCode}, PayloadLength={PayloadLength}",
            statusCode,
            payload.Length);

        return new WeatherServiceException(kind, message, statusCode, payload);
    }

    private string BuildRoute(string endpoint, WeatherRequest request)
    {
        var queryParts = new List<string>
        {
            $"appid={Uri.EscapeDataString(_options.ApiKey)}",
            $"units={Uri.EscapeDataString(_options.Units)}",
            $"lang={Uri.EscapeDataString(_options.Language)}"
        };

        if (!string.IsNullOrWhiteSpace(request.CityName))
        {
            queryParts.Add($"q={Uri.EscapeDataString(request.CityName)}");
        }
        else
        {
            queryParts.Add($"lat={request.Latitude!.Value.ToString(CultureInfo.InvariantCulture)}");
            queryParts.Add($"lon={request.Longitude!.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        return $"{endpoint}?{string.Join("&", queryParts)}";
    }
}
