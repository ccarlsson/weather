using Spectre.Console.Cli;

namespace Weather.Commands;

public sealed class GetWeatherSettings : CommandSettings
{
    [CommandArgument(0, "[CITY]")]
    public string? City { get; init; }

    [CommandOption("--lat <LATITUDE>")]
    public double? Latitude { get; init; }

    [CommandOption("--lon <LONGITUDE>")]
    public double? Longitude { get; init; }

    [CommandOption("--take <COUNT>")]
    public int Take { get; init; } = 5;
}
