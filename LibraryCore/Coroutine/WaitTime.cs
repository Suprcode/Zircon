using System;

namespace LibraryCore.Coroutine;

/// <summary>
/// 按时间等待,回收版
/// </summary>
public sealed class WaitTime : CoroutineWaitBase<WaitTime>
{
    private DateTime _time;
    public WaitTime()
    {

    }
    public WaitTime(double seconds)
    {
        _time = DateTime.Now.AddSeconds(seconds);
    }

    public WaitTime(DateTime time)
    {
        _time = time;
    }

    public override bool Wait()
    {
        if (_time > DateTime.Now)
        {
            return true;
        }
        Recycle();
        return false;
    }

    protected override WaitTime Reset()
    {
        _time = DateTime.MaxValue;
        return this;
    }

    public static WaitTime Build(double seconds) => Build(DateTime.Now.AddSeconds(seconds));
    public static WaitTime Build(DateTime time) => Build(w => w._time = time);
}