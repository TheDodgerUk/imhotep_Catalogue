using JsonPlugin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAssetBundle : GenericAssetBundle<TextAsset>
{
    protected override string ASSET_TYPE => nameof(TextAssetBundle);
    public static TextAssetBundle Instance { get; private set; }

    public static void CreateInstance(Transform root)
    {
        if (null == Instance)
        {
            Instance = new TextAssetBundle();
        }
    }
}

