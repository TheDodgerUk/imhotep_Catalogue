using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[ExecuteAlways]
public class ObjectSize : MonoBehaviour
{
    public enum Dimension
    {
        Width,
        Height,
        Depth
    }

    [Header("Current size in metres")]
    public Vector3 size;

    [Header("Resize settings")]
    public Dimension resizeBy = Dimension.Height;

    [Min(0.0001f)]
    public float targetSize = 0.035f;

    private void Awake()
    {
        UpdateSize();
    }

    public void UpdateSize()
    {
        Renderer[] renderers =
            GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            size = Vector3.zero;
            return;
        }

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        size = bounds.size;
    }

    public void ResizeObject()
    {
        UpdateSize();

        float currentSize;

        switch (resizeBy)
        {
            case Dimension.Width:
                currentSize = size.x;
                break;

            case Dimension.Height:
                currentSize = size.y;
                break;

            default:
                currentSize = size.z;
                break;
        }

        if (currentSize <= 0.000001f || targetSize <= 0f)
        {
            Debug.LogWarning(
                "Cannot resize: current and target sizes must be above zero.",
                this
            );
            return;
        }

        float multiplier = targetSize / currentSize;

        transform.localScale *= multiplier;

        UpdateSize();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectSize))]
public class ObjectSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectSize item = (ObjectSize)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Update Size"))
        {
            Undo.RecordObject(item, "Update Object Size");

            item.UpdateSize();

            SaveChanges(item);
        }

        if (GUILayout.Button("Resize Object"))
        {
            Undo.RecordObjects(
                new Object[] { item, item.transform },
                "Resize Object"
            );

            item.ResizeObject();

            SaveChanges(item);
            SaveChanges(item.transform);
        }
    }

    private static void SaveChanges(Object obj)
    {
        EditorUtility.SetDirty(obj);

        if (PrefabUtility.IsPartOfPrefabInstance(obj))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        }
    }
}
#endif