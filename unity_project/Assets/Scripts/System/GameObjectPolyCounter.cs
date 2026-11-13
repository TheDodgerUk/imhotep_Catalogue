
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameObjectPolyCounter : MonoBehaviour
{
    [Header("Settings")]
    public bool includeChildren = true;
    public bool includeInactive = true;

    [HideInInspector] public int gameObjectCount;
    [HideInInspector] public int meshCount;
    [HideInInspector] public int uniqueMeshCount;
    [HideInInspector] public long triangleCount;
    [HideInInspector] public long vertexCount;
    [HideInInspector] public int subMeshCount;

    [HideInInspector] public int meshRendererCount;
    [HideInInspector] public int skinnedMeshRendererCount;
    [HideInInspector] public int materialCount;
    [HideInInspector] public int textureCount;

    [HideInInspector] public float estimatedMeshMemoryMB;

    [ContextMenu("Analyse GameObject")]
    public void Analyse()
    {
        gameObjectCount = 0;
        meshCount = 0;
        uniqueMeshCount = 0;
        triangleCount = 0;
        vertexCount = 0;
        subMeshCount = 0;
        meshRendererCount = 0;
        skinnedMeshRendererCount = 0;
        materialCount = 0;
        textureCount = 0;
        estimatedMeshMemoryMB = 0;

        HashSet<Mesh> uniqueMeshes = new HashSet<Mesh>();
        HashSet<Material> materials = new HashSet<Material>();
        HashSet<Texture> textures = new HashSet<Texture>();

        Transform[] objects = includeChildren
            ? GetComponentsInChildren<Transform>(includeInactive)
            : new Transform[] { transform };

        foreach (Transform t in objects)
        {
            gameObjectCount++;

            MeshFilter filter = t.GetComponent<MeshFilter>();

            if (filter != null && filter.sharedMesh != null)
                AddMesh(filter.sharedMesh, uniqueMeshes);

            SkinnedMeshRenderer skinned =
                t.GetComponent<SkinnedMeshRenderer>();

            if (skinned != null)
            {
                skinnedMeshRendererCount++;

                if (skinned.sharedMesh != null)
                    AddMesh(skinned.sharedMesh, uniqueMeshes);
            }

            MeshRenderer renderer = t.GetComponent<MeshRenderer>();

            if (renderer != null)
                meshRendererCount++;

            Renderer[] renderers = t.GetComponents<Renderer>();

            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (mat == null)
                        continue;

                    materials.Add(mat);

                    Shader shader = mat.shader;

                    if (shader == null)
                        continue;

                    int count = shader.GetPropertyCount();

                    for (int i = 0; i < count; i++)
                    {
                        if (shader.GetPropertyType(i) !=
                            UnityEngine.Rendering.ShaderPropertyType.Texture)
                            continue;

                        string property = shader.GetPropertyName(i);
                        Texture tex = mat.GetTexture(property);

                        if (tex != null)
                            textures.Add(tex);
                    }
                }
            }
        }

        uniqueMeshCount = uniqueMeshes.Count;
        materialCount = materials.Count;
        textureCount = textures.Count;

        long memoryBytes = 0;

        foreach (Mesh mesh in uniqueMeshes)
        {
            // Approximate mesh memory.
            memoryBytes += (long)mesh.vertexCount * 32;

            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                memoryBytes += (long)mesh.GetIndexCount(i) *
                    (mesh.indexFormat ==
                     UnityEngine.Rendering.IndexFormat.UInt32 ? 4 : 2);
            }
        }

        estimatedMeshMemoryMB =
            memoryBytes / (1024f * 1024f);

        Debug.Log(
            $"Statistics for: {gameObject.name}\n" +
            $"GameObjects: {gameObjectCount:N0}\n" +
            $"Meshes: {meshCount:N0}\n" +
            $"Unique Meshes: {uniqueMeshCount:N0}\n" +
            $"Triangles: {triangleCount:N0}\n" +
            $"Vertices: {vertexCount:N0}\n" +
            $"Submeshes: {subMeshCount:N0}\n" +
            $"MeshRenderers: {meshRendererCount:N0}\n" +
            $"SkinnedMeshRenderers: {skinnedMeshRendererCount:N0}\n" +
            $"Materials: {materialCount:N0}\n" +
            $"Textures: {textureCount:N0}\n" +
            $"Estimated Mesh Memory: {estimatedMeshMemoryMB:F2} MB",
            gameObject
        );
    }

    private void AddMesh(Mesh mesh, HashSet<Mesh> uniqueMeshes)
    {
        meshCount++;
        vertexCount += mesh.vertexCount;
        subMeshCount += mesh.subMeshCount;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            if (mesh.GetTopology(i) == MeshTopology.Triangles)
                triangleCount += (long)mesh.GetIndexCount(i) / 3;
        }

        uniqueMeshes.Add(mesh);
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(GameObjectPolyCounter))]
public class GameObjectPolyCounterEditor : Editor
{
    private static readonly CultureInfo NumberCulture =
        CultureInfo.GetCultureInfo("en-GB");

    public override void OnInspectorGUI()
    {
        GameObjectPolyCounter counter =
            (GameObjectPolyCounter)target;

        serializedObject.Update();

        // Editable settings
        EditorGUILayout.LabelField(
            "Settings",
            EditorStyles.boldLabel
        );

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("includeChildren"));

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty("includeInactive"));

        serializedObject.ApplyModifiedProperties();

        GUILayout.Space(10);

        if (GUILayout.Button(
            "ANALYSE GAMEOBJECT",
            GUILayout.Height(35)))
        {
            counter.Analyse();
            EditorUtility.SetDirty(counter);
        }

        GUILayout.Space(15);

        // Read-only mesh statistics
        EditorGUILayout.LabelField(
            "Mesh Statistics",
            EditorStyles.boldLabel
        );

        DrawNumber("GameObjects", counter.gameObjectCount);
        DrawNumber("Meshes", counter.meshCount);
        DrawNumber("Unique Meshes", counter.uniqueMeshCount);
        DrawNumber("Triangles", counter.triangleCount);
        DrawNumber("Vertices", counter.vertexCount);
        DrawNumber("Submeshes", counter.subMeshCount);

        GUILayout.Space(10);

        // Read-only renderer statistics
        EditorGUILayout.LabelField(
            "Renderer Statistics",
            EditorStyles.boldLabel
        );

        DrawNumber("Mesh Renderers", counter.meshRendererCount);
        DrawNumber("Skinned Mesh Renderers",
            counter.skinnedMeshRendererCount);
        DrawNumber("Materials", counter.materialCount);
        DrawNumber("Textures", counter.textureCount);

        GUILayout.Space(10);

        // Read-only memory statistics
        EditorGUILayout.LabelField(
            "Memory",
            EditorStyles.boldLabel
        );

        DrawText(
            "Estimated Mesh Memory",
            counter.estimatedMeshMemoryMB.ToString(
                "N2", NumberCulture) + " MB"
        );
    }

    private static void DrawNumber(string label, long value)
    {
        DrawText(
            label,
            value.ToString("N0", NumberCulture)
        );
    }

    private static void DrawText(string label, string value)
    {
        EditorGUI.BeginDisabledGroup(true);

        EditorGUILayout.TextField(label, value);

        EditorGUI.EndDisabledGroup();
    }
}

#endif
