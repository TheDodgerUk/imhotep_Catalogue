
using UnityEngine;
using UnityEngine.UIElements;


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

    [Header("Current size in cm")]
    public Vector3 sizeCm;


    [Header("Resize Settings")]
    public Dimension resizeBy = Dimension.Height;

    [Min(0.0001f)]
    public float targetSize = 0.035f;


    private string rootModelName = "RootModel";
    private string collisionName = "PlacementCollision";

    private Transform rootModel;
    private Transform placementCollision;

    private void Awake()
    {
        if (!FindChildren())
            return;

        this.transform.localScale = Vector3.one;
        UpdateSize();
    }

    private bool FindChildren()
    {
        rootModel = transform.Find(rootModelName);
        placementCollision = transform.Find(collisionName);

        if (rootModel == null || placementCollision == null)
        {
            Debug.LogError(
                "RootModel or PlacementCollision not found!",
                this
            );

            return false;
        }

        return true;
    }

    // --------------------------------------------------
    // UPDATE ROTATION
    // --------------------------------------------------

    public void UpdateRotation(bool badXRotation)
    {
        if (!FindChildren())
            return;

#if UNITY_EDITOR
        Undo.RecordObjects(
            new UnityEngine.Object[]
            {
                rootModel,
                placementCollision
            },
            "Update Model Rotation"
        );
#endif

        Quaternion rotation = Quaternion.Euler(
            badXRotation ? -90f : 0f,
            0f,
            0f
        );

        rootModel.localRotation = rotation;
        placementCollision.localRotation = rotation;

        Physics.SyncTransforms();

        UpdateSize();

        SaveTransform(rootModel);
        SaveTransform(placementCollision);
    }

    // --------------------------------------------------
    // UPDATE SIZE
    // --------------------------------------------------

    public void UpdateSize()
    {
        if (!FindChildren())
        {
            size = Vector3.zero;
            return;
        }

        Renderer[] renderers =
            rootModel.GetComponentsInChildren<Renderer>(true);

        bool found = false;
        Bounds bounds = new Bounds();

        foreach (Renderer renderer in renderers)
        {
            if (!found)
            {
                bounds = renderer.bounds;
                found = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        size = found ? bounds.size : Vector3.zero;
        sizeCm = new Vector3(
            Mathf.Round(size.x * 1000f) / 10f,
            Mathf.Round(size.y * 1000f) / 10f,
            Mathf.Round(size.z * 1000f) / 10f
        );
    }

    // --------------------------------------------------
    // RESIZE OBJECT
    // --------------------------------------------------

    public void ResizeObject()
    {
        if (!FindChildren())
            return;

        if (targetSize <= 0f)
        {
            Debug.LogWarning(
                "Target size must be greater than zero.",
                this
            );
            return;
        }

#if UNITY_EDITOR
        Undo.RecordObjects(
            new UnityEngine.Object[]
            {
                transform,
                rootModel,
                placementCollision
            },
            "Resize Model and Collision"
        );
#endif

        // Preserve the existing world geometry when
        // normalising the parent scale.
        Vector3 parentScale = transform.localScale;

        if (Mathf.Abs(parentScale.x) < 0.000001f ||
            Mathf.Abs(parentScale.y) < 0.000001f ||
            Mathf.Abs(parentScale.z) < 0.000001f)
        {
            Debug.LogWarning(
                "Cannot resize: parent has a zero scale axis.",
                this
            );
            return;
        }

        // Move the parent's scale into both children.
        // This preserves their effective local transforms
        // when the parent scale is uniform.
        bool uniformParent =
            Mathf.Approximately(parentScale.x, parentScale.y) &&
            Mathf.Approximately(parentScale.y, parentScale.z);

        if (!uniformParent)
        {
            Debug.LogWarning(
                "Non-uniform parent scale cannot always be " +
                "transferred exactly to rotated children. " +
                "Use a uniform parent scale for reliable results.",
                this
            );
            return;
        }

        float existingScale = parentScale.x;

        rootModel.localScale *= existingScale;
        placementCollision.localScale *= existingScale;

        rootModel.localPosition *= existingScale;
        placementCollision.localPosition *= existingScale;

        transform.localScale = Vector3.one;

        Physics.SyncTransforms();
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

        if (currentSize <= 0.000001f)
        {
            Debug.LogWarning(
                "Cannot resize: selected dimension is zero.",
                this
            );

            SaveAllTransforms();
            return;
        }

        float multiplier = targetSize / currentSize;

        // Scale both children by the same amount.
        rootModel.localScale *= multiplier;
        placementCollision.localScale *= multiplier;

        // Scale local offsets to preserve alignment.
        rootModel.localPosition *= multiplier;
        placementCollision.localPosition *= multiplier;

        // Do not change either child's rotation.
        // Supports both 0,0,0 and -90,0,0.

        Physics.SyncTransforms();

        UpdateSize();

        SaveAllTransforms();

        Debug.Log(
            $"Resized: {name}\n" +
            $"Target size: {targetSize:F5}\n" +
            $"Multiplier: {multiplier:F5}\n" +
            $"Parent scale: {transform.localScale}\n" +
            $"RootModel scale: {rootModel.localScale}\n" +
            $"Collision scale: {placementCollision.localScale}\n" +
            $"Final size: {size}",
            this
        );
    }

    // --------------------------------------------------
    // ALIGN MODEL BASE TO Y = 0
    // --------------------------------------------------

    public void AlignModelBase()
    {
        if (!FindChildren())
            return;

        float lowestY = float.PositiveInfinity;
        bool foundMesh = false;

        // MeshFilter meshes
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

        // Skinned meshes
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

        float offsetY = -lowestY;

#if UNITY_EDITOR
        Undo.RecordObjects(
            new UnityEngine.Object[]
            {
                rootModel,
                placementCollision
            },
            "Align Model Base"
        );
#endif

        // Preserve existing rotations.
        Quaternion existingRotation = rootModel.localRotation;

        rootModel.localRotation = existingRotation;
        placementCollision.localRotation = existingRotation;

        // Move both children together.
        Vector3 modelPosition = rootModel.localPosition;
        modelPosition.y += offsetY;
        rootModel.localPosition = modelPosition;

        Vector3 collisionPosition =
            placementCollision.localPosition;

        collisionPosition.y += offsetY;
        placementCollision.localPosition = collisionPosition;

        Physics.SyncTransforms();

        UpdateSize();

        SaveTransform(rootModel);
        SaveTransform(placementCollision);

        Debug.Log(
            $"Aligned: {name}\n" +
            $"Model rotation: {rootModel.localEulerAngles}\n" +
            $"Collision rotation: {placementCollision.localEulerAngles}\n" +
            $"Original lowest Y: {lowestY:F5}\n" +
            $"Applied Y offset: {offsetY:F5}",
            this
        );
    }

    // --------------------------------------------------
    // EDITOR SAVE HELPERS
    // --------------------------------------------------

    private void SaveAllTransforms()
    {
        SaveTransform(transform);
        SaveTransform(rootModel);
        SaveTransform(placementCollision);
    }

    private void SaveTransform(Transform target)
    {
        if (target == null)
            return;

#if UNITY_EDITOR
        EditorUtility.SetDirty(target);

        if (PrefabUtility.IsPartOfPrefabInstance(target))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(
                target
            );
        }
#endif
    }
}

