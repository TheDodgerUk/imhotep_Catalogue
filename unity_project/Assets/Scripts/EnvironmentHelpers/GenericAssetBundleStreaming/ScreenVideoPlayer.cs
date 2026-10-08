using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using EnvironmentHelpers;

/// <summary>
/// Bare bones video player, will autosetup a Video Player component on gameobject
/// this script is attached too.
/// 
/// Call Init to setup List of screens and provide a material or it will take the first material
/// from the screens provided.
/// </summary>
public class ScreenVideoPlayer : MonoBehaviour
{
    private Action OnVideoEnded;
    private Action OnVideoPrepared;
    private Material m_Material;
    private VideoPlayer m_VideoPlayer;
    private RenderTexture m_VideoTexture;

    private float m_VideoLength;
    public float Length => m_VideoLength;

    public void Init(List<GameObject> screens, Material material)
    {
        m_Material = material;
        Init(screens);
    }

    public void Init(List<GameObject> screens)
    {
        CreateTexture(new Vector2(512, 512));
        SetupVideoPlayer();
        foreach (GameObject screen in screens)
        {
            var renderer = screen.GetComponent<Renderer>();

            if (null != renderer)
            {
                if (null == m_Material)
                {
                    m_Material = new Material(renderer.sharedMaterial);
                }

                renderer.material = m_Material;
            }
        }

        SetupMaterial();
    }

    /// <summary>
    /// Plays the specified video using the VideoLoader. 
    /// NOTE: Video name should be without directory and extension
    /// 
    /// Provides an optional callback for when the video has completed its cycle
    /// 
    /// </summary>
    /// <param name="videoName"></param>
    /// <param name="onCompleteCallback"></param>
    public void PlayVideo(string videoName, Action onPrepared, Action onCompleteCallback)
    {
        OnVideoEnded = onCompleteCallback;
        OnVideoPrepared = onPrepared;

        //Get url from video loader
        
        string url = VideoLoader.Instance.GetVideo(videoName);
      
        if (false == string.IsNullOrEmpty(url))
        {
            SetVideoData(url);
        }
        else
        {
            VideoClipStreamingAsset.Instance.GetItem(this, videoName, (item) =>
            {
                url = item.URL;
                if (false == string.IsNullOrEmpty(url))
                {
                    SetVideoData(url);
                }
                else
                {
                    Debug.LogError($"Cannont find video: {videoName}", this);
                    OnVideoEnded?.Invoke();
                    //Cannot play video... does not exist, callback instantly
                }
            });
        }
    }

    private void SetVideoData(string url)
    {
        m_VideoPlayer.url = url;
        m_VideoPlayer.source = VideoSource.Url;
        m_VideoPlayer.Prepare();
    }

    /// <summary>
    /// Use in editor only... allow user to enter manually the URL and simply run the video
    /// </summary>
    [InspectorButton]
    public void PlayDebugVideo()
    {
        m_VideoPlayer.Prepare();
    }

    public void StopVideo()
    {
        m_VideoPlayer.frame = 0;
        m_VideoPlayer.Stop();
    }

    private void OnPrepareCompleted(VideoPlayer player)
    {
        m_VideoLength = (float)player.length;
        OnVideoPrepared?.Invoke();
        player.Play();
    }

    private void SetupMaterial()
    {
        m_Material.SetTexture("_MainTex", m_VideoTexture);
        m_Material.SetTexture("_EmissionMap", m_VideoTexture);
    }

    private void CreateTexture(Vector2 res)
    {
        m_VideoTexture = new RenderTexture((int)res.x, (int)res.y, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp,
            autoGenerateMips = true,
            useMipMap = true,
            name = gameObject.name,
        };
    }

    private void SetupVideoPlayer()
    {
        m_VideoPlayer = gameObject.ForceComponent<VideoPlayer>();
        m_VideoPlayer.playOnAwake = false;
        m_VideoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        m_VideoPlayer.renderMode = VideoRenderMode.RenderTexture;
        m_VideoPlayer.aspectRatio = VideoAspectRatio.Stretch;
        m_VideoPlayer.source = VideoSource.Url;

        m_VideoPlayer.prepareCompleted += OnPrepareCompleted;
        m_VideoPlayer.loopPointReached += (p) => { OnVideoEnded?.Invoke(); };
        m_VideoPlayer.targetTexture = m_VideoTexture;
    }
}
