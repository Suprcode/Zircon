using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraryCore.Coroutine;

public static class CoroutineHelper
{
    private static readonly LinkedList<Coroutine>_coroutines;
    private static readonly Queue<Coroutine> _coroutinePool;

    static CoroutineHelper()
    {
        _coroutines = new ();
        _coroutinePool = new ();
    }

    public static Coroutine GetCoroutine(IEnumerable enumerable, DateTime startTime)
    {
        if (_coroutinePool.TryDequeue(out var coroutine))
        {
            coroutine.Reset(enumerable, startTime);
        }
        else
        {
            coroutine = new Coroutine(enumerable, startTime);
        }
        return coroutine;
    }

    /// <summary>
    /// 更新所有协程
    /// </summary>
    public static void UpdateCoroutines()
    {
        var node = _coroutines.First;
        while (node != null)
        {
            node.Value.Update();
            if (node.Value.Finish)
            {
                var t = node;
                node = node.Next;
                _coroutines.Remove(t);
                _coroutinePool.Enqueue(t.Value);
            }
            else
            {
                node = node.Next;
            }
        }
    }

    /// <summary>
    /// 创建一个协程并自动添加到协程列表中
    /// </summary>
    /// <param name="enumerable">迭代器</param>
    /// <param name="waitSeconds">多少秒后开始执行</param>
    /// <param name="name">协程名字</param>
    /// <returns>协程实列</returns>
    public static Coroutine StartCoroutine(IEnumerable enumerable, double waitSeconds = 0, string name = null)
    {
        Coroutine coroutine = GetCoroutine(enumerable, DateTime.Now.AddSeconds(waitSeconds));
        coroutine.Name = name;
        _coroutines.AddLast(coroutine);
        return coroutine;
    }
    
    /// <summary>
    /// 添加一个手动创建的协程
    /// </summary>
    /// <param name="coroutine">协程实列</param>
    public static void AddCoroutine(Coroutine coroutine)
    {
        _coroutines.AddLast(coroutine);
    }

    /// <summary>
    /// 结束第一个匹配名字的协程，必须事先命名
    /// </summary>
    /// <param name="name">名字</param>
    public static Coroutine StopCoroutine(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        LinkedListNode<Coroutine> node = _coroutines.First;
        while (node != null)
        {
            if (node.Value.Name.Equals(name))
            {
                _coroutines.Remove(node);
                return node.Value;
            }
            node = node.Next;
        }

        return null;
    }
    
    /// <summary>
    /// 结束所有协程,释放资源
    /// </summary>
    public static void StopAllCoroutine()
    {
        foreach (var coroutine in _coroutines)
        {
            _coroutinePool.Enqueue(coroutine);
        }
        _coroutines.Clear();
    }
}