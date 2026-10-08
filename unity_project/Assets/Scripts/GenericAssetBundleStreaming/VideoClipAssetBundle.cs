using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoClipAssetBundle : GenericAssetBundle<VideoClip>
{
    protected override string ASSET_TYPE => nameof(VideoClipAssetBundle);

    public static VideoClipAssetBundle Instance { get; private set; }

    public static void CreateInstance(Transform root)
    {
        if (null == Instance)
        {
            Instance = new VideoClipAssetBundle();
        }
    }
}
