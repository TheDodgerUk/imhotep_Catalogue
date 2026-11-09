using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// Attach to an empty GameObject. The custom inspector below lives in this file.
public class MeshPolygonReducer : MonoBehaviour
{
    [Header("Source")]
    public GameObject sourceModel;

    [Header("Simplification")]
    [Range(0.05f, 1f)] public float quality = 0.25f;
    [Min(0)] public int maximumCollapses = 200000;

    [Header("Results")]
    [SerializeField] private int originalTriangles;
    [SerializeField] private int reducedTriangles;

    public void CountTriangles()
    {
        originalTriangles = 0;
        if (sourceModel == null) return;
        foreach (MeshFilter filter in sourceModel.GetComponentsInChildren<MeshFilter>(true))
            if (filter.sharedMesh != null) originalTriangles += TriangleCount(filter.sharedMesh);
    }

    private static int TriangleCount(Mesh mesh)
    {
        int count = 0;
        for (int s = 0; s < mesh.subMeshCount; s++)
            if (mesh.GetTopology(s) == MeshTopology.Triangles)
                count += (int)(mesh.GetIndexCount(s) / 3);
        return count;
    }

#if UNITY_EDITOR
    public void ReducePolygons()
    {
        if (sourceModel == null)
        {
            EditorUtility.DisplayDialog("Missing model", "Assign Source Model first.", "OK");
            return;
        }

        CountTriangles();
        reducedTriangles = 0;
        string folder = "Assets/ReducedMeshes";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "ReducedMeshes");

