namespace LibraryCore.Coroutine;

/// <summary>
/// 按帧等待,回收版
/// </summary>
public sealed class WaitFrame : CoroutineWaitBase<WaitFrame>
{
    private long _frame;
    public WaitFrame() { }
    public WaitFrame(int frame)
    {
        _frame = frame;
    }

    public override bool Wait()
    {
        if (_frame-- > 0)
        {
            return true;
        }
        Recycle();
        return false;
    }

    protected override WaitFrame Reset()
    {
        _frame = 0;
        return this;
    }
    public static WaitFrame Build(int frame) => Build(w => w._frame = frame);
}