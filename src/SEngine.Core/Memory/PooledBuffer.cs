using System.Buffers;
using System.Runtime.CompilerServices;

namespace SEngine.Core.Memory;

public readonly struct PooledBuffer<T> : IDisposable
{
    private readonly T[] _array;
    private readonly int _length;

    private PooledBuffer(T[] array, int length)
    {
        _array = array;
        _length = length;
    }

#pragma warning disable CA1000
    public static PooledBuffer<T> Rent(int minimumLength)
#pragma warning restore CA1000
    {
        var array = ArrayPool<T>.Shared.Rent(minimumLength);
        return new PooledBuffer<T>(array, minimumLength);
    }

    public Span<T> Span => _array.AsSpan(0, _length);
    public Memory<T> Memory => _array.AsMemory(0, _length);
    public T[] Array => _array;

    public void Dispose()
    {
        ArrayPool<T>.Shared.Return(_array, clearArray: RuntimeHelpers.IsReferenceOrContainsReferences<T>());
    }
}