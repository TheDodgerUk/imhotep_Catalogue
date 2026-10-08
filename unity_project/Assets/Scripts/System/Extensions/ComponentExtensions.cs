using System.Collections.Generic;
using UnityEngine;

public static class ComponentExtensions
{
    public static void SetActive<T>(this T component, bool state) where T : Component
    {
        var go = component.gameObject;
        if (go.activeSelf != state)
        {
            go.SetActive(state);
        }
    }

    public static GameObject FindGameObject(this Transform transform, string name)
    {
        return transform?.Find(name)?.gameObject;
    }

    public static RectTransform FindRect(this Transform transform, string name)
    {
        return transform?.Find(name) as RectTransform;
    }

    public static T GetComponentInSibling<T>(this Transform transform, bool includeInactive = false) where T : Component
    {
        return transform.parent?.GetComponentInChildren<T>(includeInactive);
    }

    public static List<T> GetComponentsInSibling<T>(this Transform transform) where T : Component
    {
        return transform.parent?.GetComponentsInChildren<T>().ToList();
    }

    public static T FindComponent<T>(this Transform transform, string name) where T : Component
    {
        return transform?.Find(name)?.GetComponent<T>();
    }

    public static List<T> FindComponents<T>(this Transform transform, string name) where T : Component
    {
        var child = transform?.Find(name);
        if (child != null)
        {
            return child.FindComponents<T>();
        }
        return new List<T>();
    }

    public static List<T> FindComponents<T>(this Transform transform) where T : Component
    {
        var list = new List<T>();
        foreach (Transform child in transform)
        {
            list.AddIfNotNull(child.GetComponent<T>());
        }
        return list;
    }

    public static T FindComponentInSibling<T>(this Transform transform, string name) where T : Component
    {
        return transform.parent.FindComponent<T>(name);
    }

    public static List<T> FindComponentsInSibling<T>(this Transform transform, string name) where T : Component
    {
        return transform.parent.FindComponents<T>(name);
    }

    public static List<T> FindComponentsInChildren<T>(this Transform transform) where T : Component
    {
        return transform.GetComponentsInChildren<T>(true).ToList();
    }

    public static List<T> FindComponentsInChildren<T>(this Transform transform, string name) where T : Component
    {
        return transform.Find(name)?.FindComponentsInChildren<T>();
    }

    public static T SearchComponent<T>(this Transform transform, string name) where T : Component
    {
        return transform.Search(name)?.GetComponent<T>();
    }

    public static Transform Search(this Transform transform, string name)
    {
        var child = transform.Find(name);
        if (child != null)
        {
            return child;
        }
        else
        {
            foreach (Transform newChild in transform)
            {
                child = newChild.Search(name);
                if (child != null)
                {
                    return child;
                }
            }
        }
        return null;
    }

    public static List<Transform> SearchAll(this Transform transform, string name)
    {
        var children = transform.GetDirectChildren();
        var found = children.FindAll(t => t.name == name);
        var foundChildren = children.Extract(t => t.SearchAll(name));
        found.AddRange(foundChildren.Flatten());
        return found;
    }

    public static Transform FindSibling(this Transform transform, string name)
    {
        return transform.parent.Find(name);
    }

    public static List<Transform> GetDirectChildren(this Transform transform)
    {
        var children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child);
        }
        return children;
    }

    public static List<Transform> GetAllChildren(this Transform transform)
    {
        var children = new List<Transform>();
        foreach (Transform child in transform)
        {
            children.AddRange(child.GetAllChildren());
            children.Add(child);
        }
        return children;
    }

    public static Dictionary<string, Transform> SearchChildren(this Transform transform)
    {
        var children = new Dictionary<string, Transform>();
        foreach (Transform child in transform)
        {
            children.Add(child.name, child);

            var grandChildren = child.SearchChildren();
            foreach (var grandChild in grandChildren)
            {
                children.Add(grandChild.Key, grandChild.Value);
            }
        }
        return children;
    }

    public static List<Transform> SearchChildren(this Transform transform, string name)
    {
        var children = GetAllChildren(transform);
        var matching = new List<Transform>();
        foreach (Transform child in children)
        {
            if (child.name == name)
            {
                matching.Add(child);
            }
        }

        return matching;
    }

    public static Transform FindParent(this Transform transform, string name)
    {
        if (transform.parent == null)
        {
            return null;
        }
        else if (transform.parent.name == name)
        {
            return transform.parent;
        }
        else
        {
            return transform.parent.FindParent(name);
        }
    }

    public static void Reset(this Transform transform, bool withScale = true)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (withScale == true)
        {
            transform.localScale = Vector3.one;
        }
    }

    public static void OrientTo(this Transform transform, Transform target, bool withScale = true)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        if (withScale == true)
        {
            transform.localScale = target.localScale;
        }
    }

    public static void SnapToParent(this Transform transform, Transform parent, bool withScale = true)
    {
        if (transform.parent != parent)
        {
            transform.SetParent(parent, false);
        }
        transform.Reset(withScale);
    }

    public static List<Transform> GetOtherChildren(this Transform transform, params Component[] components)
    {
        var children = transform.FindComponents<Transform>();
        for (int i = 0; i < components.Length; ++i)
        {
            children.Remove(components[i].transform);
        }
        return children;
    }

    public static T ForceComponent<T>(this GameObject go) where T : Component
    {
        var component = go?.GetComponent<T>();
        if (component == null)
        {
            return go?.AddComponent<T>();
        }
        return component;
    }

    public static T AddComponent<T>(this Component component) where T : Component
    {
        return component.gameObject.AddComponent<T>();
    }

    public static string GetGameObjectPath(this Transform transform) 
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }

    public static string GetGameObjectPath(this GameObject gameObject)
    {
        return GetGameObjectPath(gameObject.transform);
    }


    public static string GetGameObjectPathWithScene(this Transform transform)
    {
        return transform.gameObject.scene.name + "  " + GetGameObjectPath(transform);
    }

    public static string GetGameObjectPathWithScene(this GameObject gameObject)
    {
        return gameObject.scene.name + "  " + GetGameObjectPath(gameObject.transform);
    }
}
