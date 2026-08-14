using SEngine.Core.Abstractions.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace SEngine.Core.Hosting;

public static class EngineModuleExtensions
{
    public static IServiceCollection AddEngineModule<TModule>(this IServiceCollection services)
        where TModule : IEngineModule, new()
    {
        var module = new TModule();
        module.RegisterServices(services);
        return services;
    }

    public static IServiceCollection AddEngineModule(this IServiceCollection services, IEngineModule module)
    {
        module.RegisterServices(services);
        return services;
    }
}