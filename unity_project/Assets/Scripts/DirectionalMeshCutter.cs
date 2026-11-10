using System;
using System.Collections.Generic;
using UnityEngine;
using UnityMeshSimplifier;
#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class DirectionalMeshCutter : MonoBehaviour
{
    public enum CutAxis { X, Y, Z }
    public enum RemovedSide { Positive, Negative }

    [Header("Target")]
    public GameObject targetObject;
    public bool includeChildren = true;

    [Header("World-space cutting plane")]
    public CutAxis axis = CutAxis.Y;
    public float cutPosition = 0f;
    public RemovedSide removeSide = RemovedSide.Positive;
    public bool showCutPlane = true;

    [Header("Simplification after cutting")]
    [Range(0.01f, 1f)] public float quality = 0.5f;
    public bool preserveBorders = true;
    public bool preserveUVSeams = true;
    public bool preserveUVFoldovers = true;

    [Header("Output")]
    public string outputFolder = "Assets/CutReducedMeshes";
    [SerializeField] private int originalTriangles;
    [SerializeField] private int finalTriangles;

    [Serializable]
    private class OriginalEntry
    {
        public MeshFilter filter;
        public Mesh mesh;
    }
    [SerializeField, HideInInspector] private List<OriginalEntry> originals = new List<OriginalEntry>();

    private Vector3 AxisVector => axis == CutAxis.X ? Vector3.right : axis == CutAxis.Y ? Vector3.up : Vector3.forward;
    private GameObject Target => targetObject != null ? targetObject : gameObject;

    private void OnDrawGizmosSelected()
    {
        if (!showCutPlane) return;
        Vector3 normal = AxisVector;
        Vector3 center = normal * cutPosition;
        Vector3 u = axis == CutAxis.X ? Vector3.up : Vector3.right;
        Vector3 v = Vector3.Cross(normal, u).normalized;
        float s = 2f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(center + u * s + v * s, center + u * s - v * s);
        Gizmos.DrawLine(center + u * s - v * s, center - u * s - v * s);
        Gizmos.DrawLine(center - u * s - v * s, center - u * s + v * s);
        Gizmos.DrawLine(center - u * s + v * s, center + u * s + v * s);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(center, normal * (removeSide == RemovedSide.Positive ? 0.7f : -0.7f));
    }

#if UNITY_EDITOR
    private struct Vertex
    {
        public Vector3 p, n;
        public Vector4 t;
        public Vector2 uv, uv2;
        public Color c;
        public float d;
        public static Vertex Lerp(Vertex a, Vertex b, float f)
        {
            return new Vertex
            {
                p = Vector3.LerpUnclamped(a.p, b.p, f),
                n = Vector3.LerpUnclamped(a.n, b.n, f).normalized,
                t = Vector4.LerpUnclamped(a.t, b.t, f),
                uv = Vector2.LerpUnclamped(a.uv, b.uv, f),
                uv2 = Vector2.LerpUnclamped(a.uv2, b.uv2, f),
                c = Color.LerpUnclamped(a.c, b.c, f),
                d = 0f
            };
        }
    }

    private static int TriangleCount(Mesh mesh)
    {
        int total = 0;
        if (mesh == null) return 0;
        for (int i = 0; i < mesh.subMeshCount; i++)
            if (mesh.GetTopology(i) == MeshTopology.Triangles)
                total += (int)mesh.GetIndexCount(i) / 3;
        return total;
    }

    private static bool Inside(Vertex v) => v.d <= 0.000001f;

    private static List<Vertex> ClipTriangle(Vertex a, Vertex b, Vertex c)
    {
        var input = new List<Vertex> { a, b, c };
        var output = new List<Vertex>(4);
        for (int i = 0; i < input.Count; i++)
        {
            Vertex current = input[i];
            Vertex next = input[(i + 1) % input.Count];
            bool ci = Inside(current), ni = Inside(next);
            if (ci && ni) output.Add(next);
            else if (ci && !ni) output.Add(Vertex.Lerp(current, next, current.d / (current.d - next.d)));
            else if (!ci && ni)
            {
                output.Add(Vertex.Lerp(current, next, current.d / (current.d - next.d)));
                output.Add(next);
            }
        }
        return output;
    }

    private Mesh CutMesh(Mesh source, Transform meshTransform)
    {
        Vector3[] positions = source.vertices;
        Vector3[] normals = source.normals;
        Vector4[] tangents = source.tangents;
        Vector2[] uvs = source.uv;
        Vector2[] uv2s = source.uv2;
        Color[] colors = source.colors;
        bool hasNormals = normals.Length == positions.Length;
        bool hasTangents = tangents.Length == positions.Length;
        bool hasUV = uvs.Length == positions.Length;
        bool hasUV2 = uv2s.Length == positions.Length;
        bool hasColors = colors.Length == positions.Length;

        Vector3 worldNormal = AxisVector * (removeSide == RemovedSide.Positive ? 1f : -1f);
        Vector3 worldPoint = AxisVector * cutPosition;
        // A signed distance evaluated in world space handles rotated, scaled and nested FBX objects.
        Vertex GetVertex(int index)
        {
            Vector3 p = positions[index];
            return new Vertex
            {
                p = p,
                n = hasNormals ? normals[index] : Vector3.up,
                t = hasTangents ? tangents[index] : new Vector4(1, 0, 0, 1),
                uv = hasUV ? uvs[index] : Vector2.zero,
                uv2 = hasUV2 ? uv2s[index] : Vector2.zero,
                c = hasColors ? colors[index] : Color.white,
                d = Vector3.Dot(meshTransform.TransformPoint(p) - worldPoint, worldNormal)
            };
        }

        var outPositions = new List<Vector3>();
        var outNormals = new List<Vector3>();
        var outTangents = new List<Vector4>();
        var outUV = new List<Vector2>();
        var outUV2 = new List<Vector2>();
        var outColors = new List<Color>();
        var subTriangles = new List<int[]>();
        void Add(Vertex vertex)
        {
            outPositions.Add(vertex.p);
            if (hasNormals) outNormals.Add(vertex.n);
            if (hasTangents) outTangents.Add(vertex.t);
            if (hasUV) outUV.Add(vertex.uv);
            if (hasUV2) outUV2.Add(vertex.uv2);
            if (hasColors) outColors.Add(vertex.c);
        }

        for (int sub = 0; sub < source.subMeshCount; sub++)
        {
            var indices = new List<int>();
            if (source.GetTopology(sub) == MeshTopology.Triangles)
            {
                int[] tris = source.GetTriangles(sub);
                for (int i = 0; i < tris.Length; i += 3)
                {
                    List<Vertex> polygon = ClipTriangle(GetVertex(tris[i]), GetVertex(tris[i + 1]), GetVertex(tris[i + 2]));
                    if (polygon.Count < 3) continue;
                    // Triangle fan preserves source winding and material/submesh assignment.
                    for (int k = 1; k < polygon.Count - 1; k++)
                    {
                        int first = outPositions.Count;
                        Add(polygon[0]); Add(polygon[k]); Add(polygon[k + 1]);
                        indices.Add(first); indices.Add(first + 1); indices.Add(first + 2);
                    }
                }
            }
            subTriangles.Add(indices.ToArray());
        }

        Mesh result = new Mesh { name = source.name + "_Cut" };
        result.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        result.SetVertices(outPositions);
        if (hasNormals) result.SetNormals(outNormals);
        if (hasTangents) result.SetTangents(outTangents);
        if (hasUV) result.SetUVs(0, outUV);
        if (hasUV2) result.SetUVs(1, outUV2);
        if (hasColors) result.SetColors(outColors);
        result.subMeshCount = source.subMeshCount;
        for (int sub = 0; sub < subTriangles.Count; sub++) result.SetTriangles(subTriangles[sub], sub);
        if (!hasNormals && result.vertexCount > 0) result.RecalculateNormals();
        result.RecalculateBounds();
        return result;
    }

    public void ApplyCutAndReduce()
    {
        RestoreOriginals();
        MeshFilter[] filters = includeChildren ? Target.GetComponentsInChildren<MeshFilter>(true) : Target.GetComponents<MeshFilter>();
        if (filters.Length == 0) { Debug.LogWarning("No MeshFilters found.", Target); return; }
        if (!outputFolder.Replace('\\', '/').StartsWith("Assets/")) { Debug.LogError("Output folder must begin with Assets/"); return; }
        EnsureFolder(outputFolder.Replace('\\', '/'));
        originalTriangles = 0; finalTriangles = 0;
        int processed = 0;
        foreach (MeshFilter filter in filters)
        {
            Mesh original = filter.sharedMesh;
            if (original == null) continue;
            if (!original.isReadable) { Debug.LogWarning("Enable Read/Write on FBX: " + original.name, filter); continue; }
            if (original.blendShapeCount > 0 || original.boneWeights.Length > 0)
            {
                Debug.LogWarning("Skipping rigged/blendshape mesh: " + original.name, filter); continue;
            }
            try
            {
                Mesh cut = CutMesh(original, filter.transform);
                Mesh result = cut;
                if (TriangleCount(cut) > 0 && quality < 0.999f)
                {
                    var simplifier = new MeshSimplifier();
                    simplifier.PreserveBorderEdges = preserveBorders;
                    simplifier.PreserveUVSeamEdges = preserveUVSeams;
                    simplifier.PreserveUVFoldoverEdges = preserveUVFoldovers;
                    simplifier.Initialize(cut);
                    simplifier.SimplifyMesh(quality);
                    Mesh reduced = simplifier.ToMesh();
                    if (TriangleCount(reduced) <= TriangleCount(cut) && TriangleCount(reduced) > 0)
                    {
                        result = reduced;
                        DestroyImmediate(cut);
                    }
                    else DestroyImmediate(reduced);
                }
                result.name = original.name + "_CutReduced";
                result.RecalculateBounds();
                string safe = MakeSafeName(filter.gameObject.name);
                string path = AssetDatabase.GenerateUniqueAssetPath(outputFolder.TrimEnd('/') + "/" + safe + "_CutReduced.asset");
                AssetDatabase.CreateAsset(result, path);
                Undo.RecordObject(filter, "Cut and Reduce Mesh");
                filter.sharedMesh = result;
                EditorUtility.SetDirty(filter);
                originals.Add(new OriginalEntry { filter = filter, mesh = original });
                originalTriangles += TriangleCount(original);
                finalTriangles += TriangleCount(result);
                processed++;
            }
            catch (Exception ex) { Debug.LogError("Failed on " + filter.name + ": " + ex, filter); }
        }
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(this);
        Debug.Log($"Cut + reduce complete. Meshes: {processed}, triangles: {originalTriangles:N0} -> {finalTriangles:N0} ({(originalTriangles > 0 ? 100f * (1f - (float)finalTriangles / originalTriangles) : 0f):F1}% reduction).", Target);
    }

    public void RestoreOriginals()
    {
        foreach (OriginalEntry entry in originals)
        {
            if (entry.filter == null || entry.mesh == null) continue;
            Undo.RecordObject(entry.filter, "Restore Original Mesh");
            entry.filter.sharedMesh = entry.mesh;
            EditorUtility.SetDirty(entry.filter);
        }
        originals.Clear();
        EditorUtility.SetDirty(this);
    }

    private static string MakeSafeName(string name)
    {
        foreach (char c in System.IO.Path.GetInvalidFileNameChars()) name = name.Replace(c, '_');
        return name.Replace('/', '_').Replace('\\', '_');
    }

    private static void EnsureFolder(string folder)
    {
        string[] parts = folder.Split('/');
        string current = "Assets";
        for (int i = 1; i < parts.Length; i++)
        {
            if (string.IsNullOrEmpty(parts[i])) continue;
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next)) AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(DirectionalMeshCutter))]
public class DirectionalMeshCutterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var cutter = (DirectionalMeshCutter)target;
        EditorGUILayout.HelpBox("World-space plane. Red arrow points to removed side. Cut surfaces are not capped. Static MeshFilters only.", MessageType.Info);
        if (GUILayout.Button("Cut + Reduce + Save", GUILayout.Height(34))) cutter.ApplyCutAndReduce();
        if (GUILayout.Button("Restore Original Meshes", GUILayout.Height(28))) cutter.RestoreOriginals();
    }
}
#endif
