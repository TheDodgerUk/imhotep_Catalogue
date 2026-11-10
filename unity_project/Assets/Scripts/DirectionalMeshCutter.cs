
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
    public enum RemovalSide
    {
        Positive,
        Negative
    }

    [Header("Target")]
    public GameObject targetGameObject;
    public bool includeChildren = true;

    [Header("Cut Plane")]
    public Vector3 planeRotation = Vector3.zero;
    public float planeOffset = 0f;
    public RemovalSide removeSide = RemovalSide.Positive;

    [Header("Preview")]
    public bool showCutPlane = true;
    public Color planeColor = new Color(1f, 0.3f, 0.2f, 0.8f);
    public float previewSize = 2f;

    [Header("Export")]
    public string exportFolder = @"D:\Meshes";
    public bool overwriteExisting = true;

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
            VertexData result = new VertexData();

            result.position =
                Vector3.LerpUnclamped(a.position, b.position, t);

            result.normal =
                Vector3.LerpUnclamped(a.normal, b.normal, t).normalized;

            result.tangent =
                Vector4.LerpUnclamped(a.tangent, b.tangent, t);

            result.uv =
                Vector2.LerpUnclamped(a.uv, b.uv, t);

            result.uv2 =
                Vector2.LerpUnclamped(a.uv2, b.uv2, t);

            result.color =
                Color.LerpUnclamped(a.color, b.color, t);

            return result;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showCutPlane || targetGameObject == null)
            return;

        Transform target = targetGameObject.transform;

        Quaternion rotation =
            target.rotation * Quaternion.Euler(planeRotation);

        Vector3 normal = rotation * Vector3.forward;

        Vector3 centre =
            target.position + normal * planeOffset;

        Gizmos.color = planeColor;
        Gizmos.matrix =
            Matrix4x4.TRS(centre, rotation, Vector3.one);

        float size = Mathf.Max(0.01f, previewSize);

        Gizmos.DrawWireCube(
            Vector3.zero,
            new Vector3(size, size, 0.001f));

        Gizmos.DrawLine(
            Vector3.zero,
            Vector3.forward * size * 0.4f);

        Gizmos.DrawSphere(
            Vector3.forward * size * 0.4f,
            size * 0.025f);

        Gizmos.matrix = Matrix4x4.identity;
    }

