using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture2DAssetBundle : GenericAssetBundle<Texture2D>
{
    public const string THUMBNAIL_MATERIAL_PREFIX = "ThumbnailMaterial_";
    public const string THUMBNAIL_VIDEO_PREFIX = "ThumbnailVideo_";

    protected override string ASSET_TYPE => nameof(Texture2DAssetBundle);

    public static Texture2DAssetBundle Instance { get; private set; }

    public static void CreateInstance(Transform root)
    {
        if (null == Instance)
        {
            Instance = new Texture2DAssetBundle();
        }
    }
}
