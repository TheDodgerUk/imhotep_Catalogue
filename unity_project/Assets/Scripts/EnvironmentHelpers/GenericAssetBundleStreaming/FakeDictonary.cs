using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FakeDictoryStrings : FakeDictonary<string, string>
{
    public FakeDictonaryItem GetItem(string name)
    {
        FakeDictonaryItem item = m_Container.Find(x => x.m_Key == name);
        if (null == item)
        {
            item = m_Container.Find(x => x.m_Value == name);
        }
        return item;
    }
};

public class FakeDictonary<T_Key, T_Value>
{
    [SerializeField]
    public List<FakeDictonaryItem> m_Container = new List<FakeDictonaryItem>(); // had to change it to allow it to be read to written and read via Json

#if CATALOG_PROGRAM
    public List<FakeDictonaryItem> GetList()
    {
        return m_Container;
    }
#endif

    public FakeDictonaryItem GetItemFromKey(T_Key key)
    {
        for (int i = 0; i < m_Container.Count; i++)
        {
            if (ReferenceEquals(key, m_Container[i].m_Key))
            {
                return m_Container[i];
            }
        }
        return new FakeDictonaryItem();
    }

    public FakeDictonaryItem GetItemFromValue(T_Value value)
    {
        for (int i = 0; i < m_Container.Count; i++)
        {
            if (ReferenceEquals(value, m_Container[i].m_Value))
            {
                return m_Container[i];
            }
        }
        return new FakeDictonaryItem();
    }


    public FakeDictonaryItem this[uint i]
    {
        get
        {
            if (i < m_Container.Count)
            {
                return m_Container[(int)i];
            }
            else
            {
                Debug.LogError($"container does not have an index at {i}");
                return new FakeDictonaryItem();
            }
        }
        set { m_Container[(int)i] = value; }
    }


    public void Add(FakeDictonaryItem item)
    {
        bool found = false;
        for (int i = 0; i < m_Container.Count; i++)
        {
            if (ReferenceEquals(item.m_Key, m_Container[i].m_Key))
            {
                found = true;
            }
        }

        if (false == found)
        {
            m_Container.Add(item);
        }
    }

    public int Count => m_Container.Count;
    public List<T_Key> Keys => m_Container.ConvertAll(x => x.m_Key);
    public List<T_Value> Values => m_Container.ConvertAll(x => x.m_Value);

    [SerializeField]
    public class FakeDictonaryItem
    {
        [SerializeField]
        public T_Key m_Key;

        [SerializeField]
        public T_Value m_Value;
    }
}
