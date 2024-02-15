using System.Buffers;

namespace PreonBenchmarks;

public class NoopArrayPool<T>: ArrayPool<T>
{
    public override T[] Rent(int minimumLength)
    {
        return new T[minimumLength];
    }

    public override void Return(T[] array, bool clearArray = false)
    {
    }
}