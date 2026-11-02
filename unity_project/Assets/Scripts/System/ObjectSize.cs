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

    public void UpdateRotation(bool badXRotation)
    {
        this.transform.Find("PlacementCollision").localRotation = Quaternion.Euler(badXRotation ? -90f : 0f, 0f, 0f);
        this.transform.Find("RootModel").localRotation = Quaternion.Euler(badXRotation ? -90f : 0f, 0f, 0f);
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

    /// <summary>
    /// //////
    /// </summary>
    [Header("Child Names")]
    [SerializeField]
    private string rootModelName = "RootModel";

    [SerializeField]
    private string collisionName = "PlacementCollision";

    public void AlignModelBase()
    {
#if UNITY_EDITOR
        Transform rootModel = transform.Find(rootModelName);
        Transform placementCollision = transform.Find(collisionName);

        if (rootModel == null || placementCollision == null)
        {
            Debug.LogError(
                "RootModel or PlacementCollision not found!",
                this
            );
            return;
        }

        // ---------------------------------------------
        // 1. PRESERVE EXISTING MODEL ROTATION
        // ---------------------------------------------

        Quaternion existingRotation = rootModel.localRotation;

        // The model may already be 0 or -90.
        // Do not force a different rotation.
        // Make the collision use the same rotation.
        //
        // Assumes the collision mesh was authored
        // with the same local axis orientation.

        // ---------------------------------------------
        // 2. FIND LOWEST VERTEX
        // ---------------------------------------------

        float lowestY = float.PositiveInfinity;
        bool foundMesh = false;

        MeshFilter[] meshFilters =
            rootModel.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter meshFilter in meshFilters)
        {
            Mesh mesh = meshFilter.sharedMesh;

            if (mesh == null)
                continue;

            Vector3[] vertices = mesh.vertices;

            foreach (Vector3 vertex in vertices)
            {
                Vector3 worldPoint =
                    meshFilter.transform.TransformPoint(vertex);

                Vector3 localPoint =
                    transform.InverseTransformPoint(worldPoint);

                lowestY = Mathf.Min(lowestY, localPoint.y);
                foundMesh = true;
            }
        }

        SkinnedMeshRenderer[] skinnedMeshes =
            rootModel.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (SkinnedMeshRenderer renderer in skinnedMeshes)
        {
            Bounds bounds = renderer.localBounds;

            Vector3 centre = bounds.center;
            Vector3 extents = bounds.extents;

            for (int x = -1; x <= 1; x += 2)
            {
                for (int y = -1; y <= 1; y += 2)
                {
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Vector3 corner = centre +
                            Vector3.Scale(
                                extents,
                                new Vector3(x, y, z)
                            );

                        Vector3 worldPoint =
                            renderer.transform.TransformPoint(corner);

                        Vector3 localPoint =
                            transform.InverseTransformPoint(worldPoint);

                        lowestY = Mathf.Min(lowestY, localPoint.y);
                        foundMesh = true;
                    }
                }
            }
        }

        if (!foundMesh)
        {
            Debug.LogError(
                "No meshes found under RootModel!",
                this
            );
            return;
        }

        // ---------------------------------------------
        // 3. CALCULATE Y OFFSET
        // ---------------------------------------------

        float offsetY = -lowestY;

        // ---------------------------------------------
        // 4. APPLY CHANGES
        // ---------------------------------------------

        Undo.RecordObject(rootModel, "Align RootModel");
        Undo.RecordObject(
            placementCollision,
            "Align PlacementCollision"
        );

        // Keep the original RootModel rotation.
        rootModel.localRotation = existingRotation;

        // Match collision rotation to model rotation.
        placementCollision.localRotation = existingRotation;

        // Move both children by the same Y offset.
        Vector3 modelPosition = rootModel.localPosition;
        modelPosition.y += offsetY;
        rootModel.localPosition = modelPosition;

        Vector3 collisionPosition =
            placementCollision.localPosition;

        collisionPosition.y += offsetY;
        placementCollision.localPosition = collisionPosition;

        // ---------------------------------------------
        // 5. SAVE CHANGES
        // ---------------------------------------------

        EditorUtility.SetDirty(rootModel);
        EditorUtility.SetDirty(placementCollision);

        Physics.SyncTransforms();

        Debug.Log(
            $"Aligned: {name}\n" +
            $"Model rotation: {rootModel.localEulerAngles}\n" +
            $"Collision rotation: {placementCollision.localEulerAngles}\n" +
            $"Original lowest Y: {lowestY:F4}\n" +
            $"Applied Y offset: {offsetY:F4}",
            this
        );
#endif
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

        if (GUILayout.Button("Bad Rotation"))
        {
            Undo.RecordObject(item, "Update Rotation");

            item.UpdateRotation(true);

            SaveChanges(item);
        }

        if (GUILayout.Button("Normal Rotation"))
        {
            Undo.RecordObject(item, "Update Rotation");

            item.UpdateRotation(false);

            SaveChanges(item);
        }

        if (GUILayout.Button("Fix Model Base Y = 0"))
        {
            item.AlignModelBase();
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