namespace SEngine.Core.Abstractions.Time;

public readonly record struct GameTime(float DeltaTime, float TotalTime, long FrameCount);