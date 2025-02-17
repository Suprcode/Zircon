using System;
using System.Threading.Tasks;

namespace LibraryCore.Coroutine;

/// <summary>
/// 异步等待,普通回收版
/// </summary>
public sealed class WaitAsync : CoroutineWaitBase<WaitAsync>
{
    private Task _task;
    private Action<Task> _completedCallbackAction;

    public WaitAsync() { }
    public WaitAsync(Task task)
    {
        _task = task;
    }

    public override bool Wait()
    {
        bool wait = _task != null && !_task.IsCompleted;
        if (!wait)
        {
            _completedCallbackAction?.Invoke(_task);
            Recycle();
        }
        return wait;
    }

    protected override WaitAsync Reset()
    {
        _task = null;
        _completedCallbackAction = null;
        return this;
    }
    public static WaitAsync Build(Task task) => Build(w => w._task = task);
    public WaitAsync CompletedCallback(Action<Task> action)
    {
        _completedCallbackAction = action;
        return this;
    }

    public static implicit operator WaitAsync(Task task)
    {
        return new WaitAsync(task);
    }
}


/// <summary>
/// 异步等待,泛型回收版
/// </summary>
/// <typeparam name="T">异步返回值类型</typeparam>
public sealed class WaitAsync<T> : CoroutineWaitBase<WaitAsync<T>>
{
    private Task<T> _task;
    private Action<Task<T>> _completedCallbackAction;
    public T Result => _task.Result;

    public WaitAsync() { }
    public WaitAsync(Task<T> task)
    {
        _task = task;
    }

    public override bool Wait()
    {
        bool wait = _task != null && !_task.IsCompleted;
        if (!wait)
        {
            _completedCallbackAction?.Invoke(_task);
            Recycle();
        }
        return wait;
    }

    protected override WaitAsync<T> Reset()
    {
        _task = null;
        _completedCallbackAction = null;
        return this;
    }
    public static WaitAsync<T> Build(Task<T> task) => Build(w => w._task = task);
    public WaitAsync<T> CompletedCallback(Action<Task<T>> action)
    {
        _completedCallbackAction = action;//需要协程结束时触发而不是异步完成时触发
        //_Task.ContinueWith(func);
        return this;
    }

    public static implicit operator WaitAsync<T>(Task<T> task)
    {
        return new WaitAsync<T>(task);
    }
}