namespace LibraryCore.Coroutine;

/// <summary>
/// 按帧等待,简易版
/// </summary>
public sealed class WaitFrame_S : ICoroutineWait
{
    private long _frame;
    public WaitFrame_S(int frame)
    {
        _frame = frame;
    }

    public bool Wait()
    {
        return _frame-- > 0;
    }
}