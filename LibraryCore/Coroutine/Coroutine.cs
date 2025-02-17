using System;
using System.Collections;

namespace LibraryCore.Coroutine;

public class Coroutine
{
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 所属线程ID
    /// </summary>
    private ICoroutineWait _wait;
    private IEnumerator _enumerator;
    /// <summary>
    /// 是否已结束
    /// </summary>
    public bool Finish { get; private set; }

    public Coroutine(IEnumerable enumerable, DateTime startTime)
    {
        _enumerator = enumerable.GetEnumerator();
        _wait = new WaitTime_S(startTime);
    }

    public void Reset(IEnumerable enumerable, DateTime startTime)
    {
        Name = null;
        Finish = false;
        _enumerator = enumerable.GetEnumerator();
        _wait = new WaitTime_S(startTime);
    }

    public void Update()
    {
        if (Finish || _wait != null && _wait.Wait())
            return;

        if (_enumerator.MoveNext())
        {
            _wait = _enumerator.Current as ICoroutineWait;
            if (_wait == null && _enumerator.Current is IEnumerable enumerable)
            {
                _enumerator = enumerable.GetEnumerator();
                _wait = new WaitFrame_S(1);
            }
        }
        else
        {
            Finish = true;
        }
    }
}
