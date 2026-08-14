using System.Runtime.InteropServices;

namespace SEngine.Core.Memory;

public sealed unsafe class ArenaAllocator : IDisposable
{
    private const int DefaultAlignment = 16;

    private byte* _buffer;
    private readonly int _capacity;
    private int _offset;
    private bool _disposed;

    public int CapacityBytes => _capacity;
    public int UsedBytes => _offset;

    public ArenaAllocator(int capacityBytes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacityBytes);

        _capacity = capacityBytes;
        _buffer = (byte*)NativeMemory.AlignedAlloc((nuint)capacityBytes, DefaultAlignment);
    }

    public Span<T> Allocate<T>(int count) where T : unmanaged
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        int bytes = sizeof(T) * count;
        int aligned = Align(bytes, DefaultAlignment);

        if (_offset + aligned > _capacity)
            throw new ArenaExhaustedException(
                $"Arena exhausted: requested {aligned} bytes, {_capacity - _offset} available.");

        var span = new Span<T>(_buffer + _offset, count);
        _offset += aligned;
        return span;
    }

    public void Reset() => _offset = 0;

    private static int Align(int size, int alignment) => (size + alignment - 1) & ~(alignment - 1);

    public void Dispose()
    {
        if (_disposed)
            return;

        NativeMemory.AlignedFree(_buffer);
        _buffer = null;
        _disposed = true;
    }
}

public sealed class ArenaExhaustedException(string message) : InvalidOperationException(message);