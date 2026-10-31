
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using UnityEngine;
using VLB;
using UnityEditor.Rendering.LookDev;


#if UNITY_EDITOR
using UnityEditor;
#endif
using Stopwatch = System.Diagnostics.Stopwatch;
public class ThumbnailMaker : MonoBehaviour
{
    private Camera m_Camera;
    private Transform m_ObjectRoot;
    private Transform m_CameraRig;
    private Transform m_Counter;

    private RenderTexture m_RenderTexture;
    private Texture2D m_ScreenshotTexture;

    private ContentStagingMetaData m_ContentStagingMetaData;

    public const int TEXTURESIZE = 256;

    private const int PICTURES_IN_360 = 6;
    private const float START_ROTATION = 225f;

    private const string THUMBNAIL_FOLDER =
        "Artwork/CatalogueThumbnails";

    private const string SINGLE_BUNDLE =
        "catalogue/cataloguethumbnail.unity3d";

    private const string GROUP_BUNDLE =
        "catalogue/cataloguethumbnailgroup.unity3d";

    [Header("Generation")]
    public bool m_OnlyGenerateMissingThumbnails = false;

    [Header("Performance")]
    [Min(1)]
    public int m_EntriesPerFrame = 10;

    [Min(1)]
    public int m_ImportBatchSize = 250;

    [Min(1)]
    public int m_LogEveryEntries = 25;

    private readonly List<ThumbnailImport> m_PendingImports =
        new List<ThumbnailImport>();

    private System.Diagnostics.Stopwatch m_Stopwatch = new System.Diagnostics.Stopwatch();

    private int m_ProcessedCount;
    private int m_TotalCount;
    private int m_CreatedCount;
    private int m_SkippedCount;
    private int m_FailedCount;

    private double m_GenerationSecondsPerEntry;
    private double m_ImportSecondsPerTexture;

    private class ThumbnailImport
    {
        public string assetPath;
        public string bundleName;

        public ThumbnailImport(string path, string bundle)
        {
            assetPath = path;
            bundleName = bundle;
        }
    }

    private void Awake()
    {
        UnityEngine.XR.XRSettings.enabled = false;

        m_ObjectRoot = transform;
        m_Camera = Camera.main;

        m_CameraRig = transform.Search("CameraRig");
        m_Counter = transform.Search("Counter");

        if (m_Camera == null || m_CameraRig == null)
        {
            UnityEngine.Debug.LogError(
                "ThumbnailMaker: Camera or CameraRig missing."
            );

            enabled = false;
            return;
        }

        m_RenderTexture = new RenderTexture(
            TEXTURESIZE,
            TEXTURESIZE,
            24,
            RenderTextureFormat.ARGB32
        );

        m_RenderTexture.name = "ThumbnailRenderTexture";
        m_RenderTexture.Create();

        m_ScreenshotTexture = new Texture2D(
            TEXTURESIZE,
            TEXTURESIZE,
            TextureFormat.RGBA32,
            false
        );

        m_Camera.forceIntoRenderTexture = true;
        m_Camera.targetTexture = m_RenderTexture;
        m_Camera.aspect = 1f;

        ProcessAllThumbnails();
    }

    private void OnDestroy()
    {
        if (m_Camera != null)
            m_Camera.targetTexture = null;

        if (m_RenderTexture != null)
        {
            m_RenderTexture.Release();
            Destroy(m_RenderTexture);
        }

        if (m_ScreenshotTexture != null)
            Destroy(m_ScreenshotTexture);

        UnityEngine.XR.XRSettings.enabled = true;

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();
#endif
    }

