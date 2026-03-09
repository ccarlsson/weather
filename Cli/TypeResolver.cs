using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace Weather.Cli;

public sealed class TypeResolver : ITypeResolver, IDisposable
{
    private readonly ServiceProvider _serviceProvider;

    public TypeResolver(ServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? Resolve(Type? type)
    {
        if (type is null)
        {
            return null;
        }

        return _serviceProvider.GetService(type) ?? ActivatorUtilities.GetServiceOrCreateInstance(_serviceProvider, type);
    }

    public void Dispose()
    {
        _serviceProvider.Dispose();
    }
}
