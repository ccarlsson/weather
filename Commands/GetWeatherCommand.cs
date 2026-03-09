using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;
using Weather.Services;

namespace Weather.Commands;

public sealed class GetWeatherCommand : AsyncCommand<GetWeatherSettings>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<GetWeatherCommand> _logger;

    public GetWeatherCommand(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<GetWeatherCommand> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public override async Task<int> ExecuteAsync(
        CommandContext context,
        GetWeatherSettings settings,
        CancellationToken cancellationToken)
    {
        try
        {
            ValidateSettings(settings);
            var weatherService = _serviceProvider.GetRequiredService<IWeatherService>();
            var request = BuildRequest(settings);
            var currentWeather = await weatherService.GetCurrentWeatherAsync(request, cancellationToken);
            var forecast = await weatherService.GetFiveDayForecastAsync(request, cancellationToken);

            AnsiConsole.MarkupLine(
                $"[bold]Current weather for {Markup.Escape(currentWeather.LocationName)}, {Markup.Escape(currentWeather.CountryCode)}[/]");
            AnsiConsole.MarkupLine(
                $"[green]{Markup.Escape(currentWeather.Summary)}[/] ({Markup.Escape(currentWeather.Description)}) | " +
                $"Temp {currentWeather.Temperature:0.#} | Feels Like {currentWeather.FeelsLike:0.#} | " +
                $"Humidity {currentWeather.Humidity}% | Wind {currentWeather.WindSpeed:0.#}");

            var table = new Table().Border(TableBorder.Rounded).Title("[bold]Forecast (UTC)[/]");
            table.AddColumn("Time");
            table.AddColumn("Summary");
            table.AddColumn("Temp");
            table.AddColumn("Precip");

            foreach (var entry in forecast.Entries.Take(settings.Take))
            {
                table.AddRow(
                    entry.ForecastAtUtc.ToString("yyyy-MM-dd HH:mm"),
                    Markup.Escape(entry.Summary),
                    entry.Temperature.ToString("0.#"),
                    $"{entry.PrecipitationProbability * 100:0}%");
            }

            AnsiConsole.Write(table);
            return 0;
        }
        catch (ArgumentException ex)
        {
            AnsiConsole.MarkupLine($"[red]{Markup.Escape(ex.Message)}[/]");
            return 1;
        }
        catch (OptionsValidationException ex)
        {
            _logger.LogWarning("OpenWeatherMap configuration is invalid: {Message}", ex.Message);
            AnsiConsole.MarkupLine(
                "[red]OpenWeatherMap configuration is invalid.[/] " +
                "Set [yellow]OpenWeatherMap:ApiKey[/] (or env var [yellow]OpenWeatherMap__ApiKey[/]) and try again.");
            return 2;
        }
        catch (WeatherServiceException ex)
        {
            _logger.LogWarning(
                "Weather call failed with kind {ErrorKind}, status {StatusCode}, and message: {Message}",
                ex.ErrorKind,
                ex.StatusCode,
                ex.Message);

            AnsiConsole.MarkupLine($"[red]Weather request failed:[/] {Markup.Escape(ex.Message)}");
            return 3;
        }
    }

    private WeatherRequest BuildRequest(GetWeatherSettings settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.City))
        {
            return WeatherRequest.ForCity(settings.City);
        }

        if (settings.Latitude.HasValue && settings.Longitude.HasValue)
        {
            return WeatherRequest.ForCoordinates(settings.Latitude.Value, settings.Longitude.Value);
        }

        var defaultCity = _configuration["WeatherSample:DefaultCity"] ?? "Stockholm";
        return WeatherRequest.ForCity(defaultCity);
    }

    private static void ValidateSettings(GetWeatherSettings settings)
    {
        var hasCity = !string.IsNullOrWhiteSpace(settings.City);
        var hasAnyCoordinate = settings.Latitude.HasValue || settings.Longitude.HasValue;

        if (hasCity && hasAnyCoordinate)
        {
            throw new ArgumentException("Provide either CITY or --lat/--lon, not both.");
        }

        if (settings.Latitude.HasValue ^ settings.Longitude.HasValue)
        {
            throw new ArgumentException("Both --lat and --lon are required when using coordinates.");
        }

        if (settings.Take < 1 || settings.Take > 40)
        {
            throw new ArgumentException("--take must be between 1 and 40.");
        }
    }
}