// ======================================================
// CUSTOM INSPECTOR
// ======================================================

#if UNITY_EDITOR

[CustomEditor(typeof(ObjectSize))]
public class ObjectSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ObjectSize item = (ObjectSize)target;

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField(
            "Object Tools",
            EditorStyles.boldLabel
        );

        if (GUILayout.Button("Update Size", GUILayout.Height(30)))
        {
            Undo.RecordObject(item, "Update Object Size");

            item.UpdateSize();

            SaveChanges(item);
        }

        if (GUILayout.Button("Resize Object", GUILayout.Height(30)))
        {
            item.ResizeObject();

            SaveChanges(item);
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Bad Rotation (-90 X)", GUILayout.Height(30)))
        {
            item.UpdateRotation(true);

            SaveChanges(item);
        }

        if (GUILayout.Button("Normal Rotation (0 X)", GUILayout.Height(30)))
        {
            item.UpdateRotation(false);

            SaveChanges(item);
        }

        EditorGUILayout.Space(5);

        if (GUILayout.Button("Fix Model Base Y = 0", GUILayout.Height(30)))
        {
            item.AlignModelBase();

            SaveChanges(item);
        }
    }

    public static void SaveChanges(UnityEngine.Object obj)
    {
        if (obj == null)
            return;

        EditorUtility.SetDirty(obj);

        GameObject go = null;

        if (obj is GameObject gameObject)
            go = gameObject;
        else if (obj is Component component)
            go = component.gameObject;

        if (go == null)
            return;

        if (PrefabUtility.IsPartOfPrefabAsset(go))
        {
            // Find the root of the prefab asset
            GameObject root = go.transform.root.gameObject;

            PrefabUtility.SavePrefabAsset(root);
        }
        else if (PrefabUtility.IsPartOfPrefabInstance(go))
        {
            // Record changes as prefab overrides
            PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
        }

        // Mark scene as modified
        if (go.scene.IsValid() && go.scene.isLoaded &&
            !PrefabUtility.IsPartOfPrefabAsset(go))
        {
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                go.scene
            );
        }
    }
}

#endif
