
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CircularWall : MonoBehaviour
{
    [Header("Wall Settings")]
    [Min(0.1f)]
    public float radius = 2f;

    [Min(0.1f)]
    public float height = 2f;

    [Range(3, 128)]
    public int segments = 32;

    [Header("Material")]
    [SerializeField]
    private Material wallMaterial;

    [Header("Texture Tiling")]
    [Min(0.01f)]
    public float textureTilingX = 1f;

    [Min(0.01f)]
    public float textureTilingY = 1f;

    [Header("Generation")]
    public bool generateOnValidate = false;

    private Mesh generatedMesh;

    private void OnValidate()
    {
        if (generateOnValidate)
        {
            // Delay generation to avoid modifying
            // meshes during Unity's validation pass.
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall +=
                DelayedGenerate;
#endif
        }
    }

#if UNITY_EDITOR
    private void DelayedGenerate()
    {
        if (this == null || !generateOnValidate)
            return;

        GenerateWall();
    }
#endif

    [ContextMenu("Generate Wall")]
    public void GenerateWall()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "Circular Wall";

        int vertexCount = (segments + 1) * 2;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];

        // Double-sided wall: 4 triangles per segment.
        int[] triangles = new int[segments * 12];

        float circumference = 2f * Mathf.PI * radius;

        for (int i = 0; i <= segments; i++)
        {
            float normalized = i / (float)segments;
            float angle = normalized * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            int index = i * 2;

            vertices[index] =
                new Vector3(x, 0f, z);

            vertices[index + 1] =
                new Vector3(x, height, z);

            float u =
                normalized *
                circumference *
                textureTilingX;

            uv[index] =
                new Vector2(u, 0f);

            uv[index + 1] =
                new Vector2(
                    u,
                    height * textureTilingY
                );
        }

        for (int i = 0; i < segments; i++)
        {
            int v = i * 2;
            int t = i * 12;

            // Outside surface
            triangles[t] = v;
            triangles[t + 1] = v + 1;
            triangles[t + 2] = v + 2;

            triangles[t + 3] = v + 1;
            triangles[t + 4] = v + 3;
            triangles[t + 5] = v + 2;

            // Inside surface
            triangles[t + 6] = v + 2;
            triangles[t + 7] = v + 1;
            triangles[t + 8] = v;

            triangles[t + 9] = v + 2;
            triangles[t + 10] = v + 3;
            triangles[t + 11] = v + 1;
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // Remove the previously generated mesh.
        Mesh oldMesh = generatedMesh;

        generatedMesh = mesh;
        meshFilter.sharedMesh = generatedMesh;

        if (oldMesh != null && oldMesh != mesh)
        {
            if (Application.isPlaying)
                Destroy(oldMesh);
            else
                DestroyImmediate(oldMesh);
        }

        // Assign material
        if (wallMaterial != null)
        {
            meshRenderer.sharedMaterial = wallMaterial;
        }
    }

    [ContextMenu("Clear Wall")]
    public void ClearWall()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (meshFilter.sharedMesh == generatedMesh)
            meshFilter.sharedMesh = null;

        if (generatedMesh != null)
        {
            if (Application.isPlaying)
                Destroy(generatedMesh);
            else
                DestroyImmediate(generatedMesh);

            generatedMesh = null;
        }
    }
}
