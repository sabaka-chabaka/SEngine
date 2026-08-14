using System.Diagnostics;
using SEngine.Core.Abstractions.Components;
using SEngine.Core.Abstractions.Time;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SEngine.Core.Application;

public sealed partial  class EngineLoopHostedService : BackgroundService
{
    private readonly IEngineComponent[] _components;
    private readonly ILogger<EngineLoopHostedService> _logger;

    public EngineLoopHostedService(
        IEnumerable<IEngineComponent> components,
        ILogger<EngineLoopHostedService> logger)
    {
        _components = [.. components.OrderBy(c => c.UpdateOrder)];
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        LogEngineLoopStarting(_logger, _components.Length);

        var stopwatch = Stopwatch.StartNew();
        var lastTime = stopwatch.Elapsed.TotalSeconds;
        long frame = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = stopwatch.Elapsed.TotalSeconds;
            var deltaTime = (float)(now - lastTime);
            lastTime = now;

            var gameTime = new GameTime(deltaTime, (float)now, frame++);

            foreach (var component in _components)
                component.Update(gameTime);

            await Task.Yield();
        }

        LogEngineLoopStopped(_logger, frame);
    }
    
    [LoggerMessage(
        EventId = 1, 
        Level = LogLevel.Information, 
        Message = "Engine loop starting with {Count} components")]
    private static partial void LogEngineLoopStarting(ILogger logger, int count);

    [LoggerMessage(
        EventId = 2, 
        Level = LogLevel.Information, 
        Message = "Engine loop stopped after {Frame} frames")]
    private static partial void LogEngineLoopStopped(ILogger logger, long frame);
}