    private void ProcessAllThumbnails()
    {
        UnityEngine.Debug.Log("Processing Catalogue");

        StartCoroutine(Process(() =>
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#endif
        }));
    }

    private void ResetProgress(int total)
    {
        m_TotalCount = total;
        m_ProcessedCount = 0;
        m_CreatedCount = 0;
        m_SkippedCount = 0;
        m_FailedCount = 0;

        m_GenerationSecondsPerEntry = 0;
        m_ImportSecondsPerTexture = 0;

        m_Stopwatch.Restart();
    }

    private string FormatTime(double seconds)
    {
        if (double.IsNaN(seconds) ||
            double.IsInfinity(seconds) ||
            seconds < 0)
        {
            return "Calculating...";
        }

        TimeSpan time = TimeSpan.FromSeconds(seconds);

        return $"{(int)time.TotalHours:D2}:" +
               $"{time.Minutes:D2}:" +
               $"{time.Seconds:D2}";
    }

    private void LogGenerationProgress()
    {
        if (m_ProcessedCount <= 0 || m_TotalCount <= 0)
            return;

        double elapsedSeconds =
            m_Stopwatch.Elapsed.TotalSeconds;

        double averageSeconds =
            elapsedSeconds / m_ProcessedCount;

        m_GenerationSecondsPerEntry = averageSeconds;

        int remainingEntries =
            m_TotalCount - m_ProcessedCount;

        double remainingSeconds =
            remainingEntries * averageSeconds;

        float percentage =
            (float)m_ProcessedCount / m_TotalCount * 100f;

        DateTime estimatedFinish =
            DateTime.Now.AddSeconds(remainingSeconds);

        UnityEngine.Debug.Log(
            $"[THUMBNAILS - GENERATING] " +
            $"{m_ProcessedCount}/{m_TotalCount} " +
            $"({percentage:F1}%) | " +
            $"Created: {m_CreatedCount} | " +
            $"Skipped: {m_SkippedCount} | " +
            $"Failed: {m_FailedCount} | " +
            $"Elapsed: {elapsedSeconds / 60.0:F1} min | " +
            $"Remaining: {remainingSeconds / 60.0:F1} min | " +
            $"ETA: {FormatTime(remainingSeconds)} | " +
            $"Finish: {estimatedFinish:HH:mm:ss}"
        );
    }

    private void LogImportProgress(
        int imported,
        int total,
        double importElapsedSeconds)
    {
        if (imported <= 0 || total <= 0)
            return;

        double averageSeconds =
            importElapsedSeconds / imported;

        m_ImportSecondsPerTexture = averageSeconds;

        int remainingTextures = total - imported;

        double remainingSeconds =
            remainingTextures * averageSeconds;

        float percentage =
            (float)imported / total * 100f;

        DateTime estimatedFinish =
            DateTime.Now.AddSeconds(remainingSeconds);

        UnityEngine.Debug.Log(
            $"[THUMBNAILS - IMPORTING] " +
            $"{imported}/{total} " +
            $"({percentage:F1}%) | " +
            $"Elapsed Total: {m_Stopwatch.Elapsed.TotalMinutes:F1} min | " +
            $"Remaining: {remainingSeconds / 60.0:F1} min | " +
            $"ETA: {FormatTime(remainingSeconds)} | " +
            $"Finish: {estimatedFinish:HH:mm:ss}"
        );
    }

    private IEnumerator Process(Action callback)
    {
        if (Catalogue.Instance == null)
        {
            UnityEngine.Debug.LogError(
                "Catalogue.Instance is null"
            );
            yield break;
        }

        var entries = Catalogue.Instance.Entries;

        int total = entries.Count;
        int processedThisFrame = 0;

        ResetProgress(total);

        string outputFolder =
            Path.Combine(
                Application.dataPath,
                THUMBNAIL_FOLDER
            );

        Directory.CreateDirectory(outputFolder);

        m_PendingImports.Clear();

        UnityEngine.Debug.Log(
            $"Starting thumbnail generation: {total} entries."
        );

        for (int i = 0; i < total; i++)
        {
            CatalogueEntry entry = entries[i];

            if (entry == null)
            {
                m_FailedCount++;
            }
            else
            {
                if (m_Counter != null)
                {
                    m_Counter.name =
                        $"Counter Item: {i + 1} out of {total}";
                }

                string directoryPath =
                    Path.Combine(
                        outputFolder,
                        entry.Guid
                    );

                if (m_OnlyGenerateMissingThumbnails &&
                    HasAllThumbnails(directoryPath, entry.Guid))
                {
                    m_SkippedCount++;
                }
                else
                {
                    RenderMetaData renderMetaData =
                        entry.GetMetaData<RenderMetaData>();

                    m_ContentStagingMetaData =
                        entry.GetMetaData<ContentStagingMetaData>();

                    if (renderMetaData == null ||
                        renderMetaData.Main == null)
                    {
                        m_FailedCount++;
                    }
                    else
                    {
                        GameObject prefab =
                            LoadPrefab(renderMetaData.Main);

                        if (prefab == null)
                        {
                            UnityEngine.Debug.LogError(
                                $"Prefab missing: " +
                                $"{renderMetaData.Main.Bundle}"
                            );

                            m_FailedCount++;
                        }
                        else
                        {
                            GameObject instance = null;

                            try
                            {
                                instance = Instantiate(prefab);

                                var lights =
                                    instance.GetComponentsInChildren<
                                        VolumetricLightBeam>(true);

                                foreach (var light in lights)
                                {
                                    light.color =
                                        new Color(0, 0, 0, 0);
                                }

                                instance.transform.SetParent(m_ObjectRoot);
                                instance.transform.Reset();

                                if (entry.FlipThumbnail)
                                {
                                    instance.transform.rotation =
                                        Quaternion.Euler(180f, 0f, 0f);
                                }

                                ApplyCameraDistance(instance);

                                Physics.SyncTransforms();

                                CreateScreenShot(
                                    entry,
                                    directoryPath
                                );

                                m_CreatedCount++;
                            }
                            catch (Exception ex)
                            {
                                UnityEngine.Debug.LogError(
                                    $"Failed thumbnail {entry.Guid}: {ex}"
                                );

                                m_FailedCount++;
                            }
                            finally
                            {
                                if (instance != null)
                                {
#if UNITY_EDITOR
                                    DestroyImmediate(instance);
#else
                                    Destroy(instance);
#endif
                                }
                            }
                        }
                    }
                }
            }

            // Count every entry, including skipped and failed ones.
            m_ProcessedCount = i + 1;

            if (m_ProcessedCount % m_LogEveryEntries == 0 ||
                m_ProcessedCount == total)
            {
                LogGenerationProgress();
            }

#if UNITY_EDITOR
            if (m_ProcessedCount % 25 == 0 ||
                m_ProcessedCount == total)
            {
                EditorUtility.DisplayProgressBar(
                    "Creating Thumbnails",
                    $"{m_ProcessedCount} out of {total} | " +
                    $"Elapsed: {m_Stopwatch.Elapsed.TotalMinutes:F1} min",
                    (float)m_ProcessedCount / total
                );
            }
#endif

            processedThisFrame++;

            if (processedThisFrame >= m_EntriesPerFrame)
            {
                processedThisFrame = 0;
                yield return null;
            }
        }

        UnityEngine.Debug.Log(
            $"Thumbnail rendering complete in " +
            $"{m_Stopwatch.Elapsed.TotalMinutes:F1} minutes. " +
            $"Starting import of {m_PendingImports.Count} textures."
        );

#if UNITY_EDITOR
        yield return StartCoroutine(ImportAllThumbnails());
#endif

        m_Stopwatch.Stop();

        UnityEngine.Debug.Log(
            $"[THUMBNAILS COMPLETE] " +
            $"Created: {m_CreatedCount} | " +
            $"Skipped: {m_SkippedCount} | " +
            $"Failed: {m_FailedCount} | " +
            $"Total: {total} | " +
            $"Total Time: {m_Stopwatch.Elapsed.TotalMinutes:F2} minutes"
        );

        if (m_Counter != null)
        {
            m_Counter.name =
                $"Finished: {m_CreatedCount} created, " +
                $"{m_SkippedCount} skipped";
        }

#if UNITY_EDITOR
        EditorUtility.ClearProgressBar();

        EditorUtility.DisplayDialog(
            "Completed",
            $"Created: {m_CreatedCount}\n" +
            $"Skipped: {m_SkippedCount}\n" +
            $"Failed: {m_FailedCount}\n" +
            $"Total: {total}\n\n" +
            $"Time: {m_Stopwatch.Elapsed.TotalMinutes:F1} minutes",
            "OK"
        );
#endif

        callback?.Invoke();
    }

    private bool HasAllThumbnails(
        string directoryPath,
        string guid)
    {
        if (!Directory.Exists(directoryPath))
            return false;

        string mainFile =
            Path.Combine(directoryPath, $"{guid}.png");

        if (!File.Exists(mainFile))
            return false;

        for (int i = 0; i < PICTURES_IN_360; i++)
        {
            string file =
                Path.Combine(
                    directoryPath,
                    $"{guid}_{i:00}.png"
                );

            if (!File.Exists(file))
                return false;
        }

        return true;
    }

    private void CreateScreenShot(
        CatalogueEntry entry,
        string directoryPath)
    {
        Directory.CreateDirectory(directoryPath);

        float anglePerSegment =
            360f / PICTURES_IN_360;

        for (int i = 0; i < PICTURES_IN_360; i++)
        {
            Vector3 rotation =
                m_CameraRig.rotation.eulerAngles;

            rotation.y =
                START_ROTATION + anglePerSegment * i;

            m_CameraRig.rotation =
                Quaternion.Euler(rotation);

            byte[] png = CaptureScreenshot();

            string rotationFile =
                $"{entry.Guid}_{i:00}.png";
            UnityEngine.Debug.Log(
                $"[THUMBNAILS] Saving {rotationFile} " +
                $"for {entry.Guid} to {directoryPath}"
            );
            File.WriteAllBytes(
                Path.Combine(directoryPath, rotationFile),
                png
            );

            AddPendingImport(
                entry.Guid,
                rotationFile,
                GROUP_BUNDLE
            );

            if (i == 0)
            {
                string mainFile = $"{entry.Guid}.png";

                File.WriteAllBytes(
                    Path.Combine(directoryPath, mainFile),
                    png
                );

                AddPendingImport(
                    entry.Guid,
                    mainFile,
                    SINGLE_BUNDLE
                );
            }
        }
    }

    private byte[] CaptureScreenshot()
    {
        RenderTexture previous =
            RenderTexture.active;

        try
        {
            m_Camera.Render();

            RenderTexture.active = m_RenderTexture;

            m_ScreenshotTexture.ReadPixels(
                new Rect(
                    0,
                    0,
                    TEXTURESIZE,
                    TEXTURESIZE
                ),
                0,
                0,
                false
            );

            m_ScreenshotTexture.Apply(false);

            return m_ScreenshotTexture.EncodeToPNG();
        }
        finally
        {
            RenderTexture.active = previous;
        }
    }

    private void AddPendingImport(
        string guid,
        string fileName,
        string bundleName)
    {
        string assetPath =
            $"Assets/{THUMBNAIL_FOLDER}/{guid}/{fileName}";

        m_PendingImports.Add(
            new ThumbnailImport(
                assetPath.Replace("\\", "/"),
                bundleName
            )
        );
    }

