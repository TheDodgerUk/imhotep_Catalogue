using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoClipStreamingAssetInformation : ScriptableObject
{
    public string URL;
}

public class VideoClipStreamingAsset : GenericStreamingAsset<VideoClipStreamingAssetInformation>
{
    public static VideoClipStreamingAsset Instance { get; private set; }

    protected override List<string> ValidExtensions => new List<string>() { "*.mp4", "*.mov" };
    protected override string STREAMING_FOLDER => $"sss/VideoClipStreamingAsset".ToLower();

    public static void CreateInstance()
    {
        if (null == Instance)
        {
            Instance = new VideoClipStreamingAsset();
            Instance.Setup();
        }
    }


    protected override IEnumerator RequestStreamingRoutine(string fullName, Action<VideoClipStreamingAssetInformation> callback = null)
    {
        string cleanName = fullName.Replace(STREAMING_FOLDER, "");
        cleanName = cleanName.Remove(0, 1);

        string fullPath = "file:///" + fullName;

        string url = fullPath;
        var list = Directory.GetFiles(STREAMING_FOLDER).ToList();
        foreach(var item in list)
        {
            string lastpart = item.Replace(STREAMING_FOLDER, "");
            lastpart = lastpart.Remove(0, 1);
            if(lastpart.EndsWith(".meta") == false)
            {
                string extention = Path.GetExtension(lastpart);
                Debug.Log(lastpart);
                string fileNameComapre = System.IO.Path.GetFileNameWithoutExtension(lastpart);
                if(fileNameComapre == cleanName)
                {
                    url = fullPath + extention;
                    break;
                }
            }

        }

        VideoClipStreamingAssetInformation scriptableItem = ScriptableObject.CreateInstance<VideoClipStreamingAssetInformation>();
        scriptableItem.name = fullPath;
        scriptableItem.URL = url;
        callback?.Invoke(scriptableItem);
        yield return null;
    }
}

