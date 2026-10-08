using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif
using System;
using TMPro;
using UnityEngine.Rendering.PostProcessing;
using System.IO;
using UnityEditor;
using UnityEngine.XR;

public class RecordSingle : MonoBehaviour
{
#if UNITY_EDITOR
    private PlayableDirector m_Director;
    private RecorderController m_RecorderController;
    private ImageRecorderSettings videoRecorder;
    private RecorderControllerSettings controllerSettings;
    private Transform m_RootTransform;
    private Transform m_RigRoot;
    private Transform TurnTable;
    private Vector3 m_RigOffset;

    [SerializeField]
    private GameObject m_CurrentViewedObject;
    private Camera m_Camera;

    private string OutputTempFolder = $@"C:\VideosImagesTEMP\Catalogue";
    private string OutputVideoDirectory = $@"C:\Videos\Catalogue";

    private TextMeshProUGUI m_ProductNameText;
    private PostProcessVolume m_PostVolume;
    private GameObject m_CreatedGameObject;
    private bool m_WithFullTextures = false;

    //private Stopwatch m_Stopwatch = new Stopwatch();


    private void Start()
    {
        SetInputSystemToVR(false);
        Localisation.CreateInstance();

        Utils.IO.ForceFolder(OutputTempFolder);
        Utils.IO.ForceFolder(OutputVideoDirectory);

        m_Camera = GetComponentInChildren<Camera>();

        m_PostVolume = m_Camera.GetComponent<PostProcessVolume>();

        m_RootTransform = transform;
        m_RigRoot = m_RootTransform.Search("Rig");
        TurnTable = m_RootTransform.Search("TurnTable");

        m_ProductNameText = transform.SearchComponent<TextMeshProUGUI>("ProductTitle");

        m_RigOffset = m_RigRoot.localPosition;

        Localisation.Instance.LoadLanguage(Localisation.Language.English);

        if (true == CreateItem())
        {
            m_WithFullTextures = EditorUtility.DisplayDialog("Render Type", "With textures, or without ", "With Textures", "No Textures");
            if (false == m_WithFullTextures)
            {
                var renderers = m_CreatedGameObject.GetComponentsInChildren<Renderer>();
                foreach(var item in renderers)
                {
                    item.material.mainTexture = null;
                    for (int i = 0; i < item.materials.Length; i++)
                    {
                        item.materials[i].mainTexture = null;
                    }                    
                }
                
            }

            m_ProductNameText.text = m_CurrentViewedObject.name;
            SetupVideoRecorder(() =>
            {
                CreateVideoSettings(m_ProductNameText.text);
                m_Director.Play();
                m_RecorderController.StartRecording();
                ApplyCameraDistance(m_CreatedGameObject);
                m_Director.stopped += OnFinishedAnimation;
            });
        }
    }

    private void SetInputSystemToVR(bool setToVR)
    {
        XRSettings.enabled = setToVR;
        UnityEngine.XR.InputTracking.disablePositionalTracking = !setToVR;
    }

    [InspectorButton]
    private bool CreateItem()
    {
        m_RootTransform = transform;
        m_RigRoot = m_RootTransform.Search("Rig");
        TurnTable = m_RootTransform.Search("TurnTable");

        if (m_CreatedGameObject != null)
        {
            Destroy(m_CreatedGameObject);
        }
        var all = FindObjectsOfType<Transform>();
        foreach(var item in all)
        {
            if(true == item.name.CaseInsensitiveContains("Clone"))
            {
                if (UnityEditor.EditorApplication.isPlaying == false)
                {
                    DestroyImmediate(item.gameObject);
                }
                else
                {
                    Destroy(item.gameObject);
                }
            }
        }
        if(m_CurrentViewedObject == null)
        {
            EditorUtility.DisplayDialog("Missing object to render", "in SpawnRoot_PlaceObject, you need to drag a object to Current Viewed Object", "OK");
            return false;
        }
        else
        {
            m_CreatedGameObject = Instantiate(m_CurrentViewedObject);
            m_CreatedGameObject.transform.SetParent(TurnTable.transform);
            m_CreatedGameObject.ClearLocals();
            CalculateCamera();
            return true;
        }
    }


    private void SetupVideoRecorder(Action callback)
    {
        controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        m_RecorderController = new RecorderController(controllerSettings);

        videoRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();

        videoRecorder.name = "Video";
        videoRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;

        videoRecorder.imageInputSettings = new CameraInputSettings
        {
            OutputWidth = 3840,
            OutputHeight = 2160,
            Source = ImageSource.ActiveCamera,
            CaptureUI = true
        };

        controllerSettings.SetRecordModeToManual();
        controllerSettings.FrameRate = 24.0f;

        controllerSettings.AddRecorderSettings(videoRecorder);
;

        m_Director = GetComponent<PlayableDirector>();
        
        
        Debug.Log("Setup recorder");
        callback?.Invoke();
    }


