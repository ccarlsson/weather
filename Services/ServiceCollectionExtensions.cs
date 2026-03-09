using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Weather.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenWeatherMap(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<OpenWeatherMapOptions>()
            .Bind(configuration.GetSection(OpenWeatherMapOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(
                options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
                "OpenWeatherMap BaseUrl must be a valid absolute URI.")
            .Validate(
                options => options.Units is "metric" or "imperial" or "standard",
                "OpenWeatherMap Units must be one of: metric, imperial, standard.")
            .ValidateOnStart();

        services.AddHttpClient<IWeatherService, OpenWeatherMapService>((serviceProvider, httpClient) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<OpenWeatherMapOptions>>()
                .Value;

            httpClient.BaseAddress = new Uri(options.BaseUrl, UriKind.Absolute);
            httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        });

        return services;
    }
}
