# weather

A .NET 10 CLI app that fetches current weather and 5-day forecast data from OpenWeatherMap.

## Features

- Spectre.Console.Cli command interface
- OpenWeatherMap integration through a typed `HttpClient`
- Service-oriented design (`IWeatherService`) for reuse
- Validation for configuration and request inputs
- File-based logging (warnings and errors)

## Tech Stack

- .NET 10
- Spectre.Console.Cli
- Microsoft.Extensions.Hosting / DI / Options
- Serilog (rolling file sink)

## Project Structure

- `Program.cs` - CLI bootstrap
- `Commands/` - command classes and settings
- `Services/` - service contracts and OpenWeatherMap client
- `appsettings.json` - non-secret defaults
- `logs/` - runtime log files

## Prerequisites

- .NET SDK 10.x
- OpenWeatherMap API key

## Setup

1. Clone the repository.
2. Set your API key as an environment variable:

```bash
export OpenWeatherMap__ApiKey="YOUR_API_KEY"
```

3. (Optional) Override defaults in `appsettings.json`.

## Build

```bash
dotnet build weather.csproj
```

## Run

```bash
dotnet run --project weather.csproj -- --help
```

By city:

```bash
dotnet run --project weather.csproj -- Stockholm
```

By coordinates:

```bash
dotnet run --project weather.csproj -- --lat 59.33 --lon 18.06
```

Limit forecast rows:

```bash
dotnet run --project weather.csproj -- Stockholm --take 8
```

## Configuration

Configuration section: `OpenWeatherMap`

- `ApiKey` (required)
- `BaseUrl` (default: `https://api.openweathermap.org`)
- `Units` (`metric`, `imperial`, `standard`)
- `Language` (default: `en`)
- `TimeoutSeconds` (default: `10`)

Sample city fallback section: `WeatherSample:DefaultCity`

## Logging

- Logs are written to `logs/weather-YYYYMMDD.log`
- Log level is warning and above
- Console logging providers are disabled to keep CLI output clean

## Exit Codes

- `0` success
- `1` invalid CLI arguments
- `2` invalid configuration
- `3` weather service failure

## Notes for GitHub

- Do not commit API keys.
- Keep secrets in environment variables or local, ignored config files.