    [InspectorButton]
    private void CalculateCamera()
    {
        m_Camera = Camera.main;
        m_RigRoot = transform.Search("Rig");
        if (null != m_CurrentViewedObject)
        {
            ApplyCameraDistance(m_CurrentViewedObject);
        }
    }

    private void ApplyCameraDistance(GameObject focus)
    {
        Renderer[] renderer = GetRenderers(focus);
        focus.transform.position = Vector3.zero;

        Bounds bounds = new Bounds(focus.transform.position, Vector3.zero);
        for (int i = 0; i < renderer.Length; i++)
        {
            bounds.Encapsulate(renderer[i].bounds);
        }

        Vector3 xyz = bounds.size;
        float radius = xyz.magnitude / 2f;
        // Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(m_Camera.fieldOfView * Mathf.Deg2Rad / 2f) * m_Camera.aspect) * Mathf.Rad2Deg;
        // Use the smaller FOV as it limits what would get cut off by the frustum        
        float fov = Mathf.Min(m_Camera.fieldOfView, horizontalFOV);
        float dist = radius / (Mathf.Sin(fov * Mathf.Deg2Rad / 2f));

        DepthOfField depth = null;
        if (null != m_PostVolume)
        {
            if (true == m_PostVolume.profile.TryGetSettings<DepthOfField>(out depth))
            {
                if (depth.active == true)
                {
                    depth.focusDistance = new FloatParameter()
                    {
                        value = dist
                    };
                }
            }
        }

        m_Camera.transform.localPosition = new Vector3(0f, bounds.center.y, -dist);
        Vector3 centreDir = bounds.center - focus.transform.position;
        //focus.transform.localPosition = new Vector3(0f, centreDir.y, 0f);
        //
        //m_CurrentViewedObject.transform.position = m_RigRoot.position + (centreDir * 0.5f);
    }
    private Renderer[] GetRenderers(GameObject root)
    {
        Transform model = root.transform.Search("Model");
        if (model != null)
        {
            return model.GetComponentsInChildren<Renderer>();
        }
        else
        {
            return root.GetComponentsInChildren<Renderer>();
        }
    }


    private void UpdateCatalogueInfo(CatalogueEntry entry)
    {
        m_ProductNameText.text = entry.ShortName.Text;
    }


    void OnFinishedAnimation(PlayableDirector director)
    {
        m_RecorderController.StopRecording();

        m_RecorderController.Settings.Save();

        WaitForCommandProcess(OutputTempFolder, m_CurrentViewedObject.name);

        StartCoroutine(WaitForRecordingComplete(() =>
        {
            Debug.Log("Finished playing animation");
            UnityEditor.EditorApplication.isPlaying = false;
            EditorUtility.DisplayDialog("Compleated", $"Temp files at {OutputTempFolder}, and the video is being created at {OutputVideoDirectory}", "OK");
        }));
    }

    IEnumerator WaitForRecordingComplete(Action callback)
    {
        yield return new WaitForSeconds(20f);

        callback?.Invoke();
    }

    void WaitForCommandProcess(string outputTempLocation, string name)
    {
        string fullPath = Application.dataPath;
        fullPath = fullPath.Replace("/", "\\");
        fullPath = fullPath.Replace("/", @"\");


        string back = System.IO.Directory.GetParent(System.IO.Directory.GetParent(fullPath).ToString()).ToString();
        string path = Path.Combine(back, "docs");
        Debug.LogError(path);


        if(true == m_WithFullTextures)
        {
            name += "_FullTextures";
        }
        else
        {
            name += "_NoTextures";
        }

        string runCommand = $"{path}/ffmpeg -y -r 24 -i {outputTempLocation}/{name}/Image_%04d.png {OutputVideoDirectory}/{name}.avi";
        runCommand = runCommand.Replace("/", "\\");
        runCommand = runCommand.Replace("/", @"\");
        Debug.LogError(runCommand);
        Utils.Code.RunCommand(runCommand);
    }

    private void CreateVideoSettings(string sName)
    {
        string directory = $"{OutputTempFolder}/{sName}";
        Utils.IO.ForceFolder(directory);

        Debug.Log($"Recording file: directory/{sName}");
        videoRecorder.OutputFile = $"{directory}/Image_{DefaultWildcard.Frame}";
    }
#endif
}