#if UNITY_EDITOR

    public void CutTargetAndExport()
    {
        if (targetGameObject == null)
        {
            Debug.LogError("Assign a Target GameObject.");
            return;
        }

        if (string.IsNullOrWhiteSpace(exportFolder))
        {
            Debug.LogError("Export folder is empty.");
            return;
        }

        string originalName = GetOriginalFbxName();

        string outputPath = Path.Combine(
            exportFolder,
            originalName + ".fbx");

        if (File.Exists(outputPath) && !overwriteExisting)
        {
            Debug.LogError(
                "File already exists: " + outputPath);
            return;
        }

        GameObject copy = null;

        try
        {
            Directory.CreateDirectory(exportFolder);

            // Duplicate the target so the original is untouched.
            copy = Instantiate(targetGameObject);
            copy.name = originalName;
            copy.SetActive(true);

            // Keep the duplicate outside the original hierarchy.
            copy.transform.SetParent(null, true);

            Vector3 planeNormal =
                targetGameObject.transform.rotation *
                Quaternion.Euler(planeRotation) *
                Vector3.forward;

            planeNormal.Normalize();

            Vector3 planePoint =
                targetGameObject.transform.position +
                planeNormal * planeOffset;

            int meshCount = 0;
            int removedTriangles = 0;

            MeshFilter[] filters = includeChildren
                ? copy.GetComponentsInChildren<MeshFilter>(true)
                : copy.GetComponents<MeshFilter>();

            foreach (MeshFilter filter in filters)
            {
                if (filter.sharedMesh == null)
                    continue;

                Mesh source = filter.sharedMesh;

                if (!source.isReadable)
                {
                    Debug.LogWarning(
                        "Mesh is not readable: " + source.name);
                    continue;
                }

                int before = CountTriangles(source);

                Mesh result = CutMesh(
                    source,
                    filter.transform,
                    planePoint,
                    planeNormal);

                if (result == null)
                    continue;

                filter.sharedMesh = result;

                removedTriangles +=
                    before - CountTriangles(result);

                meshCount++;
            }

            SkinnedMeshRenderer[] skinned =
                includeChildren
                    ? copy.GetComponentsInChildren
                        <SkinnedMeshRenderer>(true)
                    : copy.GetComponents
                        <SkinnedMeshRenderer>();

            if (skinned.Length > 0)
            {
                Debug.LogWarning(
                    "Skinned meshes are not cut by this tool.");
            }

            if (meshCount == 0)
            {
                Debug.LogError(
                    "No readable MeshFilter meshes found. " +
                    "Enable Read/Write in the FBX import settings.");
                return;
            }

            // Export the modified duplicate as an FBX.
            ModelExporter.ExportObject(outputPath, copy);

            Debug.Log(
                "FBX exported: " + outputPath +
                "\nMeshes processed: " + meshCount +
                "\nTriangles removed: " + removedTriangles);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            if (copy != null)
                DestroyImmediate(copy);
        }
    }

    private string GetOriginalFbxName()
    {
        MeshFilter filter =
            targetGameObject.GetComponentInChildren<MeshFilter>(true);

        if (filter != null && filter.sharedMesh != null)
        {
            string path =
                AssetDatabase.GetAssetPath(filter.sharedMesh);

            if (!string.IsNullOrEmpty(path) &&
                path.EndsWith(
                    ".fbx",
                    StringComparison.OrdinalIgnoreCase))
            {
                return Path.GetFileNameWithoutExtension(path);
            }
        }

        string prefabPath =
            PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(
                targetGameObject);

        if (!string.IsNullOrEmpty(prefabPath) &&
            prefabPath.EndsWith(
                ".fbx",
                StringComparison.OrdinalIgnoreCase))
        {
            return Path.GetFileNameWithoutExtension(prefabPath);
        }

        return targetGameObject.name
            .Replace("(Clone)", "")
            .Trim();
    }

    private static int CountTriangles(Mesh mesh)
    {
        int count = 0;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            if (mesh.GetTopology(i) == MeshTopology.Triangles)
                count += mesh.GetIndices(i).Length / 3;
        }

        return count;
    }

    private Mesh CutMesh(
        Mesh source,
        Transform meshTransform,
        Vector3 planePointWorld,
        Vector3 planeNormalWorld)
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

        List<Vector3> outPositions = new List<Vector3>();
        List<Vector3> outNormals = new List<Vector3>();
        List<Vector4> outTangents = new List<Vector4>();
        List<Vector2> outUV = new List<Vector2>();
        List<Vector2> outUV2 = new List<Vector2>();
        List<Color> outColors = new List<Color>();

        List<int>[] submeshTriangles =
            new List<int>[source.subMeshCount];

        for (int s = 0; s < source.subMeshCount; s++)
            submeshTriangles[s] = new List<int>();

        // Convert the world-space plane into mesh-local space.
        Matrix4x4 localToWorld =
            meshTransform.localToWorldMatrix;

        Matrix4x4 worldToLocal =
            meshTransform.worldToLocalMatrix;

        Vector3 localPlanePoint =
            worldToLocal.MultiplyPoint3x4(planePointWorld);

        Vector3 localPlaneNormal =
            localToWorld.transpose
                .MultiplyVector(planeNormalWorld)
                .normalized;

        if (localPlaneNormal.sqrMagnitude < Epsilon)
            return null;

        for (int sub = 0; sub < source.subMeshCount; sub++)
        {
            if (source.GetTopology(sub) != MeshTopology.Triangles)
            {
                Debug.LogWarning(
                    "Skipping non-triangle submesh: " + sub);
                continue;
            }

            int[] indices = source.GetIndices(sub);

            for (int i = 0; i + 2 < indices.Length; i += 3)
            {
                VertexData a = ReadVertex(indices[i]);
                VertexData b = ReadVertex(indices[i + 1]);
                VertexData c = ReadVertex(indices[i + 2]);

                List<VertexData> polygon =
                    new List<VertexData> { a, b, c };

                List<VertexData> clipped =
                    ClipPolygon(
                        polygon,
                        localPlanePoint,
                        localPlaneNormal);

                if (clipped.Count < 3)
                    continue;

                // Triangulate the resulting polygon.
                for (int j = 1; j < clipped.Count - 1; j++)
                {
                    AddVertex(clipped[0], sub);
                    AddVertex(clipped[j], sub);
                    AddVertex(clipped[j + 1], sub);
                }
            }
        }

        Mesh output = new Mesh();
        output.name = source.name + "_Cut";

        if (outPositions.Count > 65535)
        {
            output.indexFormat =
                UnityEngine.Rendering.IndexFormat.UInt32;
        }

        output.SetVertices(outPositions);

        if (hasNormals)
            output.SetNormals(outNormals);

        if (hasTangents)
            output.SetTangents(outTangents);

        if (hasUV)
            output.SetUVs(0, outUV);

        if (hasUV2)
            output.SetUVs(1, outUV2);

        if (hasColors)
            output.SetColors(outColors);

        output.subMeshCount = source.subMeshCount;

        for (int sub = 0; sub < source.subMeshCount; sub++)
        {
            output.SetTriangles(
                submeshTriangles[sub],
                sub,
                false);
        }

        if (!hasNormals)
            output.RecalculateNormals();

        output.RecalculateBounds();

        return output;

        VertexData ReadVertex(int index)
        {
            VertexData v = new VertexData();

            v.position = positions[index];

            v.normal = hasNormals
                ? normals[index]
                : Vector3.up;

            v.tangent = hasTangents
                ? tangents[index]
                : new Vector4(1f, 0f, 0f, 1f);

            v.uv = hasUV ? uv[index] : Vector2.zero;
            v.uv2 = hasUV2 ? uv2[index] : Vector2.zero;
            v.color = hasColors ? colors[index] : Color.white;

            return v;
        }

        void AddVertex(VertexData vertex, int submesh)
        {
            int index = outPositions.Count;

            outPositions.Add(vertex.position);

            if (hasNormals)
                outNormals.Add(vertex.normal);

            if (hasTangents)
                outTangents.Add(vertex.tangent);

            if (hasUV)
                outUV.Add(vertex.uv);

            if (hasUV2)
                outUV2.Add(vertex.uv2);

            if (hasColors)
                outColors.Add(vertex.color);

            submeshTriangles[submesh].Add(index);
        }
    }

    private List<VertexData> ClipPolygon(
        List<VertexData> polygon,
        Vector3 planePoint,
        Vector3 planeNormal)
    {
        List<VertexData> result = new List<VertexData>();

        for (int i = 0; i < polygon.Count; i++)
        {
            VertexData current = polygon[i];
            VertexData next =
                polygon[(i + 1) % polygon.Count];

            float currentDistance =
                Vector3.Dot(
                    current.position - planePoint,
                    planeNormal);

            float nextDistance =
                Vector3.Dot(
                    next.position - planePoint,
                    planeNormal);

            bool currentInside = removeSide ==
                RemovalSide.Positive
                    ? currentDistance <= Epsilon
                    : currentDistance >= -Epsilon;

            bool nextInside = removeSide ==
                RemovalSide.Positive
                    ? nextDistance <= Epsilon
                    : nextDistance >= -Epsilon;

            if (currentInside && nextInside)
            {
                result.Add(next);
            }
            else if (currentInside && !nextInside)
            {
                result.Add(
                    Intersect(
                        current,
                        next,
                        currentDistance,
                        nextDistance));
            }
            else if (!currentInside && nextInside)
            {
                result.Add(
                    Intersect(
                        current,
                        next,
                        currentDistance,
                        nextDistance));

                result.Add(next);
            }
        }

        return result;
    }

    private static VertexData Intersect(
        VertexData a,
        VertexData b,
        float distanceA,
        float distanceB)
    {
        float denominator = distanceA - distanceB;

        float t = Mathf.Abs(denominator) < Epsilon
            ? 0f
            : Mathf.Clamp01(distanceA / denominator);

        return VertexData.Lerp(a, b, t);
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

        EditorGUILayout.Space(12);

        DirectionalMeshCutter cutter =
            (DirectionalMeshCutter)target;

        if (cutter.targetGameObject == null)
        {
            EditorGUILayout.HelpBox(
                "Drag the model you want to cut into " +
                "Target GameObject.",
                MessageType.Info);
        }

        GUI.enabled =
            cutter.targetGameObject != null &&
            !EditorApplication.isPlaying;

        if (GUILayout.Button(
            "CUT TARGET AND EXPORT FBX",
            GUILayout.Height(40)))
        {
            cutter.CutTargetAndExport();
        }

        GUI.enabled = true;

        EditorGUILayout.HelpBox(
            "The original FBX and scene GameObject " +
            "will not be modified. " +
            "The cut model will be exported to the " +
            "specified folder.",
            MessageType.None);
    }
}

#endif
