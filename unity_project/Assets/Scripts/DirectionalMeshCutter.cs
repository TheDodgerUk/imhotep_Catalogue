
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class DirectionalMeshCutter : MonoBehaviour
{
    public enum CutDirection
    {
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
        PositiveZ,
        NegativeZ
    }

    public enum RemovalSide
    {
        Positive,
        Negative
    }

    [Header("Target")]
    public GameObject targetGameObject;
    public bool includeChildren = true;

    [Header("Cut Settings")]
    public CutDirection cutDirection = CutDirection.PositiveZ;
    public float planeOffset = 0f;
    public RemovalSide removeSide = RemovalSide.Positive;

    [Header("Preview")]
    public bool showCutPlane = true;
    public float previewSize = 2f;

    [Header("Export")]
    public string exportFolder = @"D:\Meshes";
    public bool overwriteExisting = true;

    [Header("Last Export Statistics")]
    [SerializeField] private long trianglesBefore;
    [SerializeField] private long trianglesAfter;
    [SerializeField] private long verticesBefore;
    [SerializeField] private long verticesAfter;
    [SerializeField] private float triangleReduction;
    [SerializeField] private float vertexReduction;

    public long TrianglesBefore => trianglesBefore;
    public long TrianglesAfter => trianglesAfter;
    public long VerticesBefore => verticesBefore;
    public long VerticesAfter => verticesAfter;
    public float TriangleReduction => triangleReduction;
    public float VertexReduction => vertexReduction;

    private const float Epsilon = 0.00001f;

    private struct VertexData
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector4 tangent;
        public Vector2 uv;
        public Vector2 uv2;
        public Color color;

        public static VertexData Lerp(
            VertexData a,
            VertexData b,
            float t)
        {
            Vector4 tangent = Vector4.LerpUnclamped(
                a.tangent, b.tangent, t);

            Vector3 tangentDirection = new Vector3(
                tangent.x, tangent.y, tangent.z);

            tangentDirection.Normalize();

            tangent.x = tangentDirection.x;
            tangent.y = tangentDirection.y;
            tangent.z = tangentDirection.z;
            tangent.w = a.tangent.w;

            return new VertexData
            {
                position = Vector3.LerpUnclamped(
                    a.position, b.position, t),

                normal = Vector3.LerpUnclamped(
                    a.normal, b.normal, t).normalized,

                tangent = tangent,

                uv = Vector2.LerpUnclamped(
                    a.uv, b.uv, t),

                uv2 = Vector2.LerpUnclamped(
                    a.uv2, b.uv2, t),

                color = Color.LerpUnclamped(
                    a.color, b.color, t)
            };
        }
    }

    private struct EdgeKey : IEquatable<EdgeKey>
    {
        public readonly int a;
        public readonly int b;

        public EdgeKey(int first, int second)
        {
            a = Mathf.Min(first, second);
            b = Mathf.Max(first, second);
        }

        public bool Equals(EdgeKey other)
        {
            return a == other.a && b == other.b;
        }

        public override bool Equals(object obj)
        {
            return obj is EdgeKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (a * 397) ^ b;
            }
        }
    }

    private struct MeshStats
    {
        public long triangles;
        public long vertices;
        public long meshes;
    }

    private Vector3 GetDirection()
    {
        switch (cutDirection)
        {
            case CutDirection.PositiveX:
                return Vector3.right;

            case CutDirection.NegativeX:
                return Vector3.left;

            case CutDirection.PositiveY:
                return Vector3.up;

            case CutDirection.NegativeY:
                return Vector3.down;

            case CutDirection.PositiveZ:
                return Vector3.forward;

            case CutDirection.NegativeZ:
                return Vector3.back;

            default:
                return Vector3.forward;
        }
    }

    private void GetCutPlane(
        out Vector3 point,
        out Vector3 normal)
    {
        Transform target = targetGameObject.transform;

        normal = target.rotation * GetDirection();
        normal.Normalize();

        point = target.position + normal * planeOffset;
    }

    private bool IsRemoved(float distance)
    {
        return removeSide == RemovalSide.Positive
            ? distance > Epsilon
            : distance < -Epsilon;
    }

