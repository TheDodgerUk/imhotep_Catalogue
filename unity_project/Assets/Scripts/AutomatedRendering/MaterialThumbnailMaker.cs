using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

public class MaterialThumbnailMaker : MonoBehaviour
{

    private string DIRECTORY => $"{Application.dataPath}/AssetBundles/{PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME_SHORT)}/{nameof(Texture2DAssetBundle)}";

    private readonly int SPEC_HIGHLIGHTS_ID = Shader.PropertyToID("_SpecularHighlights");
    [SerializeField]
    public Camera m_Camera;

    private RenderTexture m_RenderTexture;

    public readonly Vector3 CUBE_PLACE = new Vector3(0f, 0f, 0.85f);
    
    private List<string> MaterialItems = new List<string>();
    private List<string> VideoClipItems = new List<string>();

    [SerializeField]
    public GameObject m_Cube;
    private Renderer m_Renderer;
    private Dictionary<string, Material> m_MaterialDictionary = new Dictionary<string, Material>();
    private Dictionary<string, VideoClip> m_VideoClipDictionary = new Dictionary<string, VideoClip>();

    private RenderTexture m_Output;
    // Start is called before the first frame update
    private void Awake()
    {
        UnityEngine.XR.XRSettings.enabled = false;

        m_Camera.transform.position = Vector3.zero;
        m_Cube.transform.position = CUBE_PLACE;
        m_Camera.fieldOfView = 60;

        var res = new Vector2(1024,1024);
        m_Output = new RenderTexture((int)res.x, (int)res.y, 0, RenderTextureFormat.ARGB32)
        {
            filterMode = FilterMode.Trilinear,
            wrapMode = TextureWrapMode.Clamp,
            autoGenerateMips = true,
            useMipMap = true,
            anisoLevel = 8,
            name = gameObject.name
        };
        m_Output.Create();



        Texture2DAssetBundle.CreateInstance(this.transform);
        MaterialAssetBundle.CreateInstance(this.transform);
        VideoClipAssetBundle.CreateInstance(this.transform);

#if UNITY_EDITOR
        if(EditorUtility.DisplayDialog("Build", "Bundles have to be built as it uses the Built bundles, do you want to want to continue", "Yes", "No") == false)
        {
            UnityEditor.EditorApplication.isPlaying = false;
            return;
        }
#endif

        DeleteOldThumbnails();
        ProcessAllThumbnails();
    }

    private void DeleteOldThumbnails()
    {
        var files = Directory.GetFiles(DIRECTORY);
        for(int i = 0; i < files.Length; i++)
        {
            string fileName = Path.GetFileName(files[i]);
            if (fileName.StartsWith(Texture2DAssetBundle.THUMBNAIL_MATERIAL_PREFIX) || fileName.StartsWith(Texture2DAssetBundle.THUMBNAIL_VIDEO_PREFIX))
            {
                File.Delete(files[i]);
            }
        }
    }

    private void OnDestroy()
    {
        UnityEngine.XR.XRSettings.enabled = true;
    }


    private void ProcessAllThumbnails()
    {
        Debug.Log("Processing Catalogue");

        StartCoroutine(Process(() =>
        {
#if UNITY_EDITOR

            EditorUtility.DisplayDialog("Finished", "Items have been created", "Yes");
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }));
    }


    private IEnumerator Process(Action callback)
    {
        m_Renderer = m_Cube.GetComponent<Renderer>();

        yield return new WaitForEndOfFrame();

        TaskAction taskAll = new TaskAction(2, () =>
        {
            StartCoroutine(CollectAll(callback));
        });

        MaterialAssetBundle.Instance.GetItemList(this, (list) =>
        {
            MaterialItems = list;
            taskAll.Increment();
        });

        VideoClipAssetBundle.Instance.GetItemList(this, (list) =>
        {
            VideoClipItems = list;
            taskAll.Increment();
        });
    }

    private IEnumerator CollectAll(Action callback)
    {
        yield return new WaitForEndOfFrame();

        TaskAction taskAll = new TaskAction(MaterialItems.Count + m_VideoClipDictionary.Count, () =>
        {
            StartCoroutine(ScreenShotAll(callback));
        });

        foreach (var item in MaterialItems)
        {
            MaterialAssetBundle.Instance.GetItem(this, item, (mat) =>
            {
                m_MaterialDictionary.Add(item, mat);
                taskAll.Increment();
            });
        }

        foreach (var item in VideoClipItems)
        {
            VideoClipAssetBundle.Instance.GetItem(this, item, (vid) =>
            {
                m_VideoClipDictionary.Add(item, vid);
                taskAll.Increment();
            });
        }
    }

