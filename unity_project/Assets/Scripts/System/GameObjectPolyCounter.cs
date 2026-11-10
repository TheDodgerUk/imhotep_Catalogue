
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameObjectPolyCounter : MonoBehaviour
{
    [Header("Settings")]
    public bool includeChildren = true;
    public bool includeInactive = true;

    [Header("Mesh Statistics")]
    public int gameObjectCount;
    public int meshCount;
    public int uniqueMeshCount;
    public long triangleCount;
    public long vertexCount;
    public int subMeshCount;

    [Header("Renderer Statistics")]
    public int meshRendererCount;
    public int skinnedMeshRendererCount;
    public int materialCount;
    public int textureCount;

    [Header("Memory")]
    public float estimatedMeshMemoryMB;

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
            // Approximate CPU-side mesh data size.
            memoryBytes += (long)mesh.vertexCount * 32;

            for (int i = 0; i < mesh.subMeshCount; i++)
                memoryBytes += (long)mesh.GetIndexCount(i) *
                    (mesh.indexFormat ==
                     UnityEngine.Rendering.IndexFormat.UInt32 ? 4 : 2);
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
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        GameObjectPolyCounter counter =
            (GameObjectPolyCounter)target;

        if (GUILayout.Button("ANALYSE GAMEOBJECT",
            GUILayout.Height(35)))
        {
            counter.Analyse();
            EditorUtility.SetDirty(counter);
        }
    }
}
#endif
