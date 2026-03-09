using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Spectre.Console.Cli;
using Weather.Cli;
using Weather.Commands;
using Weather.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddSerilog((_, loggerConfiguration) => loggerConfiguration
    .MinimumLevel.Warning()
    .Enrich.FromLogContext()
    .WriteTo.File(
        path: "logs/weather-.log",
        restrictedToMinimumLevel: LogEventLevel.Warning,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        shared: true));

builder.Services.AddOpenWeatherMap(builder.Configuration);
var app = new CommandApp<GetWeatherCommand>(new TypeRegistrar(builder.Services));
app.Configure(config =>
{
    config.SetApplicationName("weather");
    config
        .AddExample(new[] { "Stockholm" })
        .AddExample(new[] { "--lat", "59.33", "--lon", "18.06" });
});

return app.Run(args);
