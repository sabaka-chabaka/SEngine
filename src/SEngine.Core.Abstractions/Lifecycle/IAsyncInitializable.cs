namespace SEngine.Core.Abstractions.Lifecycle;

public interface IAsyncInitializable
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}