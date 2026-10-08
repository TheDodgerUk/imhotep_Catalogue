using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class VideoObjectData
{
    public readonly string ScreenName;
    public readonly List<string> VideoNames;
    public readonly string RemappedScreen;   //After the intro should this screen use the video for another screen?
    public readonly bool ChangeVideoOnCompleted;
    public readonly bool MirrorImage;
    public readonly string StartVideo;
    public readonly Vector2 Resolution; 
}

public class SequenceData
{
    public readonly string TargetScreen;
    public readonly string VideoName;
}

public class VideoSequence
{
    //The list of possible changes
    public List<SequenceData> Sequence = new List<SequenceData>();
}

public class VideoSequenceData
{
    //The screen that will trigger the sequence change
    public string ControllingScreen;

    //Data for a particular sequence
    public List<VideoSequence> SequenceList = new List<VideoSequence>();
}

public class VideoScreenConfiguration
{
    public Dictionary<string, VideoObjectData> m_EnvironmentVideoData;

    public VideoScreenConfiguration(Dictionary<string, VideoObjectData> data)
    {
        m_EnvironmentVideoData = data;
    }
}

public class VideoLoader : Singleton<VideoLoader>
{
    private List<string> VideoExtension = new List<string>() { "*.mov", "*mp4" };


    private static readonly string Location = $"{Device.StreamingPath}{PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME)}/Videos/";
    public string CurrentSequenceDataFolder => $"{Location}/VideoSequenceData";
    public string CurrentDataFolder => $"{Location}/VideoData";

    private List<VideoUrlData> m_UrlsByType;
    [SerializeField]
    private VideoScreenConfiguration m_VideoConfiguration;
    [SerializeField]
    private VideoSequenceData m_VideoSequenceData;


    public List<string> FileNames => m_UrlsByType.ConvertAll(x => x.FileName);
    public List<string> Urls => m_UrlsByType.ConvertAll(x => x.Url);


    public void LoadVideoData(string sEnvironment)
    {
        m_VideoSequenceData = null;
        if (true == File.Exists($"{CurrentSequenceDataFolder}.json"))
        {
            var videoSequenceData = Json.FullSerialiser.ReadFromFile<VideoSequenceData>(CurrentSequenceDataFolder);
            m_VideoSequenceData = videoSequenceData;
        }
        else
        {
            Debug.Log($"not found {CurrentSequenceDataFolder}");
        }



        m_VideoConfiguration = null;

        if (true == File.Exists($"{CurrentDataFolder}.json"))
        {
            var videoObjectData = Json.JsonNet.ReadFromFile<Dictionary<string, VideoObjectData>>(CurrentDataFolder);
            if (null != videoObjectData)
            {
                m_VideoConfiguration = new VideoScreenConfiguration(videoObjectData);
            }
        }
    }

    public VideoObjectData GetScreenData(string screenName)
    {
        if (null != m_VideoConfiguration)
        {
            return m_VideoConfiguration.m_EnvironmentVideoData.Get(screenName);
        }

        return null;
    }

    public VideoSequenceData GetVideoSequenceData(string screenName)
    {
        if (m_VideoSequenceData != null && screenName == m_VideoSequenceData.ControllingScreen)
        {
            return m_VideoSequenceData;
        }

        return null;
    }

    public string GetRandomVideo(string type) => m_UrlsByType.GetRandom()?.Url;

    public string GetVideo(string name)
    {
        for(int i =0; i < m_UrlsByType.Count; i++)
        {
            if(m_UrlsByType[i].FileName == name)
            {
                return m_UrlsByType[i].Url;
            }
        }
        return string.Empty;
    }


    public void OnVideoSequenceLoaded(Action callback)
    {
        m_UrlsByType = new List<VideoUrlData>();

        string dir = $"{GlobalConsts.BUNDLE_STREAMING}/{PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME)}/{GlobalConsts.VIDEOCLIP_STREAMING}";
        List<string> videos = DirSearch(dir, VideoExtension);

        for (int i = 0; i < videos.Count; i++)
        {
            var videoData = new VideoUrlData()
            {
                FileName = Path.GetFileNameWithoutExtension(videos[i]),
                Url = videos[i],
            };

            m_UrlsByType.Add(videoData);
        }

        callback?.Invoke();
    }

    private List<String> DirSearch(string sDir, List<string> searchPtn)
    {
        List<String> files = new List<String>();
        try
        {
            foreach (string ptn in searchPtn)
            {
                foreach (string f in Directory.GetFiles(sDir, ptn))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d, searchPtn));
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogWarning($"Message: {e.Message}");
        }

        return files;
    }

    public class VideoUrlData
    {
        public string Url;
        public string FileName;
    }
}