    private IEnumerator ScreenShotAll(Action callback)
    {
        yield return new WaitForEndOfFrame();
        foreach(var item in m_MaterialDictionary)
        {
            m_Renderer.material = item.Value;
            var HighlightValue = m_Renderer.material.GetFloat(SPEC_HIGHLIGHTS_ID);   // get the part that has the bright spot on it 
            m_Renderer.material.SetFloat(SPEC_HIGHLIGHTS_ID, 0);                     // disables it
            yield return new WaitForEndOfFrame();
            CreateScreenShot(Texture2DAssetBundle.THUMBNAIL_MATERIAL_PREFIX, item.Key);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            m_Renderer.material.SetFloat(SPEC_HIGHLIGHTS_ID, HighlightValue);        // puts it back in 
        }



        var vidPlayer = m_Renderer.gameObject.ForceComponent<VideoPlayer>();
        vidPlayer.targetTexture = m_Output;

        m_Renderer.material = Resources.Load<Material>("ConsoleScreen");
        m_Renderer.material.mainTexture = m_Output;
        yield return new WaitForEndOfFrame();
        foreach (var item in m_VideoClipDictionary)
        {
            vidPlayer.clip = item.Value;
            
            float  Y = ((float)vidPlayer.clip.height) / ((float)vidPlayer.clip.width);
            if (Y < 1)
            {
                m_Renderer.gameObject.transform.localScale = new Vector3(1, Y, 1);
            }
            else
            {
                m_Renderer.gameObject.transform.localScale = new Vector3(1.0f /Y, 1, 1);
            }

            vidPlayer.source = VideoSource.VideoClip;
            vidPlayer.renderMode = UnityEngine.Video.VideoRenderMode.MaterialOverride;
            vidPlayer.aspectRatio = VideoAspectRatio.FitInside;

            yield return new WaitForEndOfFrame();
            vidPlayer.Prepare();
            while(vidPlayer.isPrepared == false)
            {
                yield return new WaitForEndOfFrame();
            }

            float frameFloat = ((float)vidPlayer.frameCount) * 0.75f;
            vidPlayer.frame = (int)frameFloat;
            vidPlayer.Play();
            vidPlayer.frame = (int)frameFloat;
            yield return new WaitForSeconds(2);        
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            CreateScreenShot(Texture2DAssetBundle.THUMBNAIL_VIDEO_PREFIX, item.Key);
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
        }

        callback?.Invoke();
    }

    private void CreateScreenShot(string prefix, string name)
    {
        RenderTexture tex = new RenderTexture(ThumbnailMaker.TEXTURESIZE, ThumbnailMaker.TEXTURESIZE, 0);

        m_Camera.forceIntoRenderTexture = true;
        m_Camera.targetTexture = tex;

        Utils.IO.ForceFolder(DIRECTORY);
        TakeAndSaveScreenShot(tex, $"{DIRECTORY}/", $"{prefix}{name}.png");
        m_Camera.targetTexture = m_RenderTexture;
    }


    private void TakeAndSaveScreenShot(RenderTexture renderTexture, string filePath, string fileName)
    {
        //Copy render Texture to texture
        RenderTexture.active = renderTexture;

        m_Camera.Render();

        Texture2D myTexture2D = new Texture2D(ThumbnailMaker.TEXTURESIZE, ThumbnailMaker.TEXTURESIZE);
        myTexture2D.ReadPixels(new Rect(0, 0, ThumbnailMaker.TEXTURESIZE, ThumbnailMaker.TEXTURESIZE), 0, 0);
        myTexture2D.Apply();
        try
        {
            byte[] bytes = myTexture2D.EncodeToPNG();

            System.IO.File.WriteAllBytes($"{filePath}{fileName}", bytes);
            Debug.Log($"{filePath}{fileName}");
        }
        catch(Exception e)
        {
            Debug.LogError($"Could not generate texture {filePath}{fileName}");
        }
    }

}