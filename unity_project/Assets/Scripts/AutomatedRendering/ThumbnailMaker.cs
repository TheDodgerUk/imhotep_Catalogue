using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VLB;

public class ThumbnailMaker : MonoBehaviour
{
    private Camera m_Camera;
    private Transform m_ObjectRoot;

    private Vector3 m_CameraAngle = new Vector3(45f, 225f, 0f);
    private Transform m_CameraRig;
    private RenderTexture m_RenderTexture;

    public const int TEXTURESIZE = 256;

    public bool m_OnlyGenerateMissingThumbnails = false; // Feel free to change this from the inspector before playing the scene
    private Material m_Material;

    private Transform m_Counter;

    private const int PICTURES_IN_360 = 6;
    private const float START_ROTATION = 225f;

    private ContentStagingMetaData m_ContentStagingMetaData;

    // Start is called before the first frame update
    private void Awake()
    {
        UnityEngine.XR.XRSettings.enabled = false;
        m_Material = Resources.Load<Material>("PureWhite");

        m_ObjectRoot = transform;
        m_Camera = Camera.main;

        m_CameraRig = transform.Search("CameraRig");

        m_Counter = transform.Search("Counter");

        m_RenderTexture = new RenderTexture(TEXTURESIZE, TEXTURESIZE, 0);
        {
            name = gameObject.name;
        }
        m_Camera.forceIntoRenderTexture = true;
        m_Camera.targetTexture = m_RenderTexture;

        // m_Camera.transform.rotation = Quaternion.Euler(m_CameraAngle);
        ProcessAllThumbnails();
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
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }));
    }

    // [InspectorButton]
    private void CheckAllBundles()
    {
        ProcessAllCreatedThumbnails();
    }

    private void ProcessAllCreatedThumbnails()
    {
#if UNITY_EDITOR
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(CheckAllCreated());
#endif
    }

    private IEnumerator CheckAllCreated()
    {
        string bundleName = "catalogue/cataloguethumbnail.unity3d";
        string bundleGroupName = "catalogue/cataloguethumbnailgroup.unity3d";
        string sOutputFolder = $"{Application.dataPath}/";
        string sProjectFolder = "Artwork/CatalogueThumbnails";
        int count = 0;
        foreach (var entry in Catalogue.Instance.Entries)
        {
            for (int i = 0; i < PICTURES_IN_360; i++)
            {
                ApplyTextureImportSettings($"Assets/{sProjectFolder}/{entry.Guid}/{entry.Guid}{i.ToString("00")}.png", bundleGroupName);
            }
            yield return null;
            count++;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar($"Creating Thumbnails", $"At {count} out of {Catalogue.Instance.Entries.Count}", (float)count / (float)Catalogue.Instance.Entries.Count);
#endif
            Debug.Log($"At {count} out of {Catalogue.Instance.Entries.Count} ");
        }
    }

    [InspectorButton]
    private void RemoveDeletedItems()
    {
        RemoveUnwantedItems();
    }

    private void RemoveUnwantedItems()
    {
        //D:\Work\Imhotep_ModelCatalogueAzure\unity_project\Assets\Artwork\CatalogueThumbnails
        //D:\Work\Imhotep_ModelCatalogueAzure\unity_project\Assets\StreamingAssets\BundleStreamingAssets\CatalogueData\entries
        ProcessRemoveDeletedItems($"{Application.dataPath}/Artwork/CatalogueThumbnails");
        ProcessRemoveDeletedItems($"{Application.dataPath}/StreamingAssets/BundleStreamingAssets/CatalogueData/entries");
    }

    private void ProcessRemoveDeletedItems(string foldersToRemove)
    {
        var dirs = Directory.GetDirectories(foldersToRemove, "*.*", SearchOption.TopDirectoryOnly).ToList();
        for (int i = 0; i < dirs.Count; i++)
        {
            dirs[i] = dirs[i].SanitiseSlashes();
        }
        List<string> cat = new List<string>();
        foreach (var item in Catalogue.Instance.Entries)
        {
            cat.Add($"{foldersToRemove}/{item.Guid}".SanitiseSlashes());
        }

        var dif = dirs.Except(cat).ToList();

        Debug.LogError($"dir count {dirs.Count}");
        Debug.LogError($"cat count {cat.Count}");
        Debug.LogError($"dif count {dif.Count}");
        foreach (var item in dif)
        {
            Directory.Delete(item, true);
        }
    }

    private IEnumerator Process(Action callback)
    {
        int count = 0;
        string sOutputFolder = $"{Application.dataPath}/";
        string sProjectFolder = "Artwork/CatalogueThumbnails";
        for (int i = 0; i < Catalogue.Instance.Entries.Count; i++)
        {
            Debug.Log($"{i} out of {Catalogue.Instance.Entries.Count}");
#if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar($"Creating thumbnails", $"{i} out of {Catalogue.Instance.Entries.Count}", (float)i / (float)Catalogue.Instance.Entries.Count);
#endif

            if (null != m_Counter)
            {
                m_Counter.name = $"Counter Item: {i + 1} out of {Catalogue.Instance.Entries.Count}";
            }
            CatalogueEntry entry = Catalogue.Instance.Entries[i];

            if (true == m_OnlyGenerateMissingThumbnails)
            {
                string directoryPath = $"{sOutputFolder}{sProjectFolder}/{entry.Guid}";
                if (true == Utils.IO.FolderExists(directoryPath) && Directory.GetFiles(directoryPath).Length > 2)
                {
                    continue;
                }
            }
            count++;
            if (null != entry)
            {
                var renderMetaData = entry.GetMetaData<RenderMetaData>();

                m_ContentStagingMetaData = entry.GetMetaData<ContentStagingMetaData>();

                if (null != renderMetaData && null != renderMetaData.Main)
                {
                    var prefab = LoadPrefab(renderMetaData.Main);
                    if (prefab != null)
                    {
                        var instance = GameObject.Instantiate(prefab) as GameObject;
                        if (null != instance)
                        {
                            var lights = instance.GetComponentsInChildren<VolumetricLightBeam>().ToList();
                            lights.ForEach(e => e.color = new Color(0, 0, 0, 0)); // this is so there is no lights in thumbnails 

                            instance.transform.SetParent(m_ObjectRoot);
                            instance.transform.Reset();

                            if (entry.FlipThumbnail == true)
                            {
                                // flip upside down 
                                instance.transform.rotation = Quaternion.Euler(180, 0, 0);
                            }

                            //Get size of object!
                            ApplyCameraDistance(instance);

                            yield return new WaitForEndOfFrame();

                            yield return CreateScreenShot(entry, instance);

                            yield return new WaitForEndOfFrame();


                            GameObject.Destroy(instance);

                            yield return new WaitForEndOfFrame();
                        }
                    }
                    else
                    {
                        Debug.LogError($" prefab was null, {renderMetaData.Main.Bundle}");
                    }
                }
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
        UnityEditor.EditorUtility.DisplayDialog("Completed", $" Created:{count} Total  {Catalogue.Instance.Entries.Count}", "OK");
#endif
        if (null != m_Counter)
        {
            m_Counter.name = $"ALL finished of: Created {count},  {Catalogue.Instance.Entries.Count}";
        }

        yield return null;
    }

    private IEnumerator CreateScreenShot(CatalogueEntry entry, GameObject root)
    {
        string bundleName = "catalogue/cataloguethumbnail.unity3d";
        string bundleGroupName = "catalogue/cataloguethumbnailgroup.unity3d";
        string sOutputFolder = $"{Application.dataPath}/";
        string sProjectFolder = "Artwork/CatalogueThumbnails";
        string directoryPath = $"{sOutputFolder}{sProjectFolder}/{entry.Guid}";
        Utils.IO.ForceFolder(directoryPath);


        float anglePerSegment = (float)(360f / PICTURES_IN_360);
        for (int i = 0; i < PICTURES_IN_360; i++)
        {
            Vector3 rotation = m_CameraRig.rotation.eulerAngles;
            rotation.y = START_ROTATION + (anglePerSegment * i);
            m_CameraRig.rotation = Quaternion.Euler(rotation);

            if (i == 0)
            {
                //First screenshot only, take square screenshot aswell
                RenderTexture tex = new RenderTexture(TEXTURESIZE, TEXTURESIZE, 0);

                m_Camera.forceIntoRenderTexture = true;
                m_Camera.targetTexture = tex;

                yield return new WaitForEndOfFrame();

                TakeAndSaveScreenShot(tex, $"{directoryPath}/", $"{entry.Guid}.png");

                yield return new WaitForEndOfFrame();

                ApplyTextureImportSettings($"Assets/{sProjectFolder}/{entry.Guid}/{entry.Guid}.png", bundleName);

                m_Camera.targetTexture = m_RenderTexture;
            }

            yield return new WaitForEndOfFrame();

            TakeAndSaveScreenShot(m_RenderTexture, $"{directoryPath}/", $"{entry.Guid}_{i.ToString("00")}.png");

            yield return new WaitForEndOfFrame();

            ApplyTextureImportSettings($"Assets/{sProjectFolder}/{entry.Guid}/{entry.Guid}_{i.ToString("00")}.png", bundleGroupName);
        }
        Debug.Log($"Image: {entry.Guid} saved");
    }

    private void ApplyTextureImportSettings(string assetPath, string bundleName)
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.AssetImporter import = UnityEditor.AssetImporter.GetAtPath(assetPath);
        if (null != import)
        {
            UnityEditor.TextureImporter textureSettings = (UnityEditor.TextureImporter)import;
            textureSettings.textureType = TextureImporterType.Sprite;
            textureSettings.alphaIsTransparency = true;
            textureSettings.wrapMode = TextureWrapMode.Clamp;
            textureSettings.SaveAndReimport();
            import.SetAssetBundleNameAndVariant(bundleName, "");
            import.SaveAndReimport();
        }
#endif
    }

    private void TakeAndSaveScreenShot(RenderTexture renderTexture, string filePath, string fileName)
    {
        //Copy render Texture to texture
        RenderTexture.active = renderTexture;

        m_Camera.Render();
        int textureWidth = renderTexture.width;
        int textureHeight = renderTexture.height;

        Texture2D myTexture2D = new Texture2D(textureWidth, textureHeight);
        myTexture2D.ReadPixels(new Rect(0, 0, textureWidth, textureHeight), 0, 0);
        myTexture2D.Apply();

        byte[] bytes = myTexture2D.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{filePath}{fileName}", bytes);
    }

    private void SaveNewThumbnail(Texture2D texture, string filePath, string fileName)
    {
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes($"{filePath}{fileName}", bytes);
    }

    private void MaskAlpha(Texture2D A, Texture2D mask)
    {
        Color[] color = A.GetPixels();
        Color[] maskColor = mask.GetPixels();

        for (int i = 0; i < color.Length; i++)
        {
            color[i].a = maskColor[i].r;
        }

        A.SetPixels(color);
        A.Apply();
    }

    private Renderer[] GetRenderers(GameObject root)
    {
        Transform model = root.transform.Search(GlobalConsts.RootModel);
        if (model != null)
        {
            return model.GetComponentsInChildren<Renderer>();
        }
        else
        {
            Debug.LogError($"Cannot find Model in {root.name}");
            return null;
        }
    }

    private void ApplyCameraDistance(GameObject focus)
    {
        Bounds bounds = GetEncapsulatingBounds(focus);
        focus.transform.position = Vector3.zero;

        Vector3 xyz = bounds.size;
        float radius = xyz.magnitude / 2f;
        m_Camera.fieldOfView = 36;

        // Get the horizontal FOV, since it may be the limiting of the two FOVs to properly encapsulate the objects
        float horizontalFOV = 2f * Mathf.Atan(Mathf.Tan(m_Camera.fieldOfView * Mathf.Deg2Rad / 2f) * m_Camera.aspect) * Mathf.Rad2Deg;
        // Use the smaller FOV as it limits what would get cut off by the frustum
        float fov = Mathf.Min(m_Camera.fieldOfView, horizontalFOV);
        float dist = (radius) / (Mathf.Sin(fov * Mathf.Deg2Rad / 2f));
        m_Camera.transform.localPosition = new Vector3(0f, bounds.center.y, -dist);
        Vector3 centreDir = focus.transform.position - bounds.center;
        centreDir.y = 0f;
        focus.transform.localPosition = Vector3.zero;
        m_Camera.fieldOfView = 36;


        //staging is done in an odd way 
        if (m_ContentStagingMetaData != null)
        {
            focus.transform.localPosition = -bounds.center;
        }
    }


    public Bounds GetEncapsulatingBounds(GameObject obj)
    {
        Bounds totalBounds = new Bounds();
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            var path = renderer.gameObject.GetGameObjectPath();
            if (path.CaseInsensitiveContains("Hologram/") == false)
            {
                if (totalBounds.size.magnitude == 0f)
                {
                    totalBounds = renderer.bounds;
                }
                else
                {
                    totalBounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return totalBounds;
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

}