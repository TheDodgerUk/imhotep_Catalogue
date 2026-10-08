using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UnityExtensions
{
    public static void DestroyObject(this Object obj)
    {
        if (obj != null)
        {
            Object.Destroy(obj);
        }
    }

    public static void DestroyGameObject<T>(this T component) where T : Component
    {
        if (component != null)
        {
            component.gameObject.DestroyObject();
        }
    }

    public static void DestroyObjectImmediate(this Object obj, bool allowDestroyAssets = false)
    {
        if (obj != null)
        {
            Object.DestroyImmediate(obj, allowDestroyAssets);
        }
    }

    public static void DestroyGameObjectImmediate<T>(this T component, bool allowDestroyAssets = false) where T : Component
    {
        if (component != null)
        {
            component.gameObject.DestroyObjectImmediate(allowDestroyAssets);
        }
    }

    public static RectTransform ToRect(this Transform transform)
    {
        return transform as RectTransform;
    }

    public static RectTransform GetRect<T>(this T component) where T : Component
    {
        return component.transform.ToRect();
    }

    public static Vector2 GetCentre(this RectTransform transform)
    {
        var corners = new Vector3[4];
        transform.GetLocalCorners(corners);

        var total = Vector2.zero;
        for (int i = 0; i < corners.Length; ++i)
        {
            total += (Vector2)corners[i];
        }
        var average = (total / corners.Length);
        return transform.anchoredPosition + average;
    }

    public static Vector3 GetRelativePosition(this Transform target, Transform anchor)
    {
        if (target.IsChildOf(anchor) == false)
        {
            throw new System.Exception("Target must be a child of anchor!");
        }

        if (target.parent != anchor)
        {
            return target.parent.GetRelativePosition(anchor) + target.localPosition;
        }
        else
        {
            return target.localPosition;
        }
    }


    public static void SetStaticRecursively(this GameObject go, bool state)
    {
        if (go.isStatic == false)
        {
            Debug.Log("Change to static");
        }
        go.isStatic = state;
        foreach (Transform child in go.transform)
        {
            Debug.LogError("SetStaticRecursively  removed");
            ////////////////////////// SetStaticRecursively(child.gameObject, state);
        }
    }

    public static void SetStaticRecursively(this GameObject go)
    {
        if(go.isStatic == false)
        {
            Debug.Log("Change to static");
        }
        go.isStatic = true;
        foreach (Transform child in go.transform)
        {

            Debug.LogError("SetStaticRecursively  removed");
           ///////////////////// SetStaticRecursively(child.gameObject);
        }
    }

    public static void SetLayerRecursively(this GameObject go, int layer)
    {
        go.layer = layer;
        foreach (Transform child in go.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public static void SetTagRecursively(this GameObject go, string tag)
    {
        go.tag = tag;
        foreach (Transform child in go.transform)
        {
            SetTagRecursively(child.gameObject, tag);
        }
    }

    public static Color ToColour(this Vector3 v)
    {
        return new Color(v.x, v.y, v.z);
    }

    public static Vector3 ToVector3(this Color c)
    {
        return new Vector3(c.r, c.g, c.b);
    }

    public static Tuple<Vector3, Vector3> GetVertexRange(this Mesh mesh)
    {
        var verts = mesh.vertices;
        var min = Vector3.one * float.MaxValue;
        var max = Vector3.one * float.MinValue;
        for (int i = 0; i < verts.Length; ++i)
        {
            min = min.Min(verts[i]);
            max = max.Max(verts[i]);
        }
        return Tuple.New(min, max);
    }

    public static void SetAlpha(this Graphic graphic, float alpha)
    {
        var colour = graphic.color;
        colour.a = alpha;
        graphic.color = colour;
    }

    public static void SetAlpha(this Material mat, float alpha, string property = null)
    {
        property = property ?? "_Color";
        var colour = mat.GetColor(property);
        colour.a = alpha;
        mat.SetColor(property, colour);
    }

    public static void SetAlpha(this Renderer renderer, float alpha, string property = null)
    {
        renderer.material.SetAlpha(alpha, property);
    }

    public static void SetAlpha(this SpriteRenderer sprite, float alpha)
    {
        var colour = sprite.color;
        colour.a = alpha;
        sprite.color = colour;
    }

    public static void SetTexOffset(this Renderer renderer, float? x = null, float? y = null)
    {
        var m = renderer.material;
        var o = m.mainTextureOffset;
        m.mainTextureOffset = new Vector2(x ?? o.x, y ?? o.y);
    }

    public static void SetTexScale(this Renderer renderer, float? x = null, float? y = null)
    {
        var m = renderer.material;
        var s = m.mainTextureScale;
        m.mainTextureScale = new Vector2(x ?? s.x, y ?? s.y);
    }

    public static Rect GetRect(this Texture2D texture) => new Rect(0, 0, texture.width, texture.height);

    public static void ReadFromRenderTexture(this Texture2D texture, RenderTexture renderTexture)
    {
        var currentRenderTexture = RenderTexture.active;
        RenderTexture.active = renderTexture;
        texture.ReadPixels(texture.GetRect(), 0, 0);
        RenderTexture.active = currentRenderTexture;
    }

    public static Texture2DArray ToTexture2dArray(this List<Texture2D> textures, bool mipmap, bool linear)
    {
        var t = textures[0];
        int count = textures.Count;
        var array = new Texture2DArray(t.width, t.height, count, t.format, mipmap, linear)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };
        return FillTex2dArray(textures, count, array);
    }

    public static Texture2DArray ToTexture2dArray(this List<Texture2D> textures, Texture2DArray array)
    {
        return FillTex2dArray(textures, textures.Count, array);
    }

    private static Texture2DArray FillTex2dArray(List<Texture2D> textures, int count, Texture2DArray array)
    {
        for (int i = 0; i < count; ++i)
        {
            if (textures[i] != null)
            {
                Graphics.CopyTexture(textures[i], 0, 0, array, i, 0);
            }
        }
        return array;
    }

    public static ComputeBuffer CreateComputeBuffer<T>(this T[] array)
    {
        int stride = array.Length > 0 ? Marshal.SizeOf(array[0]) : Marshal.SizeOf(typeof(T));
        var buffer = new ComputeBuffer(array.Length, stride);
        buffer.SetData(array);
        return buffer;
    }

    public static Transform CreateChild(this Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.Reset(true);
        return go.transform;
    }

    public static Component CreateChild(this Transform parent, System.Type type) => parent.CreateChild(type.Name).gameObject.AddComponent(type);
    public static T CreateChild<T>(this Transform parent, string name) where T : Component => parent.CreateChild(name).AddComponent<T>();
    public static T CreateChild<T>(this Transform parent) where T : Component => parent.CreateChild<T>(typeof(T).Name);

    public static string GetPathRelativeTo<T>(this T component, Transform relative) where T : Component => component.transform.GetPathRelativeTo(relative);
    public static string GetPathRelativeTo(this Transform transform, Transform relative) => Utils.Unity.GetPathRelativeTo(transform, relative);
    public static string GetScenePath(this Transform transform) => Utils.Unity.GetScenePath(transform);


    public static void AddEventTriggerListener(this EventTrigger trigger, EventTriggerType eventType, UnityAction<BaseEventData> callback)
    {
        var entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback = new EventTrigger.TriggerEvent();
        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    public static void RemoveEventTriggerType(this EventTrigger trigger, EventTriggerType eventType)
    {
        trigger.triggers.RemoveAll(e => e.eventID == eventType);
    }

    public static void SetAnchor(this RectTransform transform, float x, float y)
    {
        transform.anchorMin = new Vector2(x, y);
        transform.anchorMax = new Vector2(x, y);
        transform.pivot = new Vector2(x, y);
    }

    public static void SetHeight(this RectTransform transform, float height)
    {
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }

    public static void SetWidth(this RectTransform transform, float width)
    {
        transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
    }

    public static void SetAnchorPos(this RectTransform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.anchoredPosition3D = transform.anchoredPosition3D.With(x, y, z);
    }

    public static void SetLocalPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.localPosition = transform.localPosition.With(x, y, z);
    }

    public static void SetWorldPos(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.position = transform.position.With(x, y, z);
    }

    public static void SetLocalRot(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.localEulerAngles = transform.localEulerAngles.With(x, y, z);
    }

    public static void SetScale(this Transform transform, float? x = null, float? y = null, float? z = null)
    {
        transform.localScale = transform.localScale.With(x, y, z);
    }

    public static Vector3 With(this Vector3 original, float? x = null, float? y = null, float? z = null)
    {
        original.x = x ?? original.x;
        original.y = y ?? original.y;
        original.z = z ?? original.z;
        return original;
    }

    public static Color With(this Color original, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        original.r = r ?? original.r;
        original.g = g ?? original.g;
        original.b = b ?? original.b;
        original.a = a ?? original.a;
        return original;
    }

    public static void ClearLocals(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }

    public static void ClearLocals(this GameObject go)
    {
        go.transform.ClearLocals();
    }
}
