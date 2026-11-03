
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.SceneManagement;
#endif

[DisallowMultipleComponent]
public class TerrainBoxColliderGenerator : MonoBehaviour
{
    [Header("Child Objects")]
    public Transform rootModel;

    [Header("Voxel Grid")]
    [Min(4)]
    public int longestAxisCells = 96;

    [Range(0, 2)]
    public int surfacePaddingCells = 0;

    [Min(1)]
    public int maxColliders = 2000;

    [Min(1000)]
    public int maxVoxelCount = 1500000;

    [Header("Output")]
    public Transform colliderContainer;

    private const string ContainerName =
        "Generated Box Colliders";

    private void Reset()
    {
        FindChildren();
    }

    public void FindChildren()
    {
        if (rootModel == null)
            rootModel = transform.Find("RootModel");
    }

    private MeshFilter FindMeshFilter()
    {
        FindChildren();

        if (rootModel == null)
        {
            Debug.LogError("RootModel not found.", this);
            return null;
        }

        MeshFilter[] filters =
            rootModel.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter filter in filters)
        {
            if (filter.sharedMesh != null)
                return filter;
        }

        Debug.LogError(
            "No MeshFilter found under RootModel.", this);

        return null;
    }

    // ====================================================
    // REMOVE COLLIDERS
    // ====================================================

    public void RemoveColliders()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning(
                "Cannot permanently remove colliders in Play Mode.",
                this);
            return;
        }

        RemoveCollidersInternal();

        SaveChanges();
    }

    private void RemoveCollidersInternal()
    {
        MeshFilter filter = FindMeshFilter();

        if (filter == null)
            return;

        Transform existing =
            filter.transform.Find(ContainerName);

        if (existing != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Undo.DestroyObjectImmediate(existing.gameObject);
            else
#endif
                Destroy(existing.gameObject);
        }

        colliderContainer = null;
    }

    // ====================================================
    // GENERATE COLLIDERS
    // ====================================================

    public void GenerateColliders()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning(
                "Cannot permanently generate colliders in Play Mode.",
                this);
            return;
        }

        MeshFilter primaryFilter = FindMeshFilter();

        if (primaryFilter == null)
            return;

        Transform meshTransform =
            primaryFilter.transform;

        MeshFilter[] filters =
            rootModel.GetComponentsInChildren<MeshFilter>(true);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        Matrix4x4 destinationInverse =
            meshTransform.worldToLocalMatrix;

        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = filter.sharedMesh;

            if (mesh == null)
                continue;

            if (!mesh.isReadable)
            {
                Debug.LogError(
                    $"Mesh '{mesh.name}' is unreadable. " +
                    "Enable Read/Write in FBX import settings.",
                    filter);
                return;
            }

            Matrix4x4 matrix =
                destinationInverse *
                filter.transform.localToWorldMatrix;

            Vector3[] meshVertices = mesh.vertices;
            int[] meshTriangles = mesh.triangles;

            int offset = vertices.Count;

            foreach (Vector3 vertex in meshVertices)
            {
                vertices.Add(
                    matrix.MultiplyPoint3x4(vertex));
            }

            foreach (int index in meshTriangles)
                triangles.Add(offset + index);
        }

        if (vertices.Count == 0 ||
            triangles.Count == 0)
        {
            Debug.LogError(
                "Mesh contains no triangle geometry.",
                this);
            return;
        }

        Bounds bounds =
            new Bounds(vertices[0], Vector3.zero);

        foreach (Vector3 vertex in vertices)
            bounds.Encapsulate(vertex);

        float longest = Mathf.Max(
            bounds.size.x,
            Mathf.Max(bounds.size.y, bounds.size.z));

        if (longest <= 0f)
            return;

        float cell =
            longest / Mathf.Max(4, longestAxisCells);

        Vector3Int dims = new Vector3Int(
            Mathf.Max(1,
                Mathf.CeilToInt(bounds.size.x / cell)),
            Mathf.Max(1,
                Mathf.CeilToInt(bounds.size.y / cell)),
            Mathf.Max(1,
                Mathf.CeilToInt(bounds.size.z / cell))
        );

        long total =
            (long)dims.x * dims.y * dims.z;

        if (total > maxVoxelCount)
        {
            Debug.LogError(
                $"Grid requires {total:N0} cells " +
                $"(limit {maxVoxelCount:N0}). " +
                "Reduce Longest Axis Cells.",
                this);
            return;
        }

        bool[,,] solid =
            new bool[dims.x, dims.y, dims.z];

        int touched = 0;

        Vector3 half =
            Vector3.one * (cell * 0.5f);

        try
        {
            for (int t = 0; t < triangles.Count; t += 3)
            {
                Vector3 a = vertices[triangles[t]];
                Vector3 b = vertices[triangles[t + 1]];
                Vector3 c = vertices[triangles[t + 2]];

                Vector3 low =
                    Vector3.Min(a, Vector3.Min(b, c));

                Vector3 high =
                    Vector3.Max(a, Vector3.Max(b, c));

                int x0 = Mathf.Clamp(
                    Mathf.FloorToInt(
                        (low.x - bounds.min.x) / cell)
                    - surfacePaddingCells,
                    0, dims.x - 1);

                int y0 = Mathf.Clamp(
                    Mathf.FloorToInt(
                        (low.y - bounds.min.y) / cell)
                    - surfacePaddingCells,
                    0, dims.y - 1);

                int z0 = Mathf.Clamp(
                    Mathf.FloorToInt(
                        (low.z - bounds.min.z) / cell)
                    - surfacePaddingCells,
                    0, dims.z - 1);

                int x1 = Mathf.Clamp(
                    Mathf.FloorToInt(
                        (high.x - bounds.min.x) / cell)
                    + surfacePaddingCells,
                    0, dims.x - 1);

                int y1 = Mathf.Clamp(
                    Mathf.FloorToInt(
                        (high.y - bounds.min.y) / cell)
                    + surfacePaddingCells,
                    0, dims.y - 1);

                int z1 = Mathf.Clamp(
                    Mathf.FloorToInt(
                        (high.z - bounds.min.z) / cell)
                    + surfacePaddingCells,
                    0, dims.z - 1);

                for (int x = x0; x <= x1; x++)
                {
                    for (int y = y0; y <= y1; y++)
                    {
                        for (int z = z0; z <= z1; z++)
                        {
                            if (solid[x, y, z])
                                continue;

                            Vector3 center =
                                bounds.min +
                                new Vector3(
                                    (x + 0.5f) * cell,
                                    (y + 0.5f) * cell,
                                    (z + 0.5f) * cell);

                            if (TriangleIntersectsBox(
                                center, half, a, b, c))
                            {
                                solid[x, y, z] = true;
                                touched++;
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
            return;
        }

        // =================================================
        // GREEDY MERGE
        // =================================================

        List<Bounds> boxes = new List<Bounds>();

        for (int y = 0; y < dims.y; y++)
        {
            for (int z = 0; z < dims.z; z++)
            {
                for (int x = 0; x < dims.x; x++)
                {
                    if (!solid[x, y, z])
                        continue;

                    int xe = x + 1;

                    while (xe < dims.x && solid[xe, y, z])
                        xe++;

                    int ze = z + 1;

                    while (ze < dims.z)
                    {
                        bool ok = true;

                        for (int ix = x; ix < xe; ix++)
                        {
                            if (!solid[ix, y, ze])
                            {
                                ok = false;
                                break;
                            }
                        }

                        if (!ok)
                            break;

                        ze++;
                    }

                    int ye = y + 1;

                    while (ye < dims.y)
                    {
                        bool ok = true;

                        for (int iz = z; iz < ze && ok; iz++)
                        {
                            for (int ix = x; ix < xe; ix++)
                            {
                                if (!solid[ix, ye, iz])
                                {
                                    ok = false;
                                    break;
                                }
                            }
                        }

                        if (!ok)
                            break;

                        ye++;
                    }

                    for (int iy = y; iy < ye; iy++)
                    {
                        for (int iz = z; iz < ze; iz++)
                        {
                            for (int ix = x; ix < xe; ix++)
                                solid[ix, iy, iz] = false;
                        }
                    }

                    Vector3 start =
                        bounds.min +
                        new Vector3(x, y, z) * cell;

                    Vector3 end = Vector3.Min(
                        bounds.max,
                        bounds.min +
                        new Vector3(xe, ye, ze) * cell);

                    if (end.x > start.x &&
                        end.y > start.y &&
                        end.z > start.z)
                    {
                        boxes.Add(new Bounds(
                            (start + end) * 0.5f,
                            end - start));
                    }

                    if (boxes.Count > maxColliders)
                    {
                        Debug.LogWarning(
                            $"Would generate over {maxColliders} " +
                            "colliders. Existing colliders unchanged.",
                            this);
                        return;
                    }
                }
            }
        }

        // =================================================
        // CREATE COLLIDERS
        // =================================================

#if UNITY_EDITOR
        Undo.SetCurrentGroupName("Generate Terrain Colliders");
        int undoGroup = Undo.GetCurrentGroup();
#endif

        RemoveCollidersInternal();

        GameObject container =
            new GameObject(ContainerName);

#if UNITY_EDITOR
        Undo.RegisterCreatedObjectUndo(
            container, "Create Collider Container");
#endif

        container.transform.SetParent(
            meshTransform, false);

        container.transform.localPosition =
            Vector3.zero;

        container.transform.localRotation =
            Quaternion.identity;

        container.transform.localScale =
            Vector3.one;

        colliderContainer = container.transform;

        foreach (Bounds box in boxes)
        {
#if UNITY_EDITOR
            BoxCollider bc =
                Undo.AddComponent<BoxCollider>(container);
#else
            BoxCollider bc =
                container.AddComponent<BoxCollider>();
#endif

            bc.center = box.center;
            bc.size = box.size;
        }

#if UNITY_EDITOR
        Undo.CollapseUndoOperations(undoGroup);
#endif

        SaveChanges();

        Debug.Log(
            $"Generated {boxes.Count} BoxColliders " +
            $"from {touched} surface cells " +
            $"({dims.x}x{dims.y}x{dims.z}).",
            this);
    }

    // ====================================================
    // SAVE PREFAB
    // ====================================================


    private void SaveChanges()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
            return;

        EditorUtility.SetDirty(this);

        // 1. Editing the prefab directly in Prefab Mode.
        var prefabStage =
            PrefabStageUtility.GetCurrentPrefabStage();

        if (prefabStage != null &&
            gameObject.scene == prefabStage.scene)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);

            GameObject savedPrefab =
                PrefabUtility.SaveAsPrefabAsset(
                    prefabStage.prefabContentsRoot,
                    prefabStage.assetPath);

            if (savedPrefab == null)
            {
                Debug.LogError("Failed to save prefab.", this);
                return;
            }

            AssetDatabase.SaveAssets();

            Debug.Log("Saved prefab in Prefab Mode.", this);
            return;
        }

        // 2. Editing a prefab asset directly.
        if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
        {
            GameObject prefabRoot =
                transform.root.gameObject;

            bool success;

            PrefabUtility.SavePrefabAsset(
                prefabRoot,
                out success);

            if (!success)
            {
                Debug.LogError("Failed to save prefab asset.", this);
                return;
            }

            AssetDatabase.SaveAssets();

            Debug.Log("Saved prefab asset.", this);
            return;
        }

        // 3. Editing a prefab instance inside a scene.
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            GameObject prefabRoot =
                PrefabUtility.GetOutermostPrefabInstanceRoot(
                    gameObject);

            if (prefabRoot != null)
            {
                PrefabUtility.RecordPrefabInstancePropertyModifications(
                    this);

                // Applies added/removed children and colliders
                // to the original prefab asset.
                PrefabUtility.ApplyPrefabInstance(
                    prefabRoot,
                    InteractionMode.AutomatedAction);

                AssetDatabase.SaveAssets();

                Debug.Log(
                    "Applied scene instance changes to original prefab.",
                    this);
            }
        }

        // 4. Save the scene containing this object.
        if (gameObject.scene.IsValid() &&
            gameObject.scene.isLoaded)
        {
            EditorSceneManager.MarkSceneDirty(gameObject.scene);

            if (string.IsNullOrEmpty(gameObject.scene.path))
            {
                Debug.LogWarning(
                    "Scene has not been saved to disk yet.",
                    this);
            }
            else
            {
                EditorSceneManager.SaveScene(gameObject.scene);
            }
        }
#endif
    }


    // ====================================================
    // TRIANGLE / BOX INTERSECTION
    // ====================================================

    private static bool TriangleIntersectsBox(
        Vector3 center,
        Vector3 half,
        Vector3 a,
        Vector3 b,
        Vector3 c)
    {
        a -= center;
        b -= center;
        c -= center;

        Vector3 e0 = b - a;
        Vector3 e1 = c - b;
        Vector3 e2 = a - c;

        Vector3[] edges = { e0, e1, e2 };

        foreach (Vector3 edge in edges)
        {
            Vector3[] axes =
            {
                Vector3.Cross(edge, Vector3.right),
                Vector3.Cross(edge, Vector3.up),
                Vector3.Cross(edge, Vector3.forward)
            };

            foreach (Vector3 axis in axes)
            {
                if (axis.sqrMagnitude < 1e-12f)
                    continue;

                float p0 = Vector3.Dot(a, axis);
                float p1 = Vector3.Dot(b, axis);
                float p2 = Vector3.Dot(c, axis);

                float r =
                    half.x * Mathf.Abs(axis.x) +
                    half.y * Mathf.Abs(axis.y) +
                    half.z * Mathf.Abs(axis.z);

                if (Mathf.Min(p0, Mathf.Min(p1, p2)) > r ||
                    Mathf.Max(p0, Mathf.Max(p1, p2)) < -r)
                    return false;
            }
        }

        for (int axis = 0; axis < 3; axis++)
        {
            float min = Mathf.Min(
                a[axis],
                Mathf.Min(b[axis], c[axis]));

            float max = Mathf.Max(
                a[axis],
                Mathf.Max(b[axis], c[axis]));

            if (min > half[axis] ||
                max < -half[axis])
                return false;
        }

        Vector3 normal = Vector3.Cross(e0, e1);
        float dist = Vector3.Dot(normal, a);

        float radius =
            half.x * Mathf.Abs(normal.x) +
            half.y * Mathf.Abs(normal.y) +
            half.z * Mathf.Abs(normal.z);

        return Mathf.Abs(dist) <= radius;
    }
}
