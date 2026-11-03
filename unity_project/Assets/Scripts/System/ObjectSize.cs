using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[DisallowMultipleComponent]
[ExecuteAlways]
public class ObjectSize : MonoBehaviour
{
    public enum Dimension { Width, Height, Depth }

    [Header("Measured Object Size (Read Only)")]
    [SerializeField] private Vector3 size;
    [SerializeField] private Vector3 sizeCm;
    public Vector3 Size => size;
    public Vector3 SizeCm => sizeCm;

    [Header("Resize Settings")]
    public Dimension resizeBy = Dimension.Height;
    [Min(0.0001f)] public float targetSize = 0.035f;

    [Header("Target Base")]
    [Min(1f)] public float targetDiameterMM = 40f;
    [Header("Base Detection")]
    [Min(0.1f)] public float scanHeightMM = 5f;
    [Min(3)] public int minimumVertices = 12;
    [Header("Options")]
    public bool alignBottomToZero = true;


    [SerializeField] private float measuredDiameterMM;
    [SerializeField] private float lastScaleFactor;
    [SerializeField] private int sampledVertices;
    public float MeasuredDiameterMM => measuredDiameterMM;

    private Transform rootModel;
    private Transform placementCollision;

    private void Reset() => FindChildren();

    private void Awake()
    {
        if (FindChildren()) UpdateSize();
    }

    public bool FindChildren()
    {
        rootModel = transform.FindChildRecursive("RootModel");
        if (rootModel == null)
        {
            Debug.LogError("RootModelfound.", this);
            return false;
        }
        placementCollision = transform.FindChildRecursive("PlacementCollision");
        if (placementCollision == null)
        {
            Debug.LogError("PlacementCollision not found.", this);
            return false;
        }
        return true;
    }

    private void RecordChanges(string label)
    {
#if UNITY_EDITOR
        Undo.RecordObjects(new UnityEngine.Object[]
        {
            this, transform, rootModel, placementCollision
        }, label);
#endif
    }

    private void ApplyScale(float factor)
    {
        rootModel.localScale *= factor;
        rootModel.localPosition *= factor;
        placementCollision.localScale *= factor;
        placementCollision.localPosition *= factor;
    }

    // Public operations save themselves, so they also work when called from other editor code.
    public void UpdateRotation(bool badXRotation)
    {
        if (!FindChildren()) return;
        RecordChanges("Update Model Rotation");
        Quaternion rotation = Quaternion.Euler(badXRotation ? -90f : 0f, 0f, 0f);
        rootModel.localRotation = rotation;
        placementCollision.localRotation = rotation;
        FinishChanges();
    }

    public void UpdateSize()
    {
        if (!FindChildren())
        {
            size = Vector3.zero;
            sizeCm = Vector3.zero;
            return;
        }

        Renderer[] renderers = rootModel.GetComponentsInChildren<Renderer>(true);
        bool found = false;
        Bounds bounds = new Bounds();
        foreach (Renderer renderer in renderers)
        {
            if (!found) { bounds = renderer.bounds; found = true; }
            else bounds.Encapsulate(renderer.bounds);
        }
        size = found ? bounds.size : Vector3.zero;
        sizeCm = new Vector3(
            Mathf.Round(size.x * 1000f) / 10f,
            Mathf.Round(size.y * 1000f) / 10f,
            Mathf.Round(size.z * 1000f) / 10f);
    }

    private bool TryNormalizeParentScale()
    {
        Vector3 parentScale = transform.localScale;
        if (Mathf.Abs(parentScale.x) < 0.000001f ||
            Mathf.Abs(parentScale.y) < 0.000001f ||
            Mathf.Abs(parentScale.z) < 0.000001f ||
            !Mathf.Approximately(parentScale.x, parentScale.y) ||
            !Mathf.Approximately(parentScale.y, parentScale.z))
        {
            Debug.LogWarning("Parent scale must be non-zero and uniform.", this);
            return false;
        }
        ApplyScale(parentScale.x);
        transform.localScale = Vector3.one;
        return true;
    }

    public void ResizeObject()
    {
        if (!FindChildren() || targetSize <= 0f) return;
        Vector3 s = transform.localScale;
        if (Mathf.Abs(s.x) < 0.000001f ||
            !Mathf.Approximately(s.x, s.y) ||
            !Mathf.Approximately(s.y, s.z))
        {
            Debug.LogWarning("Parent scale must be non-zero and uniform.", this);
            return;
        }
        UpdateSize();
        float current = resizeBy == Dimension.Width ? size.x :
                        resizeBy == Dimension.Height ? size.y : size.z;
        if (current <= 0.000001f)
        {
            Debug.LogWarning("Selected dimension is zero.", this);
            return;
        }
        RecordChanges("Resize Object");
        // Current size already includes the parent's scale.
        if (!TryNormalizeParentScale()) return;
        float factor = targetSize / current;
        ApplyScale(factor);
        FinishChanges();
        Debug.Log($"Resized {name}: multiplier {factor:F5}, final size {size}", this);
    }