#if UNITY_EDITOR

    // --------------------------------------------------
    // LIGHTWEIGHT SCENE PREVIEW
    // --------------------------------------------------

    private void OnDrawGizmos()
    {
        if (targetGameObject == null || !showCutPlane)
            return;

        GetCutPlane(
            out Vector3 point,
            out Vector3 normal);

        Quaternion rotation =
            Quaternion.FromToRotation(
                Vector3.forward, normal);

        Gizmos.color = Color.yellow;

        Gizmos.matrix = Matrix4x4.TRS(
            point,
            rotation,
            Vector3.one);

        float size = Mathf.Max(0.01f, previewSize);

        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(size, size, 0.001f));

        Gizmos.matrix = Matrix4x4.identity;
    }

    // --------------------------------------------------
    // COUNT MESH STATISTICS
    // --------------------------------------------------

    private static MeshStats CountMeshes(
        GameObject target,
        bool children)
    {
        MeshStats stats = new MeshStats();

        MeshFilter[] filters = children
            ? target.GetComponentsInChildren<MeshFilter>(true)
            : target.GetComponents<MeshFilter>();

        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = filter.sharedMesh;

            if (mesh == null)
                continue;

            stats.meshes++;
            stats.vertices += mesh.vertexCount;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                long count = (long)mesh.GetIndexCount(i);

                switch (mesh.GetTopology(i))
                {
                    case MeshTopology.Triangles:
                        stats.triangles += count / 3;
                        break;

                    case MeshTopology.Quads:
                        stats.triangles += (count / 4) * 2;
                        break;
                }
            }
        }

        return stats;
    }

    // --------------------------------------------------
    // CUT AND EXPORT
    // --------------------------------------------------

    public void CutTargetAndExport()
    {
        if (targetGameObject == null)
        {
            EditorUtility.DisplayDialog(
                "Missing Target",
                "Please assign a Target GameObject.",
                "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(exportFolder))
        {
            EditorUtility.DisplayDialog(
                "Invalid Export Folder",
                "Please enter an export folder.",
                "OK");
            return;
        }

        string modelName = GetOriginalFbxName();

        string path = Path.Combine(
            exportFolder,
            modelName + ".fbx");

        if (File.Exists(path) && !overwriteExisting)
        {
            EditorUtility.DisplayDialog(
                "File Already Exists",
                path,
                "OK");
            return;
        }

        GameObject copy = null;
        List<Mesh> createdMeshes = new List<Mesh>();

        try
        {
            GetCutPlane(
                out Vector3 planePoint,
                out Vector3 planeNormal);

            MeshStats before = CountMeshes(
                targetGameObject,
                includeChildren);

            trianglesBefore = before.triangles;
            verticesBefore = before.vertices;

            if (before.meshes == 0)
            {
                EditorUtility.DisplayDialog(
                    "No Meshes",
                    "No static meshes were found.",
                    "OK");
                return;
            }

            // Validate before creating the export.
            MeshFilter[] originalFilters = includeChildren
                ? targetGameObject.GetComponentsInChildren
                    <MeshFilter>(true)
                : targetGameObject.GetComponents<MeshFilter>();

            foreach (MeshFilter filter in originalFilters)
            {
                Mesh mesh = filter.sharedMesh;

                if (mesh == null)
                    continue;

                if (!mesh.isReadable)
                {
                    EditorUtility.DisplayDialog(
                        "Unreadable Mesh",
                        mesh.name +
                        "\n\nEnable Read/Write in the FBX " +
                        "Model Import Settings.",
                        "OK");
                    return;
                }

                for (int s = 0; s < mesh.subMeshCount; s++)
                {
                    if (mesh.GetTopology(s) !=
                        MeshTopology.Triangles)
                    {
                        EditorUtility.DisplayDialog(
                            "Unsupported Mesh",
                            mesh.name +
                            "\n\nOnly triangle topology is supported.",
                            "OK");
                        return;
                    }
                }

                if (mesh.blendShapeCount > 0 ||
                    mesh.boneWeights.Length > 0)
                {
                    EditorUtility.DisplayDialog(
                        "Unsupported Mesh Data",
                        mesh.name +
                        "\n\nThis cutter is intended for static meshes.",
                        "OK");
                    return;
                }
            }

            // Skinned meshes are not supported.
            SkinnedMeshRenderer[] skinned =
                targetGameObject.GetComponentsInChildren
                    <SkinnedMeshRenderer>(true);

            foreach (SkinnedMeshRenderer renderer in skinned)
            {
                if (!includeChildren &&
                    renderer.gameObject != targetGameObject)
                    continue;

                if (renderer.sharedMesh != null)
                {
                    EditorUtility.DisplayDialog(
                        "Skinned Mesh Detected",
                        "This cutter does not support " +
                        "SkinnedMeshRenderer meshes.",
                        "OK");
                    return;
                }
            }

            // Copy the original hierarchy.
            copy = Instantiate(targetGameObject);
            copy.name = modelName;
            copy.transform.SetParent(null, true);

            // Remove children if only the root was requested.
            if (!includeChildren)
            {
                for (int i = copy.transform.childCount - 1;
                     i >= 0;
                     i--)
                {
                    DestroyImmediate(
                        copy.transform.GetChild(i).gameObject);
                }
            }

            // Remove scripts and non-mesh components
            // from the temporary export hierarchy.
            PrepareExportHierarchy(copy);

            MeshFilter[] filters =
                copy.GetComponentsInChildren<MeshFilter>(true);

            int processed = 0;

            foreach (MeshFilter filter in filters)
            {
                Mesh source = filter.sharedMesh;

                if (source == null)
                    continue;

                Mesh result = CreateOptimizedCutMesh(
                    source,
                    filter.transform,
                    planePoint,
                    planeNormal);

                createdMeshes.Add(result);
                filter.sharedMesh = result;

                processed++;
            }

            MeshStats after = CountMeshes(copy, true);

            trianglesAfter = after.triangles;
            verticesAfter = after.vertices;

            triangleReduction = trianglesBefore > 0
                ? (float)(trianglesBefore - trianglesAfter) /
                  trianglesBefore * 100f
                : 0f;

            vertexReduction = verticesBefore > 0
                ? (float)(verticesBefore - verticesAfter) /
                  verticesBefore * 100f
                : 0f;

            Directory.CreateDirectory(exportFolder);

            string exportedPath =
                ModelExporter.ExportObject(path, copy);

            if (string.IsNullOrEmpty(exportedPath) ||
                !File.Exists(exportedPath))
            {
                throw new IOException(
                    "FBX export did not produce a file.");
            }

            EditorUtility.SetDirty(this);

            ShowResults(exportedPath, processed);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);

            EditorUtility.DisplayDialog(
                "Mesh Cut Failed",
                exception.Message,
                "OK");
        }
        finally
        {
            if (copy != null)
                DestroyImmediate(copy);

            foreach (Mesh mesh in createdMeshes)
            {
                if (mesh != null)
                    DestroyImmediate(mesh);
            }
        }
    }

    // --------------------------------------------------
    // PREPARE TEMPORARY EXPORT
    // --------------------------------------------------

    private static void PrepareExportHierarchy(GameObject root)
    {
        // Remove unsupported skinned renderers.
        // They are already rejected during validation.
        // Strip scripts and non-rendering components
        // from the temporary export copy.

        Transform[] transforms =
            root.GetComponentsInChildren<Transform>(true);

        foreach (Transform current in transforms)
        {
            Component[] components =
                current.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null)
                    continue;

                if (component is Transform ||
                    component is MeshFilter ||
                    component is MeshRenderer)
                    continue;

                DestroyImmediate(component);
            }
        }
    }

    // --------------------------------------------------
    // OPTIMIZED MESH CUTTING
    // --------------------------------------------------

    private Mesh CreateOptimizedCutMesh(
        Mesh source,
        Transform meshTransform,
        Vector3 planePoint,
        Vector3 planeNormal)
    {
        Vector3[] positions = source.vertices;
        Vector3[] normals = source.normals;
        Vector4[] tangents = source.tangents;
        Vector2[] uv = source.uv;
        Vector2[] uv2 = source.uv2;
        Color[] colors = source.colors;

        bool hasNormals = normals.Length == positions.Length;
        bool hasTangents = tangents.Length == positions.Length;
        bool hasUV = uv.Length == positions.Length;
        bool hasUV2 = uv2.Length == positions.Length;
        bool hasColors = colors.Length == positions.Length;

        Matrix4x4 worldToLocal =
            meshTransform.worldToLocalMatrix;

        Matrix4x4 localToWorld =
            meshTransform.localToWorldMatrix;

        Vector3 localPlanePoint =
            worldToLocal.MultiplyPoint3x4(planePoint);

        Vector3 localPlaneNormal =
            localToWorld.transpose
                .MultiplyVector(planeNormal).normalized;

        float[] distances = new float[positions.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            distances[i] = Vector3.Dot(
                positions[i] - localPlanePoint,
                localPlaneNormal);
        }

        List<Vector3> outPositions =
            new List<Vector3>();

        List<Vector3> outNormals =
            new List<Vector3>();

        List<Vector4> outTangents =
            new List<Vector4>();

        List<Vector2> outUV =
            new List<Vector2>();

        List<Vector2> outUV2 =
            new List<Vector2>();

        List<Color> outColors =
            new List<Color>();

        List<int>[] submeshIndices =
            new List<int>[source.subMeshCount];

        for (int i = 0; i < submeshIndices.Length; i++)
            submeshIndices[i] = new List<int>();

        // Original vertex -> output vertex.
        Dictionary<int, int> vertexMap =
            new Dictionary<int, int>();

        // Shared edge -> intersection vertex.
        Dictionary<EdgeKey, int> edgeMap =
            new Dictionary<EdgeKey, int>();

        for (int sub = 0; sub < source.subMeshCount; sub++)
        {
            int[] indices = source.GetIndices(sub);

            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                int a = indices[i];
                int b = indices[i + 1];
                int c = indices[i + 2];

                // Clip the triangle and produce indices
                // directly into the output mesh.
                List<int> polygon = ClipTriangle(a, b, c);

                if (polygon.Count < 3)
                    continue;

                // Remove consecutive duplicate indices.
                for (int p = polygon.Count - 1; p >= 0; p--)
                {
                    int previous =
                        polygon[(p - 1 + polygon.Count) %
                                polygon.Count];

                    if (polygon[p] == previous)
                        polygon.RemoveAt(p);
                }

                if (polygon.Count < 3)
                    continue;

                // Triangulate the clipped polygon.
                for (int p = 1; p < polygon.Count - 1; p++)
                {
                    int ia = polygon[0];
                    int ib = polygon[p];
                    int ic = polygon[p + 1];

                    Vector3 pa = outPositions[ia];
                    Vector3 pb = outPositions[ib];
                    Vector3 pc = outPositions[ic];

                    // Skip zero-area triangles.
                    if (Vector3.Cross(
                        pb - pa,
                        pc - pa).sqrMagnitude < 1e-16f)
                        continue;

                    submeshIndices[sub].Add(ia);
                    submeshIndices[sub].Add(ib);
                    submeshIndices[sub].Add(ic);
                }
            }
        }

        Mesh result = new Mesh();
        result.name = source.name + "_Cut";

        if (outPositions.Count > 65535)
            result.indexFormat = IndexFormat.UInt32;

        result.SetVertices(outPositions);

        if (hasNormals)
            result.SetNormals(outNormals);

        if (hasTangents)
            result.SetTangents(outTangents);

        if (hasUV)
            result.SetUVs(0, outUV);

        if (hasUV2)
            result.SetUVs(1, outUV2);

        if (hasColors)
            result.SetColors(outColors);

        result.subMeshCount = source.subMeshCount;

        for (int i = 0; i < submeshIndices.Length; i++)
        {
            result.SetTriangles(
                submeshIndices[i], i, false);
        }

        if (!hasNormals)
            result.RecalculateNormals();

        result.RecalculateBounds();

        return result;

        // ------------------------------------------
        // ORIGINAL VERTEX DATA
        // ------------------------------------------

        VertexData ReadVertex(int index)
        {
            return new VertexData
            {
                position = positions[index],

                normal = hasNormals
                    ? normals[index]
                    : Vector3.up,

                tangent = hasTangents
                    ? tangents[index]
                    : new Vector4(1f, 0f, 0f, 1f),

                uv = hasUV
                    ? uv[index]
                    : Vector2.zero,

                uv2 = hasUV2
                    ? uv2[index]
                    : Vector2.zero,

                color = hasColors
                    ? colors[index]
                    : Color.white
            };
        }

        // ------------------------------------------
        // ADD OUTPUT VERTEX
        // ------------------------------------------

        int AddVertex(VertexData data)
        {
            int index = outPositions.Count;

            outPositions.Add(data.position);

            if (hasNormals)
                outNormals.Add(data.normal);

            if (hasTangents)
                outTangents.Add(data.tangent);

            if (hasUV)
                outUV.Add(data.uv);

            if (hasUV2)
                outUV2.Add(data.uv2);

            if (hasColors)
                outColors.Add(data.color);

            return index;
        }

        // ------------------------------------------
        // REUSE EXISTING VERTEX
        // ------------------------------------------

        int GetOriginalVertex(int index)
        {
            if (vertexMap.TryGetValue(
                index,
                out int existing))
            {
                return existing;
            }

            int created = AddVertex(ReadVertex(index));

            vertexMap.Add(index, created);

            return created;
        }

        // ------------------------------------------
        // CREATE OR REUSE EDGE INTERSECTION
        // ------------------------------------------

        int GetIntersection(int a, int b)
        {
            float da = distances[a];
            float db = distances[b];

            // Reuse a vertex lying on the plane.
            if (Mathf.Abs(da) <= Epsilon)
                return GetOriginalVertex(a);

            if (Mathf.Abs(db) <= Epsilon)
                return GetOriginalVertex(b);

            EdgeKey key = new EdgeKey(a, b);

            if (edgeMap.TryGetValue(
                key,
                out int existing))
            {
                return existing;
            }

            float denominator = da - db;

            float t = Mathf.Abs(denominator) < Epsilon
                ? 0f
                : Mathf.Clamp01(da / denominator);

            VertexData resultData = VertexData.Lerp(
                ReadVertex(a),
                ReadVertex(b),
                t);

            int created = AddVertex(resultData);

            edgeMap.Add(key, created);

            return created;
        }

        // ------------------------------------------
        // CLIP A SINGLE TRIANGLE
        // ------------------------------------------

        List<int> ClipTriangle(int a, int b, int c)
        {
            int[] original = { a, b, c };

            List<int> output = new List<int>(4);

            for (int i = 0; i < 3; i++)
            {
                int current = original[i];
                int next = original[(i + 1) % 3];

                bool keepCurrent =
                    !IsRemoved(distances[current]);

                bool keepNext =
                    !IsRemoved(distances[next]);

                if (keepCurrent && keepNext)
                {
                    output.Add(GetOriginalVertex(next));
                }
                else if (keepCurrent && !keepNext)
                {
                    output.Add(
                        GetIntersection(current, next));
                }
                else if (!keepCurrent && keepNext)
                {
                    output.Add(
                        GetIntersection(current, next));

                    output.Add(GetOriginalVertex(next));
                }
            }

            return output;
        }
    }

    // --------------------------------------------------
    // ORIGINAL FBX FILENAME
    // --------------------------------------------------

    private string GetOriginalFbxName()
    {
        MeshFilter[] filters =
            targetGameObject.GetComponentsInChildren
                <MeshFilter>(true);

        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = filter.sharedMesh;

            if (mesh == null)
                continue;

            string assetPath =
                AssetDatabase.GetAssetPath(mesh);

            if (!string.IsNullOrEmpty(assetPath) &&
                assetPath.EndsWith(
                    ".fbx",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFileNameWithoutExtension(
                    assetPath);
            }
        }

        return targetGameObject.name
            .Replace("(Clone)", "")
            .Trim();
    }

    // --------------------------------------------------
    // RESULTS DIALOG
    // --------------------------------------------------

    private void ShowResults(
        string path,
        int processedMeshes)
    {
        string message =
            "MESH CUT COMPLETE\n\n" +

            "Meshes Processed: " +
            processedMeshes.ToString("N0") + "\n\n" +

            "TRIANGLES\n" +

            "Before: " +
            trianglesBefore.ToString("N0") + "\n" +

            "After: " +
            trianglesAfter.ToString("N0") + "\n" +

            "Net Removed: " +
            (trianglesBefore - trianglesAfter)
                .ToString("N0") + "\n" +

            "Reduction: " +
            triangleReduction.ToString("F2") + "%\n\n" +

            "VERTICES\n" +

            "Before: " +
            verticesBefore.ToString("N0") + "\n" +

            "After: " +
            verticesAfter.ToString("N0") + "\n" +

            "Net Removed: " +
            (verticesBefore - verticesAfter)
                .ToString("N0") + "\n" +

            "Reduction: " +
            vertexReduction.ToString("F2") + "%\n\n" +

            "Saved To:\n" + path;

        Debug.Log(message);

        EditorUtility.DisplayDialog(
            "Mesh Cutting Complete",
            message,
            "OK");
    }

