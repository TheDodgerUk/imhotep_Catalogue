using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : Component
{
    public static T Instance { get; private set; }

    public static void CreateInstance()
    {
        if (Instance == null)
        {
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(MonoSingleton.GetParent(), false);
            Instance = go.AddComponent<T>();
        }
    }

    public static void DestroyInstance()
    {
        if (Instance != null)
        {
            GameObject.Destroy(Instance.gameObject);
            Instance = null;
        }
    }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = GetComponent<T>();
        }
    }
}

public static class MonoSingleton
{
    private static Transform m_Parent;

    public static Transform GetParent()
    {
        const string ParentName = "Singletons";
        if (m_Parent == null)
        {
            var parentObj = GameObject.Find(ParentName);

            if (parentObj == null)
            {
                parentObj = new GameObject(ParentName);
                if (Application.isPlaying)
                {
                    Object.DontDestroyOnLoad(parentObj);
                }
            }
            m_Parent = parentObj.transform;
        }
        return m_Parent;
    }
}