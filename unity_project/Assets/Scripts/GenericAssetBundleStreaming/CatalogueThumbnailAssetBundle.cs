using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueThumbnailAssetBundle : GenericAssetBundle<Texture2D>
{
    public override string AssetBundleLocation => "catalogue/cataloguethumbnail";

    public static CatalogueThumbnailAssetBundle Instance { get; private set; }

    public static void CreateInstance(Transform root)
    {
        if (null == Instance)
        {
            Instance = new CatalogueThumbnailAssetBundle();
        }
    }

}
