using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SEngine.Core.Application;

namespace SEngine.Core.Hosting;

public class EngineHostBuilder(HostApplicationBuilder builder)
{
    public IServiceCollection Services => builder.Services;
    public IConfigurationManager Configuration => builder.Configuration;
    public ILoggingBuilder Logging => builder.Logging;

    public static EngineHostBuilder Create(string[]? args = null)
    {
        var builder = Host.CreateApplicationBuilder(args ?? []);

        builder.Services.AddHostedService<EngineLoopHostedService>();
        
        return new EngineHostBuilder(builder);
    }
    
    public IHost Build() => builder.Build();
}