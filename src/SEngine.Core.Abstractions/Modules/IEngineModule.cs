using Microsoft.Extensions.DependencyInjection;

namespace SEngine.Core.Abstractions.Modules;

public interface IEngineModule
{
    void RegisterServices(IServiceCollection services);
}