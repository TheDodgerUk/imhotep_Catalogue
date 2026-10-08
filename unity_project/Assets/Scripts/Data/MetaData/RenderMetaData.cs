using FilePathHelper;
using System.Collections.Generic;
using UnityEngine;

[MetaData(MetaDataType.Render)]
public class RenderMetaData : MetaData
{
    public RenderItemData Main;
    public List<RenderItemData> Parts;
    public List<RenderItemData> Variants;
    public int Version;
    public float[] SizeOfFileMB = new float[GlobalConsts.PlatformNamesLength];

    public RenderMetaData()
    {
        Variants = new List<RenderItemData>();
        Parts = new List<RenderItemData>();
        Main = new RenderItemData();
    }

    public override void OnPostLoad(CatalogueEntry entry)
    {
        base.OnPostLoad(entry);

        Main.OnPostLoad();
        Parts.ForEach(p => p.OnPostLoad());
        Variants.ForEach(p => p.OnPostLoad());
    }
}

public class RenderItemData
{
    public string Guid { get; set; }

    public string Bundle;
    public string Prefab;

    public void OnPostLoad()
    {
        Guid = GetPrefabGuid(Bundle, Prefab);
    }

    private static string GetPrefabGuid(string bundleName, string prefabName)
    {
#if UNITY_EDITOR
        var markedAssets = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(bundleName).ToList();
        string path = markedAssets.Find(p => FilePath.FromRelative(p).FileName == $"{prefabName}.prefab");
        if (string.IsNullOrEmpty(path) == false)
        {
            return UnityEditor.AssetDatabase.AssetPathToGUID(path);
        }
#endif
        return null;
    }
}

public class EditorRenderData
{
    public readonly string Main;

    public readonly List<string> Parts;
    public readonly List<string> Variants;

    public EditorRenderData() { }

    public EditorRenderData(RenderMetaData renderMetaData)
    {
        Main = renderMetaData.Main.Guid;
        Parts = renderMetaData.Parts.Extract(r => r.Guid);
        Variants = renderMetaData.Variants.Extract(r => r.Guid);
    }

    public void Apply(RenderMetaData renderMetaData, CatalogueEntry entry)
    {
        Apply(renderMetaData.Main, Main, entry);

        for (int i = 0; i < renderMetaData.Parts.Count; ++i)
        {
            Apply(renderMetaData.Parts[i], Parts[i], entry);
        }
        for (int i = 0; i < renderMetaData.Variants.Count; ++i)
        {
            Apply(renderMetaData.Variants[i], Variants[i], entry);
        }
    }

    private static void Apply(RenderItemData renderItem, string guid, CatalogueEntry entry)
    {
#if UNITY_EDITOR
        var prefab = LoadPrefab(guid);
        if (prefab != null)
        {
            renderItem.Guid = guid;
            renderItem.Prefab = prefab.name;
            renderItem.Bundle = GetAssetBundleName(guid);
        }
#endif
    }

#if UNITY_EDITOR
    public static GameObject LoadPrefab(string guid) => UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(GetAssetPath(guid).Relative);
    public static FilePath GetAssetPath(string guid) => FilePath.FromRelative(UnityEditor.AssetDatabase.GUIDToAssetPath(guid));
    public static string GetAssetBundleName(string guid) => GetAssetBundleName(GetAssetPath(guid));
    public static string GetAssetBundleName(FilePath assetPath)
    {
        var importer = UnityEditor.AssetImporter.GetAtPath(assetPath.Relative);
        string bundleName = importer?.assetBundleName;
        if (string.IsNullOrEmpty(bundleName) == false)
        {
            return bundleName;
        }
        else
        {
            var folder = assetPath.GetFolder();
            if (folder.Absolute == assetPath.Absolute)
            {
                return null;
            }
            return GetAssetBundleName(folder);
        }
    }
#endif
}