#if UNITY_EDITOR
#if CATALOG_PROGRAM
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[ExecuteInEditMode]
public class Video360Validation : MonoBehaviour
{
    public string m_VideoPlayerName;
    public string m_PreloadedVideoName;
    public VideoAudioOutputMode m_VideoMode = VideoAudioOutputMode.Direct;

    [InspectorButton]
    private void Awake()
    {
        string taggedName = "";
        EnvironmentHelpers.Helpers.GetNamedTagged(this.gameObject, ref taggedName);
        m_VideoPlayerName = EnvironmentHelpers.Helpers.GetObjectName(this.gameObject);
        m_PreloadedVideoName = EnvironmentHelpers.Helpers.GetObjectNameExtentionArguments(this.gameObject, 0);
        m_VideoMode =(VideoAudioOutputMode)System.Enum.Parse(typeof(VideoAudioOutputMode), EnvironmentHelpers.Helpers.GetObjectNameExtentionArguments(this.gameObject, 1));
    }

    [InspectorButton]
    private void CreateGameObjectName()
    {
        this.gameObject.name = $"[360Video]_{m_VideoPlayerName}_{m_PreloadedVideoName}_{m_VideoMode}";
    }
}
#endif
#endif
