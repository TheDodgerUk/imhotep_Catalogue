using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter))]
public class TerrainBoxColliderGenerator : MonoBehaviour
{
    [Header("Voxel grid (local mesh units)")]
    [Min(4)] public int longestAxisCells = 96;
    [Range(0, 2)] public int surfacePaddingCells = 0;
    [Min(1)] public int maxColliders = 2000;
    [Min(1000)] public int maxVoxelCount = 1500000;
    [Header("Output")]
    public Transform colliderContainer;

    public void RemoveColliders()
    {
        Transform existing = transform.Find("Generated Box Colliders");
        if (existing != null) DestroyImmediate(existing.gameObject);
        colliderContainer = null;
    }

    public void GenerateColliders()
    {
        MeshFilter filter = GetComponent<MeshFilter>();
        Mesh mesh = filter != null ? filter.sharedMesh : null;
        if (mesh == null || !mesh.isReadable)
        {
            Debug.LogError("Missing or unreadable mesh. Enable Read/Write in the FBX import settings.", this);
            return;
        }

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;
        if (vertices.Length == 0 || triangles.Length == 0)
        {
            Debug.LogError("Mesh contains no triangle geometry.", this);
            return;
        }

        Bounds bounds = mesh.bounds;
        float longest = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y, bounds.size.z));
        if (longest <= 0f) return;
        float cell = longest / Mathf.Max(4, longestAxisCells);
        Vector3Int dims = new Vector3Int(
            Mathf.Max(1, Mathf.CeilToInt(bounds.size.x / cell)),
            Mathf.Max(1, Mathf.CeilToInt(bounds.size.y / cell)),
            Mathf.Max(1, Mathf.CeilToInt(bounds.size.z / cell)));
        long total = (long)dims.x * dims.y * dims.z;
        if (total > maxVoxelCount)
        {
            Debug.LogError($"Grid requires {total:N0} cells (limit {maxVoxelCount:N0}). Reduce Longest Axis Cells.", this);
            return;
        }

        bool[,,] solid = new bool[dims.x, dims.y, dims.z];
        int touched = 0;
        try
        {
            for (int t = 0; t < triangles.Length; t += 3)
            {
                Vector3 a = vertices[triangles[t]];
                Vector3 b = vertices[triangles[t + 1]];
                Vector3 c = vertices[triangles[t + 2]];
                Vector3 low = Vector3.Min(a, Vector3.Min(b, c));
                Vector3 high = Vector3.Max(a, Vector3.Max(b, c));
                int x0 = Mathf.Clamp(Mathf.FloorToInt((low.x - bounds.min.x) / cell) - surfacePaddingCells, 0, dims.x - 1);
                int y0 = Mathf.Clamp(Mathf.FloorToInt((low.y - bounds.min.y) / cell) - surfacePaddingCells, 0, dims.y - 1);
                int z0 = Mathf.Clamp(Mathf.FloorToInt((low.z - bounds.min.z) / cell) - surfacePaddingCells, 0, dims.z - 1);
                int x1 = Mathf.Clamp(Mathf.FloorToInt((high.x - bounds.min.x) / cell) + surfacePaddingCells, 0, dims.x - 1);
                int y1 = Mathf.Clamp(Mathf.FloorToInt((high.y - bounds.min.y) / cell) + surfacePaddingCells, 0, dims.y - 1);
                int z1 = Mathf.Clamp(Mathf.FloorToInt((high.z - bounds.min.z) / cell) + surfacePaddingCells, 0, dims.z - 1);
                for (int x = x0; x <= x1; x++)
                    for (int y = y0; y <= y1; y++)
                        for (int z = z0; z <= z1; z++)
                        {
                            if (solid[x, y, z]) continue;
                            Vector3 center = bounds.min + new Vector3((x + .5f) * cell, (y + .5f) * cell, (z + .5f) * cell);
                            if (TriangleIntersectsBox(center, new Vector3(cell, cell, cell) * .5f, a, b, c))
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

        // Greedy merge adjacent occupied cells into axis-aligned rectangular boxes.
        // Precompute the plan before changing scene objects so a collider limit failure is safe.
        var boxes = new List<Bounds>();
        for (int y = 0; y < dims.y; y++)
            for (int z = 0; z < dims.z; z++)
                for (int x = 0; x < dims.x; x++)
                {
                    if (!solid[x, y, z]) continue;
                    int xe = x + 1;
                    while (xe < dims.x && solid[xe, y, z]) xe++;
                    int ze = z + 1;
                    while (ze < dims.z)
                    {
                        bool ok = true;
                        for (int ix = x; ix < xe; ix++) if (!solid[ix, y, ze]) { ok = false; break; }
                        if (!ok) break;
                        ze++;
                    }
                    int ye = y + 1;
                    while (ye < dims.y)
                    {
                        bool ok = true;
                        for (int iz = z; iz < ze && ok; iz++)
                            for (int ix = x; ix < xe; ix++) if (!solid[ix, ye, iz]) { ok = false; break; }
                        if (!ok) break;
                        ye++;
                    }
                    for (int iy = y; iy < ye; iy++)
                        for (int iz = z; iz < ze; iz++)
                            for (int ix = x; ix < xe; ix++) solid[ix, iy, iz] = false;
                    Vector3 start = bounds.min + new Vector3(x, y, z) * cell;
                    Vector3 end = Vector3.Min(bounds.max, bounds.min + new Vector3(xe, ye, ze) * cell);
                    if (end.x > start.x && end.y > start.y && end.z > start.z)
                        boxes.Add(new Bounds((start + end) * .5f, end - start));
                    if (boxes.Count > maxColliders)
                    {
                        Debug.LogWarning($"Would generate over {maxColliders} colliders. Increase Max Colliders or reduce Longest Axis Cells. Existing colliders unchanged.", this);
                        return;
                    }
                }

        RemoveColliders();
        GameObject root = new GameObject("Generated Box Colliders");
        root.transform.SetParent(transform, false);
        colliderContainer = root.transform;
        foreach (Bounds b in boxes)
        {
            // Colliders on this child share the original mesh's local coordinate system.
            BoxCollider bc = root.AddComponent<BoxCollider>();
            bc.center = b.center;
            bc.size = b.size;
        }
        Debug.Log($"Generated {boxes.Count} BoxColliders from {touched} surface cells ({dims.x}x{dims.y}x{dims.z}).", this);
    }

    // Separating axis theorem triangle/AABB overlap, in box-centred coordinates.
    private static bool TriangleIntersectsBox(Vector3 center, Vector3 half, Vector3 a, Vector3 b, Vector3 c)
    {
        a -= center; b -= center; c -= center;
        Vector3 e0 = b - a, e1 = c - b, e2 = a - c;
        Vector3[] edges = { e0, e1, e2 };
        foreach (Vector3 edge in edges)
        {
            Vector3[] axes = { Vector3.Cross(edge, Vector3.right), Vector3.Cross(edge, Vector3.up), Vector3.Cross(edge, Vector3.forward) };
            foreach (Vector3 axis in axes)
            {
                if (axis.sqrMagnitude < 1e-12f) continue;
                float p0 = Vector3.Dot(a, axis), p1 = Vector3.Dot(b, axis), p2 = Vector3.Dot(c, axis);
                float r = half.x * Mathf.Abs(axis.x) + half.y * Mathf.Abs(axis.y) + half.z * Mathf.Abs(axis.z);
                if (Mathf.Min(p0, Mathf.Min(p1, p2)) > r || Mathf.Max(p0, Mathf.Max(p1, p2)) < -r) return false;
            }
        }
        for (int axis = 0; axis < 3; axis++)
        {
            float min = Mathf.Min(a[axis], Mathf.Min(b[axis], c[axis]));
            float max = Mathf.Max(a[axis], Mathf.Max(b[axis], c[axis]));
            if (min > half[axis] || max < -half[axis]) return false;
        }
        Vector3 normal = Vector3.Cross(e0, e1);
        float dist = Vector3.Dot(normal, a);
        float radius = half.x * Mathf.Abs(normal.x) + half.y * Mathf.Abs(normal.y) + half.z * Mathf.Abs(normal.z);
        return Mathf.Abs(dist) <= radius;
    }
}
