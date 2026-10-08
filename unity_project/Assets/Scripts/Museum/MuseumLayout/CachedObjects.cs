using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CachedObjects : MonoSingleton<CachedObjects>
{
    public const string TAG = "[CachedObjects]";

    [SerializeField]
    private Dictionary<string, GameObject> m_DictOfCachedObjects = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();
    }

    void OnPreEnvironmentLoad()
    {
        foreach (var item in m_DictOfCachedObjects)
        {
            Destroy(item.Value);
        }
        m_DictOfCachedObjects.Clear();
    }


    public void AddObjectsToCache(Transform parent)
    {
        List<Transform> childrenRef = parent.GetDirectChildren();
        for (int i = 0; i < childrenRef.Count; i++)
        {
            string name = childrenRef[i].name;
            if (false == m_DictOfCachedObjects.ContainsKey(name))
            {
                m_DictOfCachedObjects.Add(name, childrenRef[i].gameObject);
                parent.SetActive(false);
            }
            else
            {
                Debug.LogError($" {name} already exists, please check your scenes");
            }
        }
    }

    public List<string> GetItemsWhichContain(string name)
    {
        List<string> names = new List<string>();
        foreach (string item in m_DictOfCachedObjects.Keys)
        {
            if (true == item.CaseInsensitiveContains(name))
            {
                names.Add(item);
            }
        }
        return names;
    }

    public GameObject InstantiateCachedGameObject(string name, bool removeCloneName = true)
    {
        if (true == m_DictOfCachedObjects.ContainsKey(name))
        {
            GameObject t = Instantiate(m_DictOfCachedObjects[name]);
            if (true == removeCloneName)
            {
                t.name = name;
            }
            return t;
        }
        else
        {
            Debug.LogError($" {name} does not exist");
            return null;
        }
    }

    public GameObject InstantiateCachedGameObject(string name, Transform parent, bool removeClone = true)
    {
        if (true == m_DictOfCachedObjects.ContainsKey(name))
        {
            GameObject t = Instantiate(m_DictOfCachedObjects[name], parent);
            if (true == removeClone)
            {
                t.name = name;
            }
            return t;
        }
        else
        {
            Debug.LogError($" {name} does not exist");
            return null;
        }
    }
}