#endif
}

// --------------------------------------------------
// CUSTOM INSPECTOR
// --------------------------------------------------

#if UNITY_EDITOR

[CustomEditor(typeof(DirectionalMeshCutter))]
public class DirectionalMeshCutterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DirectionalMeshCutter cutter =
            (DirectionalMeshCutter)target;

        serializedObject.Update();

        EditorGUILayout.LabelField(
            "Target",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("targetGameObject"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("includeChildren"));

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Cut Settings",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("cutDirection"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("planeOffset"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("removeSide"));

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Preview",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("showCutPlane"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("previewSize"));

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Export",
            EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("exportFolder"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("overwriteExisting"));

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(12);

        GUI.enabled =
            cutter.targetGameObject != null &&
            !EditorApplication.isPlaying;

        if (GUILayout.Button(
            "CUT AND EXPORT FBX",
            GUILayout.Height(45)))
        {
            cutter.CutTargetAndExport();
        }

        GUI.enabled = true;

        EditorGUILayout.Space(12);

        EditorGUILayout.LabelField(
            "Last Export Statistics",
            EditorStyles.boldLabel);

        DrawReadOnly(
            "Triangles Before",
            cutter.TrianglesBefore);

        DrawReadOnly(
            "Triangles After",
            cutter.TrianglesAfter);

        DrawReadOnly(
            "Vertices Before",
            cutter.VerticesBefore);

        DrawReadOnly(
            "Vertices After",
            cutter.VerticesAfter);

        DrawReadOnly(
            "Triangle Reduction",
            cutter.TriangleReduction.ToString("F2") + "%");

        DrawReadOnly(
            "Vertex Reduction",
            cutter.VertexReduction.ToString("F2") + "%");

        EditorGUILayout.Space(8);

        EditorGUILayout.HelpBox(
            "The original mesh is unchanged.\n" +
            "Vertices are reused where possible.\n" +
            "No polygon colouring is performed.",
            MessageType.Info);
    }

    private static void DrawReadOnly(
        string label,
        long value)
    {
        DrawReadOnly(
            label,
            value.ToString(
                "N0",
                System.Globalization.CultureInfo
                    .GetCultureInfo("en-GB")));
    }

    private static void DrawReadOnly(
        string label,
        string value)
    {
        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.TextField(label, value);
        }
    }
}

#endif
