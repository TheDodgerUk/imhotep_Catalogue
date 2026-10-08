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

public class RecordMultiple : MonoBehaviour
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

    private int iCount = 0;
    private int iToProcessCount = 0;
    [SerializeField]
    private GameObject m_CurrentViewedObject;
    private Camera m_Camera;

    private string OutputTempFolder = $@"C:\VideosImages\Catalogue";
    private string OutputVideoDirectory = $@"C:\Videos\Catalogue";

    private TextMeshProUGUI m_ProductNameText;
    private PostProcessVolume m_PostVolume;

    //private Stopwatch m_Stopwatch = new Stopwatch();

    private void Start()
    {
        //WaitForCommandProcess(@"C:\Videos\Catalogue\Behringer_Deepmind12", "fred");
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

        // m_Stopwatch.Reset();
        SetupVideoRecorder(ProcessAllCatalogueVideos);
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


        m_Director = GetComponent<PlayableDirector>();
        m_Director.stopped += OnFinishedAnimation;

        Debug.Log("Setup recorder");

        callback?.Invoke();
    }

    private void ProcessAllCatalogueVideos()
    {
        Debug.Log("Catalogue loaded and ready");
        iToProcessCount = Catalogue.Instance.Entries.Count;
        //ObjectsToRecord = transform.GetComponentsInChildren<Transform>();
        UpdateRecorderAndRecord();
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
        return model.GetComponentsInChildren<Renderer>();
    }

    private void ShowOnlyCount(CatalogueEntry entry, string name)
    {
        GameObject.Destroy(m_CurrentViewedObject);

        var renderMetaData = entry.GetMetaData<RenderMetaData>();

        if (null != renderMetaData && null != renderMetaData.Main)
        {
            var prefab = LoadPrefab(renderMetaData.Main);
            var instance = GameObject.Instantiate(prefab) as GameObject;
            if (null != instance)
            {
                instance.name = name;
                m_CurrentViewedObject = instance;

                instance.transform.SetParent(TurnTable);
                instance.transform.Reset();
            }
        }
    }

    private void UpdateCatalogueInfo(CatalogueEntry entry)
    {
        m_ProductNameText.text = entry.ShortName.Text;
    }

    public static GameObject LoadPrefab(RenderItemData renderItem)
    {
        if (string.IsNullOrEmpty(renderItem.Guid) == false)
        {
#if UNITY_EDITOR
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(renderItem.Guid);
            if (string.IsNullOrEmpty(path) == false)
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
#endif
        }
        return null;
    }

    private void UpdateRecorderAndRecord()
    {
        if (iToProcessCount > iCount)
        {
            CatalogueEntry entry = Catalogue.Instance.Entries[iCount];
            if (null != entry)
            {
                entry.OnPostLoad();
                string productName = entry.FullName.Text.Replace(" ", "_");
                if (false == Utils.IO.FileExists($"{OutputVideoDirectory}/{productName}.avi") &&
                    true == entry.HasVideo)
                {
                    ShowOnlyCount(entry, productName);
                    Debug.Log("Loaded catalogue entry: " + productName);

                    CreateVideoSettings(productName);
                    UpdateCatalogueInfo(entry);
                    ApplyCameraDistance(m_CurrentViewedObject);

                    Debug.Log($"Starting recording of {iCount}/{iToProcessCount}");
                    m_RecorderController.StartRecording();
                    m_Director.Play();
                }
                else
                {
                    iCount++;
                    UpdateRecorderAndRecord();
                }
            }
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"Finished all recordings in time {Time.timeSinceLevelLoad}");
            UnityEditor.EditorApplication.isPlaying = false;
            //UnityEditor.EditorApplication.Exit(0);
#endif
        }
    }

    void OnFinishedAnimation(PlayableDirector director)
    {
        Debug.Log($"Finished Recording: {iCount}/{iToProcessCount}");
        m_RecorderController.StopRecording();

        m_RecorderController.Settings.Save();

        WaitForCommandProcess(OutputTempFolder, m_CurrentViewedObject.name);

        StartCoroutine(WaitForRecordingComplete(() =>
        {
            // Utils.IO.DestroyFolder($"{OutputDirectory}/{m_CurrentViewedObject.name}");
            iCount++;
            Debug.Log("Finished playing animation");
            UpdateRecorderAndRecord();
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