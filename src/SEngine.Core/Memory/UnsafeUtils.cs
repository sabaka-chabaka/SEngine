using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SEngine.Core.Memory;

public static class UnsafeUtils
{
    public static Span<byte> AsBytes<T>(Span<T> span) where T : unmanaged
        => MemoryMarshal.AsBytes(span);

    public static ReadOnlySpan<byte> AsBytes<T>(ReadOnlySpan<T> span) where T : unmanaged
        => MemoryMarshal.AsBytes(span);

    public static Span<T> AsStruct<T>(Span<byte> bytes) where T : unmanaged
        => MemoryMarshal.Cast<byte, T>(bytes);

    public static Span<TTo> Reinterpret<TFrom, TTo>(Span<TFrom> span)
        where TFrom : unmanaged
        where TTo : unmanaged
        => MemoryMarshal.Cast<TFrom, TTo>(span);

    public static unsafe bool IsAligned(void* address, int alignment)
        => ((nuint)address & (nuint)(alignment - 1)) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int AlignUp(int size, int alignment) => (size + alignment - 1) & ~(alignment - 1);
}