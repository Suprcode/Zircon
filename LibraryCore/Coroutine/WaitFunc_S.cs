using System;

namespace LibraryCore.Coroutine;

/// <summary>
/// 等待一个引用方法，直到返回为false
/// </summary>
public sealed class WaitFunc_S : ICoroutineWait
{
    private readonly Func<bool> _func;
    public WaitFunc_S(Func<bool> func)
    {
        _func = func;
    }

    public bool Wait()
    {
        return _func();
    }
}