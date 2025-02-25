using System;

namespace LibraryCore.Coroutine;

/// <summary>
/// 等待一个引用方法，直到返回为false
/// </summary>
public sealed class WaitFunc : CoroutineWaitBase<WaitFunc>
{
    private Func<bool> _func;
    static bool DefFunc() => false;
    public WaitFunc(Func<bool> func)
    {
        _func = func;
    }

    public WaitFunc()
    {
        _func = DefFunc;
    }

    public override bool Wait()
    {
        if (_func())
        {
            return true;
        }
        Recycle();
        return false;
    }

    protected override WaitFunc Reset()
    {
        _func = DefFunc;
        return this;
    }
    public static WaitFunc Build(Func<bool> func) => Build(w => w._func = func);
}