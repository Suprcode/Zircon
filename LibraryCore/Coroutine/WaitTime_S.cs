using System;

namespace LibraryCore.Coroutine;

/// <summary>
/// 按时间等待,简易版
/// </summary>
public sealed class WaitTime_S : ICoroutineWait
{
    private readonly DateTime _time;
    public WaitTime_S(double seconds)
    {
        _time = DateTime.Now.AddSeconds(seconds);
    }

    public WaitTime_S(DateTime time)
    {
        _time = time;
    }

    public bool Wait()
    {
        return _time > DateTime.Now;
    }
}