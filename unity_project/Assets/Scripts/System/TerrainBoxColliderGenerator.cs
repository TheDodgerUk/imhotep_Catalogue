
using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TerrainBoxColliderGenerator : MonoBehaviour
{
    [Header("Child Objects")]
    public Transform rootModel;

    [Header("Voxel grid (local mesh units)")]
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
            Debug.LogError(
                "RootModel not found.", this);
            return null;
        }

        MeshFilter[] filters =
            rootModel.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter filter in filters)
        {
            if (filter.sharedMesh != null)
                return filter;
        }

        return null;
    }

    public void RemoveColliders()
    {
        MeshFilter filter = FindMeshFilter();

        if (filter == null)
            return;

        Transform existing =
            filter.transform.Find(ContainerName);

        if (existing != null)
        {
            if (Application.isPlaying)
                Destroy(existing.gameObject);
            else
                DestroyImmediate(existing.gameObject);
        }

        colliderContainer = null;
    }

    public void GenerateColliders()
    {
        MeshFilter primaryFilter = FindMeshFilter();

        if (primaryFilter == null)
        {
            Debug.LogError(
                "No MeshFilter found under RootModel.",
                this);
            return;
        }

        Transform meshTransform =
            primaryFilter.transform;

        MeshFilter[] filters =
            rootModel.GetComponentsInChildren<MeshFilter>(true);

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        // Transform all mesh geometry into the
        // primary mesh GameObject's local space.
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
                    for (int y = y0; y <= y1; y++)
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
        catch (Exception ex)
        {
            Debug.LogException(ex, this);
            return;
        }

        // Greedy merge adjacent occupied voxels.
        // Build the plan before modifying existing colliders.
        List<Bounds> boxes = new List<Bounds>();

        for (int y = 0; y < dims.y; y++)
            for (int z = 0; z < dims.z; z++)
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
                            for (int ix = x; ix < xe; ix++)
                            {
                                if (!solid[ix, ye, iz])
                                {
                                    ok = false;
                                    break;
                                }
                            }

                        if (!ok)
                            break;

                        ye++;
                    }

                    for (int iy = y; iy < ye; iy++)
                        for (int iz = z; iz < ze; iz++)
                            for (int ix = x; ix < xe; ix++)
                                solid[ix, iy, iz] = false;

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
                            "colliders. Increase Max Colliders or " +
                            "reduce Longest Axis Cells. " +
                            "Existing colliders unchanged.",
                            this);
                        return;
                    }
                }

        // Replace colliders only after successful generation.
        RemoveColliders();

        GameObject container =
            new GameObject(ContainerName);

        // Parent directly under the mesh GameObject.
        container.transform.SetParent(
            meshTransform, false);

        container.transform.localPosition =
            Vector3.zero;

        container.transform.localRotation =
            Quaternion.identity;

        container.transform.localScale =
            Vector3.one;

        colliderContainer = container.transform;

        // Bounds are already in meshTransform local space.
        foreach (Bounds box in boxes)
        {
            BoxCollider bc =
                container.AddComponent<BoxCollider>();

            bc.center = box.center;
            bc.size = box.size;
        }

        Debug.Log(
            $"Generated {boxes.Count} BoxColliders " +
            $"from {touched} surface cells " +
            $"({dims.x}x{dims.y}x{dims.z}).",
            this);
    }

    // Separating Axis Theorem triangle/AABB overlap.
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
