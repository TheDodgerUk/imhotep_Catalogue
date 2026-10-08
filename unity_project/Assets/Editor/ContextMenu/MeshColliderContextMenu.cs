using UnityEditor;
using UnityEngine;

public static class MeshColliderContextMenu
{
    [MenuItem("CONTEXT/MeshCollider/Convert to Box", false, 0)]
    public static void ConvertToBox(MenuCommand command)
    {
        var meshCollider = command.context as MeshCollider;
        if (meshCollider != null)
        {
            var box = Undo.AddComponent<BoxCollider>(meshCollider.gameObject);
            box.sharedMaterial = meshCollider.sharedMaterial;
            box.isTrigger = meshCollider.isTrigger;
            box.center = meshCollider.bounds.center;
            box.size = meshCollider.bounds.size;

            Undo.DestroyObjectImmediate(meshCollider);
        }
    }

    [MenuItem("CONTEXT/MeshCollider/Add Renderer", false, 0)]
    public static void AddRenderer(MenuCommand command)
    {
        var meshCollider = command.context as MeshCollider;
        if (meshCollider != null)
        {
            var existingFilter = meshCollider.GetComponent<MeshFilter>();
            var existingRenderer = meshCollider.GetComponent<Renderer>();

            if (existingFilter != null)
            {
                Undo.DestroyObjectImmediate(existingFilter);
            }
            if (existingRenderer != null)
            {
                Undo.DestroyObjectImmediate(existingRenderer);
            }

            var filter = Undo.AddComponent<MeshFilter>(meshCollider.gameObject);
            var renderer = Undo.AddComponent<MeshRenderer>(meshCollider.gameObject);
            var material = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");

            var mesh = meshCollider.sharedMesh;
            filter.sharedMesh = mesh;

            var materials = new Material[mesh.subMeshCount];
            for (int i = 0; i < materials.Length; ++i)
            {
                materials[i] = material;
            }
            renderer.sharedMaterials = materials;
        }
    }
}
