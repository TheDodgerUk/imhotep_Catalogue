using System;
using System.Collections.Generic;
using UnityEngine;

public class ReuseablePool<T> where T : Component
{
    public Predicate<T> IsActive = null;

    private GameObject m_OriginalObj;
    private Transform m_Parent;

    private List<T> m_Pool;
    private T m_Original;

    public ReuseablePool(T original, Transform parent)
    {
        m_OriginalObj = original.gameObject;
        m_Original = original;
        m_Parent = parent;

        m_Pool = new List<T>();
    }

    public void ForEach(Action<T> action)
    {
        m_Pool.ForEach(action);
        action(m_Original);
    }

    public T GetNext()
    {
        var next = m_Pool.Find(IsActive);
        if (next == null)
        {
            next = SpawnNew();
        }
        return next;
    }

    private T SpawnNew()
    {
        var next = Utils.Unity.Clone<T>(m_OriginalObj, m_Parent);
        next.name = m_Pool.Count.ToString();
        m_Pool.Add(next);
        return next;
    }
}
