using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshCombiner : ScriptableWizard
{
    [Tooltip("Root gameobject of meshes.")]
    public GameObject meshesRoot;

    [Tooltip("Gameobject to save new combine mesh.")]
    public GameObject meshSave;

    [Tooltip("Save each LOD combined.")]
    public bool combineLODs;

    [MenuItem("Tools/Mesh Combiner &M")]
    private static void ShowEdit()
    {
        DisplayWizard("Mesh Combiner", typeof(MeshCombiner), "Combine");
    }

    private void OnWizardCreate()
    {
        var meshPath = EditorUtility.SaveFilePanelInProject("Save new Mesh", "CombinedMesh", "Asset", "Enter file name");

        if (string.Empty == meshPath)
            return;

        if (true == HasLods())
        {
            ProcessLODModel(meshPath);
        }
        else
        {
            ProcessNoneLodModel(meshPath);
        }
    }

    private void ProcessLODModel(string meshPath)
    {
        var lodMeshes = meshesRoot.GetComponentsInChildren<LODGroup>();
        Dictionary<int, List<Renderer>> dictOfLodsAndRenderes = new Dictionary<int, List<Renderer>>();
        float[] lodValues = new float[lodMeshes.Length];
        for (int i = 0; i < lodMeshes.Length; i++)
        {
            //Gather renderes for each lod
            LOD[] lods = lodMeshes[i].GetLODs();
            for (int x = 0; x < lods.Length; x++)
            {
                lodValues[x] = lods[x].screenRelativeTransitionHeight;
                if (false == dictOfLodsAndRenderes.ContainsKey(x))
                {
                    dictOfLodsAndRenderes.Add(x, new List<Renderer>());
                }

                dictOfLodsAndRenderes[x].AddRange(lods[x].renderers);
            }
        }

        LODGroup lodGroup = meshSave.ForceComponent<LODGroup>();
        lodGroup.SetLODs(new LOD[0]);

        foreach (KeyValuePair<int, List<Renderer>> kvp in dictOfLodsAndRenderes)
        {
            //Combine this lod together into a single mesh
            var combines = new CombineInstance[kvp.Value.Count];
            var materialList = new List<Material>();
            List<Renderer> renderers = kvp.Value;
            for (int i = 0; i < kvp.Value.Count; i++)
            {
                Transform transform = renderers[i].transform;
                MeshFilter meshFilter = renderers[i].GetComponent<MeshFilter>();
                combines[i].mesh = meshFilter.sharedMesh;
                combines[i].transform = Matrix4x4.TRS(transform.position - meshesRoot.transform.position,
                transform.rotation, transform.lossyScale);

                var materials = meshFilter.GetComponent<MeshRenderer>().sharedMaterials;
                foreach (var mat in materials)
                {
                    if (null != mat &&
                        false == materialList.Contains(mat))
                    {
                        materialList.Add(mat);
                    }
                }
            }

            var newMesh = new Mesh();
            Debug.Log("Combining LOD: " + kvp.Key + " | " + combines.Length + " models");
            newMesh.CombineMeshes(combines, true);

            //Create in scene
            GameObject child = CreateTransform(meshSave, newMesh, lodMeshes[kvp.Key], materialList, "LOD" + kvp.Key);

            string saveName = meshPath.Replace(".Asset", "_LOD" + kvp.Key + ".Asset");
            AssetDatabase.CreateAsset(newMesh, saveName);
            AssetDatabase.Refresh();
            Selection.activeObject = newMesh;

            Renderer[] combinedRenderers = child.GetComponents<Renderer>();

            List<LOD> lods = new List<LOD>(lodGroup.GetLODs());
            lods.Add(new LOD(lodValues[kvp.Key], combinedRenderers));
            lodGroup.SetLODs(lods.ToArray());
        }
    }

    private GameObject CreateTransform(GameObject parent, Mesh mesh, LODGroup lodData, List<Material> materials, string name)
    {
        GameObject child = new GameObject(name);
        child.transform.SetParent(parent.transform);
        child.transform.localPosition = Vector3.zero;
        child.transform.localRotation = Quaternion.identity;

        var filter = child.ForceComponent<MeshFilter>();
        var renderer = child.ForceComponent<MeshRenderer>();

        filter.sharedMesh = mesh;
        renderer.sharedMaterials = materials.ToArray();

        return child;
    }

    private void ProcessNoneLodModel(string meshPath)
    {
        var meshFilters = meshesRoot.GetComponentsInChildren<MeshFilter>();
        var combineAble = new List<CombineInstance>();
        var combines = new CombineInstance[meshFilters.Length];
        var materialList = new List<Material>();

        for (int i = 0; i < meshFilters.Length; i++)
        {
            Transform transform = meshFilters[i].transform;

            combines[i].mesh = meshFilters[i].sharedMesh;
            combines[i].transform = Matrix4x4.TRS(transform.position - meshesRoot.transform.position,
                transform.rotation, transform.lossyScale);
            var materials = meshFilters[i].GetComponent<MeshRenderer>().sharedMaterials;
            foreach (var mat in materials)
            {
                if (null != mat &&
                    false == materialList.Contains(mat))
                {
                    materialList.Add(mat);
                }
            }

            combineAble.Add(combines[i]);
        }

        GetTextures(materialList);

        var newMesh = new Mesh();
        Debug.Log("Combining: " + combineAble.Count + " models");
        newMesh.CombineMeshes(combineAble.ToArray(), true);

        var filter = meshSave.ForceComponent<MeshFilter>();
        var renderer = meshSave.ForceComponent<MeshRenderer>();

        filter.sharedMesh = newMesh;
        renderer.sharedMaterials = materialList.ToArray();

        AssetDatabase.CreateAsset(newMesh, meshPath);
        AssetDatabase.Refresh();
        Selection.activeObject = newMesh;
    }

    private bool HasLods()
    {
        var lodMeshes = meshesRoot.GetComponentsInChildren<LODGroup>();

        if (null != lodMeshes)
        {
            return lodMeshes.Length > 0;
        }

        return false;
    }

    private bool AllowModel(GameObject obj)
    {
        //dealing with LODS?
        if (true == obj.name.Contains("_LOD"))
        {
            if (false == combineLODs)
            {
                //Only process LOD0 models
                if (false == obj.name.Contains("_LOD0"))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void GetTextures(List<Material> materials)
    {
        List<Texture> textures = new List<Texture>();
        //foreach unique material
        for (int i = 0; i < materials.Count; i++)
        {
            //get texture and store
            textures.Add(materials[i].mainTexture);
        }

        Debug.Log("Texture count: " + textures.Count);
    }

    private void CombineTextures(List<Texture2D> textures)
    {

    }
}
