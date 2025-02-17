using System;
using System.Collections;
using System.Collections.Generic;

namespace LibraryCore.Coroutine;

/// <summary>
/// 协程对象基类
/// (单线程,所有CoroutineObject对象必须在调用UpdateObjects()的线程下，除了独立更新对象)
/// </summary>
public abstract class CoroutineObject
{
    /// <summary>
    /// 释放资源
    /// </summary>
    public void Relese()
    {
        Coroutines.Clear();
    }

    #region 更新和创建协程

    protected readonly LinkedList<Coroutine> Coroutines = new LinkedList<Coroutine>();

    /// <summary>
    /// 更新所有协程
    /// </summary>
    protected virtual void UpdateCoroutines()
    {
        LinkedListNode<Coroutine> node = Coroutines.First;
        while (node != null)
        {
            node.Value.Update();
            if (node.Value.Finish)
            {
                var t = node;
                node = node.Next;
                //修复协程对象自身调用Relese后移除出错
                if (Coroutines.Count > 0)
                {
                    Coroutines.Remove(t);
                }
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
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerable enumerable, double waitSeconds = 0)
    {
        Coroutine coroutine = new Coroutine(enumerable, DateTime.Now.AddSeconds(waitSeconds));
        Coroutines.AddLast(coroutine);
        return coroutine;
    }

    /// <summary>
    /// 添加一个手动创建的协程
    /// </summary>
    /// <param name="coroutine">协程实列</param>
    public virtual void AddCoroutine(Coroutine coroutine)
    {
        Coroutines.AddLast(coroutine);
    }

    /// <summary>
    /// 结束第一个匹配名字的协程，必须事先命名
    /// </summary>
    /// <param name="name">名字</param>
    public virtual Coroutine StopCoroutine(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        LinkedListNode<Coroutine> node = Coroutines.First;
        while (node != null)
        {
            if (node.Value.Name == name)
            {
                Coroutines.Remove(node);
                return node.Value;
            }
            node = node.Next;
        }

        return null;
    }

    /// <summary>
    /// 结束所有协程
    /// </summary>
    public virtual void StopAllCoroutine()
    {
        Coroutines.Clear();
    }

    #endregion 更新和创建协程
}