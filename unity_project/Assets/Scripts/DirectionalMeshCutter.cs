
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Formats.Fbx.Exporter;
#endif

[ExecuteAlways]
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
    public bool showRemovalPreview = true;
    public bool showCutPlane = true;

    public Color removalColor =
        new Color(1f, 0f, 0f, 0.55f);

    public Color planeColor =
        new Color(1f, 0.7f, 0f, 0.9f);

    public float previewSize = 2f;

    [Header("Export")]
    public string exportFolder = @"D:\Meshes";
    public bool overwriteExisting = true;

    private const float Epsilon = 0.00001f;

    private Vector3 GetDirection()
    {
        switch (cutDirection)
        {
            case CutDirection.PositiveX: return Vector3.right;
            case CutDirection.NegativeX: return Vector3.left;
            case CutDirection.PositiveY: return Vector3.up;
            case CutDirection.NegativeY: return Vector3.down;
            case CutDirection.PositiveZ: return Vector3.forward;
            case CutDirection.NegativeZ: return Vector3.back;
            default: return Vector3.forward;
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

    private struct VertexData
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector4 tangent;
        public Vector2 uv;
        public Vector2 uv2;
        public Color color;

        public static VertexData Lerp(
            VertexData a, VertexData b, float t)
        {
            return new VertexData
            {
                position = Vector3.LerpUnclamped(
                    a.position, b.position, t),

                normal = Vector3.LerpUnclamped(
                    a.normal, b.normal, t).normalized,

                tangent = Vector4.LerpUnclamped(
                    a.tangent, b.tangent, t),

                uv = Vector2.LerpUnclamped(
                    a.uv, b.uv, t),

                uv2 = Vector2.LerpUnclamped(
                    a.uv2, b.uv2, t),

                color = Color.LerpUnclamped(
                    a.color, b.color, t)
            };
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (targetGameObject == null)
            return;

        GetCutPlane(out Vector3 point, out Vector3 normal);

        if (showCutPlane)
        {
            Quaternion rotation =
                Quaternion.FromToRotation(
                    Vector3.forward, normal);

            Gizmos.color = planeColor;
            Gizmos.matrix = Matrix4x4.TRS(
                point, rotation, Vector3.one);

            float size = Mathf.Max(0.01f, previewSize);

            Gizmos.DrawWireCube(
                Vector3.zero,
                new Vector3(size, size, 0.001f));

            Gizmos.matrix = Matrix4x4.identity;
        }

        if (showRemovalPreview)
            DrawRemovalPreview(point, normal);
    }

    private void DrawRemovalPreview(
        Vector3 planePoint,
        Vector3 planeNormal)
    {
        MeshFilter[] filters = includeChildren
            ? targetGameObject.GetComponentsInChildren
                <MeshFilter>(true)
            : targetGameObject.GetComponents<MeshFilter>();

        Color fillColor = removalColor;
        Color outlineColor = removalColor;
        outlineColor.a = 1f;

        foreach (MeshFilter filter in filters)
        {
            Mesh mesh = filter.sharedMesh;

            if (mesh == null || !mesh.isReadable)
                continue;

            Vector3[] vertices = mesh.vertices;

            Matrix4x4 matrix =
                filter.transform.localToWorldMatrix;

            for (int sub = 0; sub < mesh.subMeshCount; sub++)
            {
                if (mesh.GetTopology(sub) !=
                    MeshTopology.Triangles)
                    continue;

                int[] triangles = mesh.GetIndices(sub);

                for (int i = 0; i + 2 < triangles.Length; i += 3)
                {
                    Vector3 a = matrix.MultiplyPoint3x4(
                        vertices[triangles[i]]);

                    Vector3 b = matrix.MultiplyPoint3x4(
                        vertices[triangles[i + 1]]);

                    Vector3 c = matrix.MultiplyPoint3x4(
                        vertices[triangles[i + 2]]);

                    List<Vector3> polygon =
                        new List<Vector3> { a, b, c };

                    List<Vector3> removed =
                        ClipWorldPolygon(
                            polygon,
                            planePoint,
                            planeNormal,
                            true);

                    if (removed.Count < 3)
                        continue;

                    Handles.color = fillColor;

                    for (int j = 1; j < removed.Count - 1; j++)
                    {
                        Handles.DrawAAConvexPolygon(
                            removed[0],
                            removed[j],
                            removed[j + 1]);
                    }

                    Handles.color = outlineColor;

                    for (int j = 0; j < removed.Count; j++)
                    {
                        Handles.DrawLine(
                            removed[j],
                            removed[(j + 1) % removed.Count]);
                    }
                }
            }
        }
    }

    private List<Vector3> ClipWorldPolygon(
        List<Vector3> polygon,
        Vector3 planePoint,
        Vector3 planeNormal,
        bool keepRemoved)
    {
        List<Vector3> output = new List<Vector3>();

        for (int i = 0; i < polygon.Count; i++)
        {
            Vector3 current = polygon[i];
            Vector3 next = polygon[(i + 1) % polygon.Count];

            float da = Vector3.Dot(
                current - planePoint, planeNormal);

            float db = Vector3.Dot(
                next - planePoint, planeNormal);

            bool insideA = keepRemoved
                ? IsRemoved(da)
                : !IsRemoved(da);

            bool insideB = keepRemoved
                ? IsRemoved(db)
                : !IsRemoved(db);

            if (insideA && insideB)
            {
                output.Add(next);
            }
            else if (insideA && !insideB)
            {
                output.Add(Intersection(
                    current, next, da, db));
            }
            else if (!insideA && insideB)
            {
                output.Add(Intersection(
                    current, next, da, db));

                output.Add(next);
            }
        }

        return output;
    }

    private static Vector3 Intersection(
        Vector3 a, Vector3 b,
        float da, float db)
    {
        float denominator = da - db;

        float t = Mathf.Abs(denominator) < Epsilon
            ? 0f
            : Mathf.Clamp01(da / denominator);

        return Vector3.LerpUnclamped(a, b, t);
    }

    public void CutTargetAndExport()
    {
        if (targetGameObject == null)
        {
            Debug.LogError("Target GameObject is missing.");
            return;
        }

        string name = GetOriginalFbxName();
        string path = Path.Combine(exportFolder, name + ".fbx");

        if (File.Exists(path) && !overwriteExisting)
        {
            Debug.LogError("FBX already exists: " + path);
            return;
        }

        GameObject copy = null;

        try
        {
            Directory.CreateDirectory(exportFolder);

            GetCutPlane(
                out Vector3 planePoint,
                out Vector3 planeNormal);

            copy = Instantiate(targetGameObject);
            copy.name = name;
            copy.transform.SetParent(null, true);

            MeshFilter[] filters = includeChildren
                ? copy.GetComponentsInChildren<MeshFilter>(true)
                : copy.GetComponents<MeshFilter>();

            int processed = 0;

            foreach (MeshFilter filter in filters)
            {
                Mesh mesh = filter.sharedMesh;

                if (mesh == null || !mesh.isReadable)
                    continue;

                Mesh cut = CreateCutMesh(
                    mesh,
                    filter.transform,
                    planePoint,
                    planeNormal);

                filter.sharedMesh = cut;
                processed++;
            }

            if (processed == 0)
            {
                Debug.LogError(
                    "No readable meshes. Enable Read/Write " +
                    "in the FBX import settings.");
                return;
            }

            ModelExporter.ExportObject(path, copy);

            Debug.Log(
                "Exported cut FBX: " + path +
                "\nMeshes processed: " + processed);
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);
        }
        finally
        {
            if (copy != null)
                DestroyImmediate(copy);
        }
    }

    private Mesh CreateCutMesh(
        Mesh source,
        Transform meshTransform,
        Vector3 planePoint,
        Vector3 planeNormal)
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

        List<Vector3> outputPositions = new List<Vector3>();
        List<Vector3> outputNormals = new List<Vector3>();
        List<Vector4> outputTangents = new List<Vector4>();
        List<Vector2> outputUV = new List<Vector2>();
        List<Vector2> outputUV2 = new List<Vector2>();
        List<Color> outputColors = new List<Color>();

        List<int>[] outputIndices =
            new List<int>[source.subMeshCount];

        for (int i = 0; i < outputIndices.Length; i++)
            outputIndices[i] = new List<int>();

        Matrix4x4 worldToLocal =
            meshTransform.worldToLocalMatrix;

        Matrix4x4 localToWorld =
            meshTransform.localToWorldMatrix;

        Vector3 localPoint =
            worldToLocal.MultiplyPoint3x4(planePoint);

        Vector3 localNormal =
            localToWorld.transpose
                .MultiplyVector(planeNormal).normalized;

        for (int sub = 0; sub < source.subMeshCount; sub++)
        {
            if (source.GetTopology(sub) !=
                MeshTopology.Triangles)
                continue;

            int[] indices = source.GetIndices(sub);

            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                List<VertexData> triangle =
                    new List<VertexData>
                    {
                        Read(indices[i]),
                        Read(indices[i + 1]),
                        Read(indices[i + 2])
                    };

                List<VertexData> kept =
                    ClipLocalPolygon(
                        triangle,
                        localPoint,
                        localNormal);

                for (int j = 1; j < kept.Count - 1; j++)
                {
                    Add(kept[0], sub);
                    Add(kept[j], sub);
                    Add(kept[j + 1], sub);
                }
            }
        }

        Mesh result = new Mesh();
        result.name = source.name + "_Cut";

        if (outputPositions.Count > 65535)
        {
            result.indexFormat =
                UnityEngine.Rendering.IndexFormat.UInt32;
        }

        result.SetVertices(outputPositions);

        if (hasNormals)
            result.SetNormals(outputNormals);

        if (hasTangents)
            result.SetTangents(outputTangents);

        if (hasUV)
            result.SetUVs(0, outputUV);

        if (hasUV2)
            result.SetUVs(1, outputUV2);

        if (hasColors)
            result.SetColors(outputColors);

        result.subMeshCount = source.subMeshCount;

        for (int i = 0; i < source.subMeshCount; i++)
        {
            result.SetTriangles(
                outputIndices[i], i, false);
        }

        if (!hasNormals)
            result.RecalculateNormals();

        result.RecalculateBounds();

        return result;

        VertexData Read(int index)
        {
            return new VertexData
            {
                position = positions[index],
                normal = hasNormals
                    ? normals[index] : Vector3.up,
                tangent = hasTangents
                    ? tangents[index]
                    : new Vector4(1f, 0f, 0f, 1f),
                uv = hasUV ? uvs[index] : Vector2.zero,
                uv2 = hasUV2 ? uv2s[index] : Vector2.zero,
                color = hasColors
                    ? colors[index] : Color.white
            };
        }

        void Add(VertexData v, int sub)
        {
            int index = outputPositions.Count;

            outputPositions.Add(v.position);

            if (hasNormals)
                outputNormals.Add(v.normal);

            if (hasTangents)
                outputTangents.Add(v.tangent);

            if (hasUV)
                outputUV.Add(v.uv);

            if (hasUV2)
                outputUV2.Add(v.uv2);

            if (hasColors)
                outputColors.Add(v.color);

            outputIndices[sub].Add(index);
        }
    }

    private List<VertexData> ClipLocalPolygon(
        List<VertexData> polygon,
        Vector3 point,
        Vector3 normal)
    {
        List<VertexData> result = new List<VertexData>();

        for (int i = 0; i < polygon.Count; i++)
        {
            VertexData a = polygon[i];
            VertexData b =
                polygon[(i + 1) % polygon.Count];

            float da = Vector3.Dot(
                a.position - point, normal);

            float db = Vector3.Dot(
                b.position - point, normal);

            bool keepA = !IsRemoved(da);
            bool keepB = !IsRemoved(db);

            if (keepA && keepB)
            {
                result.Add(b);
            }
            else if (keepA && !keepB)
            {
                result.Add(
                    VertexData.Lerp(
                        a, b, GetT(da, db)));
            }
            else if (!keepA && keepB)
            {
                result.Add(
                    VertexData.Lerp(
                        a, b, GetT(da, db)));

                result.Add(b);
            }
        }

        return result;
    }

    private static float GetT(float a, float b)
    {
        float denominator = a - b;

        return Mathf.Abs(denominator) < Epsilon
            ? 0f
            : Mathf.Clamp01(a / denominator);
    }

    private string GetOriginalFbxName()
    {
        MeshFilter[] filters =
            targetGameObject.GetComponentsInChildren
                <MeshFilter>(true);

        foreach (MeshFilter filter in filters)
        {
            if (filter.sharedMesh == null)
                continue;

            string path =
                AssetDatabase.GetAssetPath(filter.sharedMesh);

            if (path.EndsWith(
                ".fbx",
                StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFileNameWithoutExtension(path);
            }
        }

        return targetGameObject.name
            .Replace("(Clone)", "")
            .Trim();
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

        DirectionalMeshCutter cutter =
            (DirectionalMeshCutter)target;

        EditorGUILayout.Space(10);

        GUI.enabled =
            cutter.targetGameObject != null &&
            !EditorApplication.isPlaying;

        if (GUILayout.Button(
            "CUT AND EXPORT FBX",
            GUILayout.Height(40)))
        {
            cutter.CutTargetAndExport();
        }

        GUI.enabled = true;

        EditorGUILayout.HelpBox(
            "Red polygons show the area that will be removed. " +
            "The original FBX remains unchanged.",
            MessageType.Info);
    }
}

#endif