#if UNITY_EDITOR

    private IEnumerator ImportAllThumbnails()
    {
        if (m_PendingImports.Count == 0)
            yield break;

        int total = m_PendingImports.Count;
        int batchSize = Mathf.Max(1, m_ImportBatchSize);

        UnityEngine.Debug.Log(
            $"[IMPORT] Refreshing AssetDatabase for {total} textures..."
        );

        System.Diagnostics.Stopwatch importStopwatch =
            System.Diagnostics.Stopwatch.StartNew();

        AssetDatabase.Refresh();

        UnityEngine.Debug.Log(
            $"[IMPORT] AssetDatabase.Refresh completed in " +
            $"{importStopwatch.Elapsed.TotalSeconds:F1} seconds."
        );

        for (int start = 0; start < total; start += batchSize)
        {
            int end = Mathf.Min(
                start + batchSize,
                total
            );

            AssetDatabase.StartAssetEditing();

            try
            {
                for (int i = start; i < end; i++)
                {
                    ThumbnailImport item =
                        m_PendingImports[i];

                    ApplyTextureImportSettings(
                        item.assetPath,
                        item.bundleName
                    );
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            LogImportProgress(
                end,
                total,
                importStopwatch.Elapsed.TotalSeconds
            );

            EditorUtility.DisplayProgressBar(
                "Importing Thumbnails",
                $"{end}/{total} | " +
                $"Elapsed: {importStopwatch.Elapsed.TotalMinutes:F1} min",
                (float)end / total
            );

            yield return null;
        }

        m_PendingImports.Clear();

        EditorUtility.ClearProgressBar();

        UnityEngine.Debug.Log(
            $"[IMPORT COMPLETE] " +
            $"Time: {importStopwatch.Elapsed.TotalMinutes:F2} minutes"
        );
    }

    private void ApplyTextureImportSettings(
        string assetPath,
        string bundleName)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(assetPath)
            as TextureImporter;

        if (importer == null)
        {
            UnityEngine.Debug.LogWarning(
                $"Texture importer not found: {assetPath}"
            );

            return;
        }

        bool changed = false;

        if (importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType =
                TextureImporterType.Sprite;

            changed = true;
        }

        if (!importer.alphaIsTransparency)
        {
            importer.alphaIsTransparency = true;
            changed = true;
        }

        if (importer.wrapMode != TextureWrapMode.Clamp)
        {
            importer.wrapMode = TextureWrapMode.Clamp;
            changed = true;
        }

        if (importer.assetBundleName != bundleName)
        {
            importer.SetAssetBundleNameAndVariant(
                bundleName,
                ""
            );

            changed = true;
        }

        if (changed)
        {
            importer.SaveAndReimport();
        }
    }

#endif

    private void ApplyCameraDistance(GameObject focus)
    {
        Bounds bounds = GetEncapsulatingBounds(focus);

        focus.transform.position = Vector3.zero;

        Vector3 xyz = bounds.size;

        float radius = xyz.magnitude / 2f;

        m_Camera.fieldOfView = 36f;

        float horizontalFOV =
            2f * Mathf.Atan(
                Mathf.Tan(
                    m_Camera.fieldOfView *
                    Mathf.Deg2Rad / 2f
                ) * m_Camera.aspect
            ) * Mathf.Rad2Deg;

        float fov = Mathf.Min(
            m_Camera.fieldOfView,
            horizontalFOV
        );

        float dist =
            radius /
            Mathf.Sin(fov * Mathf.Deg2Rad / 2f);

        m_Camera.transform.localPosition =
            new Vector3(
                0f,
                bounds.center.y,
                -dist
            );

        focus.transform.localPosition = Vector3.zero;

        if (m_ContentStagingMetaData != null)
        {
            focus.transform.localPosition =
                -bounds.center;
        }
    }

    public Bounds GetEncapsulatingBounds(GameObject obj)
    {
        Bounds totalBounds = new Bounds();

        bool hasBounds = false;

        foreach (var renderer in
                 obj.GetComponentsInChildren<Renderer>())
        {
            string path =
                renderer.gameObject.GetGameObjectPath();

            if (path.CaseInsensitiveContains("Hologram/"))
                continue;

            if (!hasBounds)
            {
                totalBounds = renderer.bounds;
                hasBounds = true;
            }
            else
            {
                totalBounds.Encapsulate(renderer.bounds);
            }
        }

        return totalBounds;
    }

    public static GameObject LoadPrefab(
        RenderItemData renderItem)
    {
        if (renderItem == null)
            return null;

        if (string.IsNullOrEmpty(renderItem.Guid))
            return null;

#if UNITY_EDITOR
        string path =
            AssetDatabase.GUIDToAssetPath(renderItem.Guid);

        if (!string.IsNullOrEmpty(path))
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(
                path
            );
        }
#endif

        return null;
    }

    [InspectorButton]
    private void CheckAllBundles()
    {
        ProcessAllCreatedThumbnails();
    }

    private void ProcessAllCreatedThumbnails()
    {
#if UNITY_EDITOR
        StartCoroutine(CheckAllCreated());
#endif
    }

    private IEnumerator CheckAllCreated()
    {
        var entries = Catalogue.Instance.Entries;

        m_PendingImports.Clear();

        foreach (var entry in entries)
        {
            if (entry == null)
                continue;

            string folder =
                Path.Combine(
                    Application.dataPath,
                    THUMBNAIL_FOLDER,
                    entry.Guid
                );

            string mainFile = $"{entry.Guid}.png";

            if (File.Exists(Path.Combine(folder, mainFile)))
            {
                AddPendingImport(
                    entry.Guid,
                    mainFile,
                    SINGLE_BUNDLE
                );
            }

            for (int i = 0; i < PICTURES_IN_360; i++)
            {
                string file =
                    $"{entry.Guid}_{i:00}.png";

                if (File.Exists(Path.Combine(folder, file)))
                {
                    AddPendingImport(
                        entry.Guid,
                        file,
                        GROUP_BUNDLE
                    );
                }
            }
        }

#if UNITY_EDITOR
        yield return StartCoroutine(ImportAllThumbnails());
#else
        yield break;
#endif
    }

    [InspectorButton]
    private void RemoveDeletedItems()
    {
        RemoveUnwantedItems();
    }

    private void RemoveUnwantedItems()
    {
        ProcessRemoveDeletedItems(
            Path.Combine(
                Application.dataPath,
                "Artwork/CatalogueThumbnails"
            )
        );

        ProcessRemoveDeletedItems(
            Path.Combine(
                Application.dataPath,
                "StreamingAssets/BundleStreamingAssets/CatalogueData/entries"
            )
        );

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    private void ProcessRemoveDeletedItems(
        string foldersToRemove)
    {
        if (!Directory.Exists(foldersToRemove))
            return;

        var validGuids = new HashSet<string>(
            Catalogue.Instance.Entries
                .Where(e => e != null)
                .Select(e => e.Guid),
            StringComparer.OrdinalIgnoreCase
        );

        string[] directories =
            Directory.GetDirectories(
                foldersToRemove,
                "*",
                SearchOption.TopDirectoryOnly
            );

        foreach (string directory in directories)
        {
            string guid = Path.GetFileName(directory);

            if (!validGuids.Contains(guid))
            {
                Directory.Delete(directory, true);

                string metaFile = directory + ".meta";

                if (File.Exists(metaFile))
                    File.Delete(metaFile);
            }
        }
    }
}
