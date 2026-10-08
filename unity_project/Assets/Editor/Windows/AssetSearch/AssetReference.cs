using System.Collections.Generic;
using UnityEngine;

public class AssetReferenceGroup
{
    public List<AssetReference> References { get; private set; }
    public Object Asset { get; private set; }
    public bool Foldout { get; set; }

    public AssetReferenceGroup(Object asset, List<AssetReference> allReferences)
    {
        References = allReferences.FindAll(r => r.Asset == asset);
        References.Sort(Comparisons.GetAlphanumeric<AssetReference>(x => x.Path));
        Asset = asset;
        Foldout = false;
    }
}

public class AssetReference
{
    public Object Asset { get; set; }
    public string Path { get; set; }

    public AssetReference(Object asset, string path)
    {
        Asset = asset;
        Path = path;
    }
}
