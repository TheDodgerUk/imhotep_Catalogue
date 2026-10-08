using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

using System.Diagnostics;
using System.IO;



public class SimpleDictionaryList<TKey, TValue>
{
    Dictionary<TKey, List<TValue>> m_Dictionary = new Dictionary<TKey, List<TValue>>();
    public SimpleDictionaryList()
    {
        m_Dictionary.Clear();
    }

    /// <summary>
    /// 
    /// <returns></returns>
    public List<TValue> GetList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
        return m_Dictionary[lTKey];
    }

    public TValue GetRandomFromList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
        int index = UnityEngine.Random.Range(0, m_Dictionary[lTKey].Count);
        return m_Dictionary[lTKey][index];
    }


    public List<TKey> GetKeys()
    {
        var lKeys = m_Dictionary.Keys;
        List<TKey> lList = new List<TKey>();
        foreach (var lKey in lKeys)
        {
            lList.Add(lKey);
        }
        return lList;
    }


    public void GetKey(string lKeyString, ref TKey lTKey)
    {
        List<TKey> lList = new List<TKey>();
        foreach (var lKey in lList)
        {
            if (lKey.ToString().Contains(lKeyString) == true)
            {
                lTKey = lKey;
            }
        }
    }


    public int Count(TKey lKey)
    {
        m_CheckForKey(lKey);
        return m_Dictionary[lKey].Count;
    }

    public int CountAll()
    {
        int count = 0;
        foreach(var key in m_Dictionary.Keys)
        {
            count += m_Dictionary[key].Count;
        }
        return count;
    }

    public List<TValue> this[TKey lTKey]
    {
        get
        {
            m_CheckForKey(lTKey);
            return m_Dictionary[lTKey];
        }
        set
        {
            m_CheckForKey(lTKey);
            m_Dictionary[lTKey] = value;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void ClearAll()
    {
        foreach (TKey lKey in m_Dictionary.Keys)
        {
            m_CheckForKey(lKey);
            m_Dictionary[lKey].Clear();
        }
        m_Dictionary.Clear();
    }
    /// <summary>
    /// 
    /// <param name="lEDebugType"></param>
    public void ClearList(TKey lTKey)
    {
        m_CheckForKey(lTKey);
        m_Dictionary[lTKey].Clear();
    }

    public void GetValue(TKey lTKey, string lName, ref TValue lRefItem)
    {
        List<TValue> lList = GetList(lTKey);
        foreach (var lItem in lList)
        {
            if (lItem.ToString() == lName)
            {
                lRefItem = lItem;
            }
        }
    }

    public void GetValue(string KeyName, string lName, ref TValue lRefItem)
    {
        List<TKey> lListKeys = GetKeys();
        foreach (var lKey in lListKeys)
        {
            if (lKey.ToString().Contains(KeyName))
            {
                List<TValue> lValues = GetList(lKey);
                foreach (var lItem in lValues)
                {
                    if (lItem.ToString().Contains(lName))
                    {
                        lRefItem = lItem;
                    }
                }
            }
        }
    }


    public List<TValue> GetValuesList(TKey lTKey)
    {
        return  GetList(lTKey);
    }

    public void AddToList(TKey lTKey, TValue lTValue)
    {
        m_CheckForKey(lTKey);
        m_Dictionary[lTKey].Add(lTValue);
    }

    public void AddToList(TKey lTKey, List<TValue> lTValue)
    {
        m_CheckForKey(lTKey);
        foreach (var item in lTValue)
        {
            m_Dictionary[lTKey].Add(item);
        }
    }

    public void RemoveFromList(TKey lTKey, TValue lTValue)
    {
        m_CheckForKey(lTKey);
        m_Dictionary[lTKey].Remove(lTValue);
    }

    private void m_CheckForKey(TKey lTKey)
    {
        if (m_Dictionary.ContainsKey(lTKey) == false)
        {
            m_Dictionary.Add(lTKey, new List<TValue>());
        }
    }

}