        GameObject instance = null;
        try
        {
            instance = Instantiate(sourceModel);
            instance.name = sourceModel.name + "_Reduced";
            instance.SetActive(false);
            Dictionary<Mesh, Mesh> processed = new Dictionary<Mesh, Mesh>();
            MeshFilter[] filters = instance.GetComponentsInChildren<MeshFilter>(true);
            for (int i = 0; i < filters.Length; i++)
            {
                MeshFilter filter = filters[i];
                Mesh original = filter.sharedMesh;
                if (original == null) continue;
                if (!processed.TryGetValue(original, out Mesh simplified))
                {
                    if (!original.isReadable)
                        throw new InvalidOperationException(original.name + " is not readable. Enable Read/Write in its import settings.");
                    EditorUtility.DisplayProgressBar("Reducing polygons", original.name, (float)i / Mathf.Max(1, filters.Length));
                    simplified = QuadricSimplifier.Simplify(original, quality, maximumCollapses);
                    simplified.name = original.name + "_Reduced";
                    string path = AssetDatabase.GenerateUniqueAssetPath(folder + "/" + simplified.name + ".asset");
                    AssetDatabase.CreateAsset(simplified, path);
                    processed.Add(original, simplified);
                }
                filter.sharedMesh = simplified;
                reducedTriangles += TriangleCount(simplified);
            }
            AssetDatabase.SaveAssets();
            instance.SetActive(sourceModel.activeSelf);
            Undo.RegisterCreatedObjectUndo(instance, "Create reduced model");
            Selection.activeGameObject = instance;
            EditorUtility.SetDirty(this);
            EditorUtility.DisplayDialog("Reduction complete",
                "Original triangles: " + originalTriangles.ToString("N0") +
                "\nReduced triangles: " + reducedTriangles.ToString("N0") +
                "\n\nNew mesh assets saved in " + folder, "OK");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            if (instance != null) DestroyImmediate(instance);
            EditorUtility.DisplayDialog("Reduction failed", ex.Message, "OK");
        }
        finally { EditorUtility.ClearProgressBar(); }
    }

    // Quadric-error edge-collapse reducer. Boundary edges, material boundaries and UV
    // discontinuities are locked to protect texture seams and the silhouette.
    private static class QuadricSimplifier
    {
        private struct Quadric
        {
            public double a, b, c, d, e, f, g, h, i, j;
            public static Quadric operator +(Quadric x, Quadric y)
            {
                return new Quadric
                {
                    a = x.a + y.a,
                    b = x.b + y.b,
                    c = x.c + y.c,
                    d = x.d + y.d,
                    e = x.e + y.e,
                    f = x.f + y.f,
                    g = x.g + y.g,
                    h = x.h + y.h,
                    i = x.i + y.i,
                    j = x.j + y.j
                };
            }
            public void AddPlane(Vector3 n, double distance)
            {
                double x = n.x, y = n.y, z = n.z, w = distance;
                a += x * x; b += x * y; c += x * z; d += x * w; e += y * y; f += y * z; g += y * w; h += z * z; i += z * w; j += w * w;
            }
            public double Evaluate(Vector3 v)
            {
                double x = v.x, y = v.y, z = v.z;
                return a * x * x + 2 * b * x * y + 2 * c * x * z + 2 * d * x + e * y * y + 2 * f * y * z + 2 * g * y + h * z * z + 2 * i * z + j;
            }
            public Vector3 Best(Vector3 p, Vector3 q)
            {
                double det = a * (e * h - f * f) - b * (b * h - c * f) + c * (b * f - c * e);
                if (Math.Abs(det) > 1e-12)
                {
                    double x = ((-d) * (e * h - f * f) + b * ((-g) * h + f * i) + c * (g * f - e * i)) / det;
                    double y = (a * ((-g) * h + f * i) - (-d) * (b * h - c * f) + c * (b * i - c * g)) / det;
                    double z = (a * (e * (-i) + g * f) - b * (b * (-i) + g * c) + (-d) * (b * f - e * c)) / det;
                    Vector3 candidate = new Vector3((float)x, (float)y, (float)z);
                    if (IsFinite(candidate)) return candidate;
                }
                Vector3 mid = (p + q) * 0.5f;
                double ep = Evaluate(p), eq = Evaluate(q), em = Evaluate(mid);
                return ep <= eq && ep <= em ? p : (eq <= em ? q : mid);
            }
            private static bool IsFinite(Vector3 v)
            {
                return !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z) &&
                    !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
            }
        }
        private struct Triangle
        {
            public int a, b, c, sub;
            public bool dead;
            public Triangle(int a, int b, int c, int sub) { this.a = a; this.b = b; this.c = c; this.sub = sub; dead = false; }
        }
        private struct Edge
        {
            public int a, b;
            public Edge(int x, int y) { a = Math.Min(x, y); b = Math.Max(x, y); }
            public long Key => ((long)a << 32) | (uint)b;
        }
        private sealed class EdgeInfo
        {
            public Edge edge;
            public int count;
            public int firstSub;
            public bool mixed;
        }
        private struct Candidate
        {
            public Edge edge;
            public Vector3 position;
            public double error;
        }

        public static Mesh Simplify(Mesh input, float quality, int maxCollapses)
        {
            if (quality >= 0.9999f) return UnityEngine.Object.Instantiate(input);
            Vector3[] positions = input.vertices;
            Vector2[] uv = input.uv;
            Vector3[] normals = input.normals;
            Vector4[] tangents = input.tangents;
            Color[] colors = input.colors;
            bool hasUV = uv.Length == positions.Length;
            bool hasNormal = normals.Length == positions.Length;
            bool hasTangent = tangents.Length == positions.Length;
            bool hasColor = colors.Length == positions.Length;
            List<Triangle> tris = new List<Triangle>();
            for (int s = 0; s < input.subMeshCount; s++)
            {
                if (input.GetTopology(s) != MeshTopology.Triangles)
                    throw new NotSupportedException("Only triangle topology is supported: " + input.name);
                int[] indices = input.GetTriangles(s);
                for (int t = 0; t < indices.Length; t += 3)
                    tris.Add(new Triangle(indices[t], indices[t + 1], indices[t + 2], s));
            }
            int target = Mathf.Max(1, Mathf.RoundToInt(tris.Count * quality));
            bool[] removed = new bool[positions.Length];
            int remaining = tris.Count;
            int collapses = 0;
            // Rebuild quadrics and edge candidates after each collapse. This favours
            // correctness and simplicity over speed on very large meshes.
            while (remaining > target && collapses < maxCollapses)
            {
                Quadric[] quadrics = new Quadric[positions.Length];
                Dictionary<long, EdgeInfo> edges = new Dictionary<long, EdgeInfo>();
                for (int t = 0; t < tris.Count; t++)
                {
                    Triangle tri = tris[t]; if (tri.dead) continue;
                    Vector3 p = positions[tri.a], q = positions[tri.b], r = positions[tri.c];
                    Vector3 cross = Vector3.Cross(q - p, r - p);
                    if (cross.sqrMagnitude > 1e-16f)
                    {
                        Vector3 n = cross.normalized;
                        double dist = -Vector3.Dot(n, p);
                        quadrics[tri.a].AddPlane(n, dist);
                        quadrics[tri.b].AddPlane(n, dist);
                        quadrics[tri.c].AddPlane(n, dist);
                    }
                    AddEdge(edges, tri.a, tri.b, tri.sub);
                    AddEdge(edges, tri.b, tri.c, tri.sub);
                    AddEdge(edges, tri.c, tri.a, tri.sub);
                }
                Candidate best = new Candidate { error = double.PositiveInfinity };
                bool found = false;
                foreach (EdgeInfo info in edges.Values)
                {
                    // Protect open boundaries and cross-material seams.
                    if (info.count != 2 || info.mixed) continue;
                    int a = info.edge.a, b = info.edge.b;
                    if (removed[a] || removed[b]) continue;
                    if (hasUV && (uv[a] - uv[b]).sqrMagnitude > 0.01f) continue;
                    if (hasNormal && Vector3.Dot(normals[a].normalized, normals[b].normalized) < 0.5f) continue;
                    Quadric sum = quadrics[a] + quadrics[b];
                    Vector3 p = sum.Best(positions[a], positions[b]);
                    double error = sum.Evaluate(p);
                    if (error < best.error)
                    {
                        best = new Candidate { edge = info.edge, position = p, error = error };
                        found = true;
                    }
                }
                if (!found) break;
                int keep = best.edge.a, discard = best.edge.b;
                // Prevent flipped or degenerate faces outside the removed edge's triangles.
                bool valid = true;
                for (int t = 0; t < tris.Count; t++)
                {
                    Triangle tri = tris[t]; if (tri.dead) continue;
                    bool usesKeep = tri.a == keep || tri.b == keep || tri.c == keep;
                    bool usesDiscard = tri.a == discard || tri.b == discard || tri.c == discard;
                    if (!usesKeep && !usesDiscard) continue;
                    if (usesKeep && usesDiscard) continue;
                    Vector3 oldA = positions[tri.a], oldB = positions[tri.b], oldC = positions[tri.c];
                    Vector3 newA = (tri.a == keep || tri.a == discard) ? best.position : oldA;
                    Vector3 newB = (tri.b == keep || tri.b == discard) ? best.position : oldB;
                    Vector3 newC = (tri.c == keep || tri.c == discard) ? best.position : oldC;
                    Vector3 oldN = Vector3.Cross(oldB - oldA, oldC - oldA);
                    Vector3 newN = Vector3.Cross(newB - newA, newC - newA);
                    if (newN.sqrMagnitude < 1e-14f || Vector3.Dot(oldN, newN) <= 0f) { valid = false; break; }
                }
                if (!valid)
                {
                    // Avoid retrying the same bad candidate: temporarily freeze this edge
                    // by separating its UV coordinates for subsequent passes.
                    if (hasUV) uv[discard] = uv[keep] + Vector2.one * 10f;
                    else break;
                    continue;
                }
                positions[keep] = best.position;
                if (hasUV) uv[keep] = (uv[keep] + uv[discard]) * 0.5f;
                if (hasNormal) normals[keep] = (normals[keep] + normals[discard]).normalized;
                if (hasTangent) tangents[keep] = (tangents[keep] + tangents[discard]) * 0.5f;
                if (hasColor) colors[keep] = (colors[keep] + colors[discard]) * 0.5f;
                removed[discard] = true;
                for (int t = 0; t < tris.Count; t++)
                {
                    Triangle tri = tris[t]; if (tri.dead) continue;
                    if (tri.a == discard) tri.a = keep;
                    if (tri.b == discard) tri.b = keep;
                    if (tri.c == discard) tri.c = keep;
                    if (tri.a == tri.b || tri.b == tri.c || tri.c == tri.a)
                    { tri.dead = true; remaining--; }
                    tris[t] = tri;
                }
                collapses++;
                if (collapses % 50 == 0 && EditorUtility.DisplayCancelableProgressBar(
                    "Reducing polygons", input.name + " - " + remaining + " triangles",
                    1f - (float)(remaining - target) / Mathf.Max(1, tris.Count - target)))
                    throw new OperationCanceledException("Polygon reduction cancelled.");
            }
            int[] map = new int[positions.Length];
            for (int i = 0; i < map.Length; i++) map[i] = -1;
            List<Vector3> outPos = new List<Vector3>();
            List<Vector2> outUV = new List<Vector2>();
            List<Vector3> outNormals = new List<Vector3>();
            List<Vector4> outTangents = new List<Vector4>();
            List<Color> outColors = new List<Color>();
            foreach (Triangle tri in tris)
            {
                if (tri.dead) continue;
                MapVertex(tri.a, positions, uv, normals, tangents, colors, hasUV, hasNormal, hasTangent, hasColor, map, outPos, outUV, outNormals, outTangents, outColors);
                MapVertex(tri.b, positions, uv, normals, tangents, colors, hasUV, hasNormal, hasTangent, hasColor, map, outPos, outUV, outNormals, outTangents, outColors);
                MapVertex(tri.c, positions, uv, normals, tangents, colors, hasUV, hasNormal, hasTangent, hasColor, map, outPos, outUV, outNormals, outTangents, outColors);
            }
            Mesh result = new Mesh();
            result.indexFormat = outPos.Count > 65535 ? UnityEngine.Rendering.IndexFormat.UInt32 : UnityEngine.Rendering.IndexFormat.UInt16;
            result.SetVertices(outPos);
            if (hasUV) result.SetUVs(0, outUV);
            if (hasNormal) result.SetNormals(outNormals);
            if (hasTangent) result.SetTangents(outTangents);
            if (hasColor) result.SetColors(outColors);
            result.subMeshCount = input.subMeshCount;
            for (int s = 0; s < input.subMeshCount; s++)
            {
                List<int> indices = new List<int>();
                foreach (Triangle tri in tris)
                {
                    if (tri.dead || tri.sub != s) continue;
                    indices.Add(map[tri.a]); indices.Add(map[tri.b]); indices.Add(map[tri.c]);
                }
                result.SetTriangles(indices, s, false);
            }
            if (!hasNormal) result.RecalculateNormals();
            result.RecalculateBounds();
            return result;
        }
        private static void AddEdge(Dictionary<long, EdgeInfo> edges, int a, int b, int sub)
        {
            Edge edge = new Edge(a, b);
            if (!edges.TryGetValue(edge.Key, out EdgeInfo info))
            {
                info = new EdgeInfo { edge = edge, firstSub = sub }; edges.Add(edge.Key, info);
            }
            info.count++;
            if (info.firstSub != sub) info.mixed = true;
        }
        private static void MapVertex(int v, Vector3[] p, Vector2[] uv, Vector3[] n, Vector4[] t, Color[] c,
            bool hu, bool hn, bool ht, bool hc, int[] map, List<Vector3> op, List<Vector2> ou, List<Vector3> on, List<Vector4> ot, List<Color> oc)
        {
            if (map[v] >= 0) return;
            map[v] = op.Count; op.Add(p[v]);
            if (hu) ou.Add(uv[v]); if (hn) on.Add(n[v]); if (ht) ot.Add(t[v]); if (hc) oc.Add(c[v]);
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(MeshPolygonReducer))]
public class MeshPolygonReducerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MeshPolygonReducer reducer = (MeshPolygonReducer)target;
        EditorGUILayout.Space();
        if (GUILayout.Button("Count Triangles")) { reducer.CountTriangles(); EditorUtility.SetDirty(reducer); }
        if (GUILayout.Button("Reduce Polygons", GUILayout.Height(36))) reducer.ReducePolygons();
    }
}
#endif
