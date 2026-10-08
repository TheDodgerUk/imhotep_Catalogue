using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialAssetBundle : GenericAssetBundle<Material>
{
    protected override  string ASSET_TYPE => nameof(MaterialAssetBundle);

    public static MaterialAssetBundle Instance { get; private set; }

    public static void CreateInstance(Transform root)
    {
        if (null == Instance)
        {
            Instance = new MaterialAssetBundle();
        }
    }


}
