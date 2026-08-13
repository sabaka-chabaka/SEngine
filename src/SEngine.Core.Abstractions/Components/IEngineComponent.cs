using SEngine.Core.Abstractions.Time;

namespace SEngine.Core.Abstractions.Components;

public interface IEngineComponent
{
    int UpdateOrder => 0;
    void Update(GameTime time);
}