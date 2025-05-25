using System;
using System.Collections.Concurrent;

namespace LibraryCore.Coroutine;

public abstract class CoroutineWaitBase<T> : ICoroutineWait where T : class, ICoroutineWait, new()
{
    private static readonly ConcurrentQueue<T> InstancePool = new();
    public static int PoolSize = 1024;
    protected CoroutineWaitBase() { }
    public abstract bool Wait();
    protected abstract T Reset();

    protected void Recycle()
    {
        if (InstancePool.Count < PoolSize)
            InstancePool.Enqueue(Reset());
    }
    protected static T Build(Action<T> action)
    {
        if (!InstancePool.TryDequeue(out var wait))
        {
            wait = new T();
        }

        action?.Invoke(wait);
        return wait;
    }

    public static void ClearPool()
    {
        InstancePool.Clear();
    }
}