    private bool TryGetLowestY(out float lowestY)
    {
        lowestY = float.PositiveInfinity;
        bool found = false;
        foreach (MeshFilter filter in rootModel.GetComponentsInChildren<MeshFilter>(true))
        {
            Mesh mesh = filter.sharedMesh;
            if (mesh == null) continue;
            Vector3[] vertices;
            try { vertices = mesh.vertices; }
            catch (Exception)
            {
                Debug.LogWarning($"Cannot read mesh {mesh.name}. Enable Read/Write.", filter);
                continue;
            }
            Matrix4x4 matrix = transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            foreach (Vector3 vertex in vertices)
            {
                lowestY = Mathf.Min(lowestY, matrix.MultiplyPoint3x4(vertex).y);
                found = true;
            }
        }
        foreach (SkinnedMeshRenderer renderer in rootModel.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {
            Bounds b = renderer.localBounds;
            Matrix4x4 matrix = transform.worldToLocalMatrix * renderer.transform.localToWorldMatrix;
            for (int x = -1; x <= 1; x += 2)
                for (int y = -1; y <= 1; y += 2)
                    for (int z = -1; z <= 1; z += 2)
                    {
                        Vector3 corner = b.center + Vector3.Scale(b.extents, new Vector3(x, y, z));
                        lowestY = Mathf.Min(lowestY, matrix.MultiplyPoint3x4(corner).y);
                        found = true;
                    }
        }
        return found;
    }

    private void OffsetChildrenY(float offset)
    {
        rootModel.localPosition += Vector3.up * offset;
        placementCollision.localPosition += Vector3.up * offset;
    }

    public void AlignModelBase()
    {
        if (!FindChildren() || !TryGetLowestY(out float bottom))
        {
            Debug.LogWarning("Unable to find readable model geometry.", this);
            return;
        }
        RecordChanges("Align Model Base");
        OffsetChildrenY(-bottom);
        FinishChanges();
    }

    private bool TryMeasure(out float diameter, out float bottomY, out int count)
    {
        diameter = 0f;
        bottomY = 0f;
        count = 0;
        if (!FindChildren() || !TryGetLowestY(out bottomY)) return false;

        float maxY = bottomY + scanHeightMM / 1000f;
        float minX = float.PositiveInfinity, maxX = float.NegativeInfinity;
        float minZ = float.PositiveInfinity, maxZ = float.NegativeInfinity;
        foreach (MeshFilter filter in rootModel.GetComponentsInChildren<MeshFilter>(true))
        {
            Mesh mesh = filter.sharedMesh;
            if (mesh == null) continue;
            Vector3[] vertices;
            try { vertices = mesh.vertices; }
            catch (Exception) { continue; }
            Matrix4x4 matrix = transform.worldToLocalMatrix * filter.transform.localToWorldMatrix;
            foreach (Vector3 vertex in vertices)
            {
                Vector3 p = matrix.MultiplyPoint3x4(vertex);
                if (p.y > maxY) continue;
                minX = Mathf.Min(minX, p.x);
                maxX = Mathf.Max(maxX, p.x);
                minZ = Mathf.Min(minZ, p.z);
                maxZ = Mathf.Max(maxZ, p.z);
                count++;
            }
        }
        if (count < minimumVertices) return false;
        diameter = Mathf.Max(maxX - minX, maxZ - minZ);
        return diameter > 0.000001f;
    }

    public void MeasureBase()
    {
        if (!TryMeasure(out float diameter, out float bottom, out int count))
        {
            Debug.LogWarning("Unable to measure base. Check mesh Read/Write and scan height.", this);
            return;
        }
        RecordChanges("Measure Base");
        measuredDiameterMM = diameter * 1000f;
        sampledVertices = count;
        FinishChanges();
        Debug.Log($"Base diameter: {measuredDiameterMM:F2} mm, bottom Y: {bottom:F5}, vertices: {count}", this);
    }

    public void ScaleToTarget()
    {
        if (!FindChildren() || targetDiameterMM <= 0f) return;
        if (!Mathf.Approximately(transform.localScale.x, 1f) ||
            !Mathf.Approximately(transform.localScale.y, 1f) ||
            !Mathf.Approximately(transform.localScale.z, 1f))
        {
            Debug.LogError("Parent scale must be (1,1,1) before scaling base.", this);
            return;
        }
        if (!TryMeasure(out float diameter, out _, out _))
        {
            Debug.LogWarning("Unable to measure base.", this);
            return;
        }
        float factor = targetDiameterMM / (diameter * 1000f);
        if (float.IsNaN(factor) || float.IsInfinity(factor) || factor <= 0f) return;
        RecordChanges("Scale Miniature Base");
        ApplyScale(factor);
        Physics.SyncTransforms();
        if (TryMeasure(out float newDiameter, out float bottom, out int count))
        {
            if (alignBottomToZero) OffsetChildrenY(-bottom);
            measuredDiameterMM = newDiameter * 1000f;
            sampledVertices = count;
        }
        lastScaleFactor = factor;
        FinishChanges();
        Debug.Log($"Scaled base to {targetDiameterMM:F2} mm, factor {factor:F5}", this);
    }

    private void FinishChanges()
    {
        Physics.SyncTransforms();
        UpdateSize();
#if UNITY_EDITOR
        SaveEverything();
#endif
    }
#if UNITY_EDITOR
    public void SaveEverything()
    {
        if (!FindChildren())
            return;

        Physics.SyncTransforms();
        UpdateSize();

        UnityEngine.Object[] changedObjects =
        {
        this,
        transform,
        rootModel,
        placementCollision
    };

        // Mark every modified object as dirty.
        foreach (UnityEngine.Object obj in changedObjects)
        {
            if (obj == null)
                continue;

            EditorUtility.SetDirty(obj);

            if (PrefabUtility.IsPartOfPrefabInstance(obj))
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(obj);
            }
        }

        GameObject prefabRoot =
            PrefabUtility.GetNearestPrefabInstanceRoot(gameObject);

        // CASE 1: Editing an instance inside a scene.
        if (prefabRoot != null &&
            !PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            string prefabPath =
                PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(
                    prefabRoot
                );

            if (!string.IsNullOrEmpty(prefabPath))
            {
                // Apply the modified components and transforms
                // to the source prefab.
                foreach (UnityEngine.Object obj in changedObjects)
                {
                    if (obj == null)
                        continue;

                    if (!PrefabUtility.IsPartOfPrefabInstance(obj))
                        continue;

                    PrefabUtility.ApplyObjectOverride(
                        obj,
                        prefabPath,
                        InteractionMode.AutomatedAction
                    );
                }

                AssetDatabase.SaveAssets();

                Debug.Log(
                    "Applied changes to prefab: " + prefabPath,
                    this
                );
            }
        }
        // CASE 2: Editing the prefab asset directly.
        else if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            GameObject assetRoot = transform.root.gameObject;

            PrefabUtility.SavePrefabAsset(assetRoot);
            AssetDatabase.SaveAssets();

            Debug.Log(
                "Saved prefab asset: " + assetRoot.name,
                this
            );
        }

        // CASE 3: Save changes to the scene.
        if (gameObject.scene.IsValid() &&
            gameObject.scene.isLoaded &&
            !PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);

            if (!string.IsNullOrEmpty(gameObject.scene.path))
            {
                EditorSceneManager.SaveScene(gameObject.scene);
            }
            else
            {
                Debug.LogWarning(
                    "Scene has not been saved to disk yet.",
                    this
                );
            }
        }

        Debug.Log("ObjectSize changes saved.", this);
    }


    private void OnDrawGizmosSelected()
    {
        if (!FindChildren()) return;
        Renderer[] renderers = rootModel.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) return;
        Bounds b = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
        Vector3 centre = new Vector3(b.center.x, b.min.y + scanHeightMM / 1000f, b.center.z);
        float half = Mathf.Max(b.size.x, b.size.z) * 0.75f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(centre + Vector3.left * half, centre + Vector3.right * half);
        Gizmos.DrawLine(centre + Vector3.back * half, centre + Vector3.forward * half);
        Handles.Label(centre + Vector3.up * 0.005f, $"Scan Height: {scanHeightMM:F1} mm");
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ObjectSize))]
public class ObjectSizeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script", "size", "sizeCm",
            "measuredDiameterMM", "lastScaleFactor", "sampledVertices");
        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Measured Results (Read Only)", EditorStyles.boldLabel);
        using (new EditorGUI.DisabledScope(true))
        {
            foreach (string property in new[] { "size", "sizeCm", "measuredDiameterMM", "lastScaleFactor", "sampledVertices" })
                EditorGUILayout.PropertyField(serializedObject.FindProperty(property));
        }

        ObjectSize item = (ObjectSize)target;
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Object Tools", EditorStyles.boldLabel);
        if (GUILayout.Button("Update Size", GUILayout.Height(30))) { Undo.RecordObject(item, "Update Size"); item.UpdateSize(); item.SaveEverything(); }
        if (GUILayout.Button("Resize Object", GUILayout.Height(30))) item.ResizeObject();
        if (GUILayout.Button("Bad Rotation (-90 X)", GUILayout.Height(30))) item.UpdateRotation(true);
        if (GUILayout.Button("Normal Rotation (0 X)", GUILayout.Height(30))) item.UpdateRotation(false);
        if (GUILayout.Button("Fix Model Base Y = 0", GUILayout.Height(30))) item.AlignModelBase();
        EditorGUILayout.Space(15);
        if (GUILayout.Button("Measure Current Base", GUILayout.Height(30))) item.MeasureBase();
        if (GUILayout.Button("Scale Base To Target Size", GUILayout.Height(36))) item.ScaleToTarget();
    }
}
#endif
