using EditorTools.CatalogueViewer;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using static GlobalConsts;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;

namespace EditorTools.Bundles
{
    public class BundleBuilderWindow : EditorWindow, ISerializationCallbackReceiver
    {
        [MenuItem("Window/Custom/Asset Bundle Builder", false, 2)]
        public static void ShowWindow() => GetWindow<BundleBuilderWindow>("Bundle Builder");

        private static BuildTarget m_Platform = BuildTarget.StandaloneWindows64;



        private BrowseFolder m_StagingBundlesFolder;
        private BrowseFolder m_MainProjectFolder;


        private bool m_Serialised = false;
        private bool m_ShowLabels = false;

        private Vector2 m_ScrollPos;

        private bool AwakeRun = false;

        private GlobalConsts.AssetBundleVersion m_ThumbnailVersion;
        private GlobalConsts.AssetBundleVersion m_ThumbnailGroupVersion;
        private const int LARGE_BUTTON_WIDTH = 450;
        private const int MEDIUM_BUTTON_WIDTH = 200;
        private const int SMALL_BUTTON_WIDTH = 20;

        public static string m_StartCatalogueGuid = "";


        public static bool m_ValidBuildBundles = false;
        public static bool m_ValidCopyToAll = false;
        public static bool m_ValidCopyToVariant = false;
        private static Texture2D m_CompleatedIcon;

        private void Awake()
        {
            if (Catalogue.Instance == null)
            {
                Catalogue.CreateInstance();
                using (var progress = new ProgressBar("Loading Catalogue For All"))
                {
                    Catalogue.Instance.LoadFromFile(str => progress.UpdateIncrement(str), c => progress.SetMaxIncrement(c), null);
                }
            }
            BuildTargetGroup buildTargetGroup = BuildTargetGroup.Standalone;
            m_Platform = BuildTarget.StandaloneWindows64;

#if UNITY_EDITOR_OSX
            m_Platform = BuildTarget.StandaloneOSX;
            buildTargetGroup = BuildTargetGroup.Standalone;
#endif

#if UNITY_ANDROID
            m_Platform = BuildTarget.Android;
            buildTargetGroup = BuildTargetGroup.Android;
#endif

#if UNITY_WEBGL
            m_Platform = BuildTarget.WebGL;
            buildTargetGroup = BuildTargetGroup.WebGL;
#endif


            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;

            m_StagingBundlesFolder = new StagingBundlesFolder(EditorUserBuildSettings.activeBuildTarget);
            m_MainProjectFolder = new MainProjectFolder();
            AwakeRun = true;
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() => m_Serialised = true;

        private void OnGUI()
        {
            if (false == AwakeRun)
            {
                return;
            }

            if (m_Serialised == true)
            {
                Awake();
                m_Serialised = false;
            }

            GUILayout.Label("Asset Bundle Builder", EditorStyles.boldLabel);

            GUILayout.Label($"   Target Platform: {m_Platform} ", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            m_MainProjectFolder.DrawFolderPath();
            EditorGUILayout.Space();

            EditorHelper.Separator();

            using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
            {

                DrawBuiltBundlesControls();


                EditorGUILayout.Space();
                DrawMainProjectControls();

                //   EditorGUILayout.Space();
                //  DrawBundleSetupControls();

                m_ScrollPos = s.scrollPosition;
            }
        }


        private void DrawBuiltBundlesControls()
        {
            if (m_CompleatedIcon == null)
            {
                m_CompleatedIcon = (Texture2D)Resources.Load("Valid", typeof(Texture2D));
            }

            m_StagingBundlesFolder.DrawControls(AssetBundleBuilder.IsAnyPlatformBuilt(m_Platform), null, button =>
            {
                //dont use this handler because it doesnt go new line
            });
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Clear loaded Catalogue", GUILayout.Width(200)) == true)
                {
                    using (var progress = new ProgressBar("Loading Catalogue For All"))
                    {
                        Catalogue.Instance.LoadFromFile(str => progress.UpdateIncrement(str), c => progress.SetMaxIncrement(c), null);
                    }
                }
                if (GUILayout.Button("Replace with Prefabs", GUILayout.Width(200)) == true)
                {
                    ReplacePrefabs();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Build Bundles", GUILayout.Width(200)) == true)
                {
                    string dataSource = Device.StreamingPath;
                    dataSource = dataSource.SanitiseSlashes();
                    CreateJsonToRePlacePrefabs(dataSource);
                    BuildBundles(false, onlyManifest: false);
                    m_ValidBuildBundles = true;
                    m_ValidCopyToAll = false;
                    m_ValidCopyToVariant = false;
                }
                if (GUILayout.Button("ReBuild Bundles", GUILayout.Width(250)) == true)
                {
                    if (EditorUtility.DisplayDialog("Build Bundle", "This will rebuild all Assets", "Continue", "Cancel") == true)
                    {
                        BuildBundles(true, onlyManifest: false);
                        m_ValidBuildBundles = true;
                        m_ValidCopyToAll = false;
                        m_ValidCopyToVariant = false;
                    }
                }
                if (GUILayout.Button("CreateJsonToRePlacePrefabs TEST", GUILayout.Width(250)) == true)
                {
                    string dataSource = Device.StreamingPath;
                    dataSource = dataSource.SanitiseSlashes();
                    CreateJsonToRePlacePrefabs(dataSource);
                }


                if (m_ValidBuildBundles == true)
                {
                    GUILayout.Label(m_CompleatedIcon, GUILayout.Width(20), GUILayout.Height(20));
                }
            }
        }

        private void BuildBundles(bool cleanBuild, bool onlyManifest)
        {
            bool runBundle = true;

            // files missing when SaveCataloguePerEnvironment is called 
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            string rootFolder = $"{GlobalConsts.BUNDLE_STREAMING}/";
            foreach (var item in environmentData.Environments)
            {
                foreach (var comb in item.Variants)
                {
                    string folder = $"{rootFolder}{comb.CombinedName}/{GlobalConsts.DataFolder}";
                    string file = $"{folder}/CatalogueData.json";

                    if (File.Exists(file) == false)
                    {
                        List<string> empty = new List<string>();
                        Json.JsonNet.WriteToFile(empty, $"{folder}/CatalogueData", true);
                    }
                }
            }


#if UNITY_EDITOR
            const string GraphicsSettingsAssetPath = "ProjectSettings/GraphicsSettings.asset";
            SerializedObject graphicsManager = new SerializedObject(UnityEditor.AssetDatabase.LoadAllAssetsAtPath(GraphicsSettingsAssetPath)[0]);
            SerializedProperty lightStripping = graphicsManager.FindProperty("m_LightmapStripping");
            SerializedProperty fogStripping = graphicsManager.FindProperty("m_FogStripping");
            //Debug.LogError($"lightStripping {lightStripping.intValue}");
            //Debug.LogError($"fogStripping {fogStripping.intValue}");
            if (lightStripping.intValue != 0 || fogStripping.intValue != 0)
            {
                runBundle = EditorUtility.DisplayDialog("This will be long", "Striping is not set to Automatic, which means the build will Take ages \n Project settings->Grathics, Lightmap Modes and Fog Modes, set both to Automatic for fast build", "Continue without Change", "Cancel");
            }
#endif
            if (runBundle == true)
            {
                Debug.LogError($" platform is  {m_Platform}");
                if (false == AssetBundleBuilder.BuildBundles(m_Platform, cleanBuild, onlyManifest))
                {
                    Debug.LogError($"Asset bundle build failed for {m_Platform}\n");
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Build Bundle", "Build Bundle has failed! :(", "Ok");
                }
                else
                {
                    Debug.Log($"Asset bundle build completed\n");
                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Build Bundle", "Build Bundle has been completed", "Ok");
                }
            }
        }

        private class FolderTarget
        {
            public readonly string[] Extensions;
            public readonly string SourcePath;

            public FolderTarget(string source, params string[] extensions)
            {
                Extensions = extensions;
                SourcePath = source;
            }
        }

        private void DrawMainProjectControls()
        {
            string bundlePath = AssetBundleBuilder.StagingFolder(GlobalConsts.CurrentPlatformName()).Absolute;

            string bundleFolder = GlobalConsts.BundleFolder + GlobalConsts.CurrentPlatformName();
            var targets = new Dictionary<string, FolderTarget>
            {
                { bundleFolder, new FolderTarget(bundlePath, "json", AssetBundleBuilder.BundleExtension) },
                { GlobalConsts.DataFolder, new FolderTarget(Catalogue.DataFolder, "json") },
            };

            string bundleSource = bundlePath;
            string dataSource = Device.StreamingPath;
            string rootPath = m_MainProjectFolder.GetCurrentPath().Absolute + $"/{GlobalConsts.BundleFolder}{GlobalConsts.CurrentPlatformName()}/";
            string dataTarget = m_MainProjectFolder.GetCurrentPath().Absolute + "/";

            bundlePath = bundlePath.SanitiseSlashes();
            bundleSource = bundleSource.SanitiseSlashes();
            rootPath = rootPath.SanitiseSlashes();
            dataSource = dataSource.SanitiseSlashes();
            dataTarget = dataTarget.SanitiseSlashes();

            GUILayout.Label("Bundle Source: " + bundleSource);
            GUILayout.Label("Root Target: " + rootPath);
            GUILayout.Label("Data Source: " + dataSource);
            GUILayout.Label("Data Target: " + dataTarget);

            string varientName = PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME_SHORT);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Copy to (All)", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
                {

                    Utils.IO.ForceFolder(rootPath);
                    CleanStreamingAssetsFolder(dataTarget);

                    string[] bundleExtensions = new string[] { "*.json", AssetBundleBuilder.BundleSearchExtension };
                    //Utils.Editor.CopyFiles(bundlePath, rootPath, m_CleanCopy, bundleExtensions);
                    CopyFiles(bundlePath, rootPath, bundleExtensions);

                    string[] dataExtensions = new string[] { "*.json", "*.mov", "*.mp4", };
                    CopyFiles(dataSource, dataTarget, dataExtensions);

                    string[] unwantedFolders = new string[] { "metaData" };
                    RemoveFolders(dataTarget, unwantedFolders);

                    RemoveJunkFilesFolders(dataTarget);

                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/BrandData/BrandData.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/CatalogueData/description.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/LanguageData/english.json", null, true);

                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Copy to (All)", "Copy to has been completed", "Ok");
                    m_ValidCopyToAll = true;
                }

                if (m_ValidCopyToAll == true)
                {
                    GUILayout.Label(m_CompleatedIcon, GUILayout.Width(20), GUILayout.Height(20));
                    m_ValidCopyToAll = true;
                    m_ValidCopyToVariant = false;
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button($"Copy to (Variant only)   {varientName}", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
                {
                    Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
                    environmentData.LoadFromFile();
                    var variant = environmentData.GetCurrentVariant();

                    Utils.IO.ForceFolder(rootPath);
                    CleanStreamingAssetsFolder(dataTarget);



                    string[] bundleExtensions = new string[] { "*.json", AssetBundleBuilder.BundleSearchExtension };
                    //Utils.Editor.CopyFiles(bundlePath, rootPath, m_CleanCopy, bundleExtensions);
                    CopyFilesVarientAssetBundles(bundlePath, rootPath, bundleExtensions);

                    string[] dataExtensions = new string[] { "BundleStreamingAssets/BrandData", "BundleStreamingAssets/CatalogueData", "BundleStreamingAssets/EnvironmentData", "BundleStreamingAssets/LanguageData", };



                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/CatalogueData.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/CatalogueData_Version.json", null, true);

                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/AttributeData.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/ModuleData.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/CatalogueReplaceInScene.json", null, true);


                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/BrandData/BrandData.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/CatalogueData/description.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/LanguageData/english.json", null, true);
                    CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/EnvironmentData/{variant.Environment.Guid}/{variant.Guid}.json", null, true);



                    dataExtensions = new string[] { "*.json", "*.mov", "*.mp4", "index.json" };
                    CopyFilesVarient(dataSource, dataTarget, dataExtensions);

                    CopyFilesVarientIndexNoMenu(dataSource, dataTarget, variant); // this disables the menu system so it goes straight to the correct program

                    CopyVariantCatalogueExtraBundleFiles(dataSource, bundlePath, rootPath, variant);
                    CopyVariantCatalogueJsonFiles(dataSource, dataSource, dataTarget, "*.json", variant);
                    CopyVariantCatalogueBundleFiles(bundleSource, dataSource, bundlePath, rootPath, AssetBundleBuilder.BundleSearchExtension, variant);

                    List<string> copiedFiles = new List<string>();
                    string masterFile = $"{bundleSource}/master";
                    var data = Json.JsonNet.ReadFromFile<AssetBundleDependencyMap>(masterFile);
                    string key = $"environments/{variant.Guid}.unity3d";
                    if (data.ContainsKey(key) == true)
                    {
                        var itemsArray = data[key];
                        foreach (var item in itemsArray)
                        {

                            var newPath = $"{bundleSource}/{item.BundleName}";
                            CopyFile(bundleSource, rootPath, newPath, copiedFiles);


                            // this will find all the items missing
                            string rawBundlekey = $"{item.BundleName}";
                            if (data.ContainsKey(rawBundlekey) == true)
                            {
                                var rawItemArray = data[rawBundlekey];
                                foreach (var rawItem in rawItemArray)
                                {
                                    var rawNewPath = $"{bundleSource}/{rawItem.BundleName}";
                                    CopyFile(bundleSource, rootPath, rawNewPath, copiedFiles);
                                }
                            }
                        }
                    }


                    CheckVarientReplaceFiles(dataSource, dataTarget, variant, copiedFiles);

                    string[] unwantedFolders = new string[] { "metaData" };
                    RemoveFolders(dataTarget, unwantedFolders);

                    RemoveUnwantedVariantFolders($"{dataTarget}bundles/{GlobalConsts.CurrentPlatformName()}/environment");
                    RemoveUnwantedVariantFolders($"{dataTarget}bundles/{GlobalConsts.CurrentPlatformName()}/environments");
                    RemoveJunkFilesFolders(dataTarget);


                    if (variant.IncludeCataloguePrefabs == false)
                    {
                        RemoveUnwantedFile($"{dataTarget}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/CatalogueData.json");
                        RemoveUnwantedFile($"{dataTarget}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/CatalogueData_Version.json");
                        RemoveUnwantedFile($"{dataSource}/BundleStreamingAssets/{variant.CombinedName}/Data/CatalogueReplaceInScene.json");
                        RemoveUnwantedFile($"{dataTarget}bundles/{GlobalConsts.CurrentPlatformName()}/master.json");
                        RemoveUnwantedFolders($"{dataTarget}BundleStreamingAssets/CatalogueData/entries");

                    }
                    else
                    {
                        CopyFile(dataSource, dataTarget, $"{dataSource}/BundleStreamingAssets/CatalogueData/entries/Combined.json", null, true);
                    }

                    if (variant.IncludeGenericAssetBundles == false)
                    {
                        RemoveUnwantedFolders($"{dataTarget}bundles/{GlobalConsts.CurrentPlatformName()}/environment/{variant.OverloadCombinedName}");
                    }


                    EditorApplication.Beep();
                    EditorUtility.DisplayDialog("Copy to (Variant only)", $"Copy to has been completed: {variant.Environment.Name}/{variant.Name.ToString()}", "Ok");

                    m_ValidCopyToVariant = true;
                }

                if (m_ValidCopyToVariant == true)
                {
                    GUILayout.Label(m_CompleatedIcon, GUILayout.Width(20), GUILayout.Height(20));
                    m_ValidCopyToVariant = true;
                    m_ValidCopyToAll = false;
                }
            }



            var boldtext = new GUIStyle(GUI.skin.label);
            boldtext.fontStyle = FontStyle.BoldAndItalic;
            boldtext.fontSize = 15;
            boldtext.alignment = TextAnchor.MiddleCenter;


            EditorHelper.Separator();

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Asset Server, ALL", boldtext, GUILayout.ExpandWidth(true));
            GUILayout.Space(10);


            var platformNameString = GlobalConsts.CurrentPlatformName();
            string thumbnailVersion = $"{dataSource}/BundleStreamingAssets/{platformNameString}/Catalogue/cataloguethumbnail_Version";
            string thumbnailGroupVersion = $"{dataSource}/BundleStreamingAssets/{platformNameString}/Catalogue/cataloguethumbnailGroup_Version";
            string thumbnailVersionBundle = $"{dataSource}/Bundle/{platformNameString}/Catalogue/cataloguethumbnail_Version";
            string thumbnailGroupVersionBundle = $"{dataSource}/Bundle/{platformNameString}/Catalogue/cataloguethumbnailGroup_Version";
            if (m_ThumbnailVersion == null)
            {
                if (File.Exists($"{thumbnailVersion}.json") == true)
                {
                    m_ThumbnailVersion = Json.JsonNet.ReadFromFile<GlobalConsts.AssetBundleVersion>(thumbnailVersion);
                }
                else
                {
                    m_ThumbnailVersion = new AssetBundleVersion();
                }
            }
            if (m_ThumbnailGroupVersion == null)
            {
                if (File.Exists($"{thumbnailGroupVersion}.json") == true)
                {
                    m_ThumbnailGroupVersion = Json.JsonNet.ReadFromFile<GlobalConsts.AssetBundleVersion>(thumbnailGroupVersion);
                }
                else
                {
                    m_ThumbnailGroupVersion = new AssetBundleVersion();
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                m_ThumbnailVersion.Version = EditorGUILayout.IntField("CatalogueThumbnailVersion", m_ThumbnailVersion.Version, GUILayout.MaxWidth(200));
                m_ThumbnailGroupVersion.Version = EditorGUILayout.IntField("CataloguethumbnailgroupVersion", m_ThumbnailGroupVersion.Version, GUILayout.MaxWidth(200));
            }
            if (GUILayout.Button("Save Version Control", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
            {
                SaveThumbnailVersions(dataSource, platformNameString);
            }

            GUILayout.Space(10);

            ServerDepot(dataSource);
        }

        private static void CheckVarientReplaceFiles(string dataSource, string dataTarget, Environments.EnvironmentVariantData variant, List<string> copiedFiles)
        {
            var replaceData = Json.JsonNet.ReadFromFile<List<CatalogueReplaceInScene>>($"{dataSource}/BundleStreamingAssets/{variant.OverloadCombinedName}/Data/CatalogueReplaceInScene.json");
            foreach (var item in replaceData)
            {
                var replace = Catalogue.Instance.Entries.Find(e => e.Guid == item.CatalogueGUID);
                if (replace == null)
                {
                    Debug.LogError($"Need to add to environmentData  {item.CatalogueGUID}");
                }
            }
        }

        private void SaveThumbnailVersions(string dataSource, string platformNameString)
        {
            string thumbnailVersion = $"{dataSource}/BundleStreamingAssets/{platformNameString}/Catalogue/cataloguethumbnail_Version";
            string thumbnailGroupVersion = $"{dataSource}/BundleStreamingAssets/{platformNameString}/Catalogue/cataloguethumbnailGroup_Version";
            string thumbnailVersionBundle = $"{dataSource}/Bundles/{platformNameString}/Catalogue/cataloguethumbnail_Version";
            string thumbnailGroupVersionBundle = $"{dataSource}/Bundles/{platformNameString}/Catalogue/cataloguethumbnailGroup_Version";

            m_ThumbnailVersion.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnail.unity3d").Length);
            m_ThumbnailGroupVersion.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnailgroup.unity3d").Length);
            Json.JsonNet.WriteToFile(m_ThumbnailVersion, thumbnailVersion, true);
            Json.JsonNet.WriteToFile(m_ThumbnailGroupVersion, thumbnailGroupVersion, true);

            Json.JsonNet.WriteToFile(m_ThumbnailVersion, thumbnailVersionBundle, true);
            Json.JsonNet.WriteToFile(m_ThumbnailGroupVersion, thumbnailGroupVersionBundle, true);
        }

        private void CreateJsonToRePlacePrefabs(string dataSource)
        {

            InternalCreateJsonToRePlacePrefabs(dataSource);
            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();

        }

        private void InternalCreateJsonToRePlacePrefabs(string dataSource)
        {

            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            string currentGUID = environmentData.GetCurrentVariant().Guid;
            string combinedName = environmentData.GetCurrentVariant().OverloadCombinedName;
            const string CATALOGUE_PATH = "Assets/Artwork/Models/Catalogue/";

            List<CatalogueReplaceInScene> otherData = new List<CatalogueReplaceInScene>();

            var allItemTags = GameObject.FindGameObjectsWithTag(GlobalConsts.RootCatalogueItemTag);
            for (int j = 0; j < allItemTags.Length; j++)
            {
                Selection.activeObject = allItemTags[j].gameObject;
                EditorGUIUtility.PingObject(Selection.activeObject);

                var realPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(allItemTags[j].gameObject);
                if (realPath.StartsWith(CATALOGUE_PATH) == true)
                {
                    string realGUID = AssetDatabase.AssetPathToGUID(realPath);
                    CatalogueReplaceInScene newData = new CatalogueReplaceInScene();

                    newData.GameObjectRef = allItemTags[j].gameObject;
                    newData.HierarchyPath = allItemTags[j].gameObject.GetGameObjectPath();
                    newData.HierarchyPathRoot = newData.HierarchyPath.Split('/')[0];
                    newData.Filepath = realPath;
                    newData.RealGUID = realGUID;
                    newData.Position = allItemTags[j].gameObject.transform.position;
                    newData.Rotation = allItemTags[j].gameObject.transform.rotation;
                    newData.Scene = allItemTags[j].gameObject.scene.name;
                    CheckStatic(newData, allItemTags[j].gameObject);
                    if (newData.Scene.StartsWith(currentGUID) == false)
                    {
                        newData.Scene = $"{currentGUID}_{newData.Scene}"; // makes sure its full ie   "7d4b9bf2-9588-4996-a03a-7207b3cb7147_MT_MonitorWhitebox_Android"
                    }
                    otherData.Add(newData);
                }
                else
                {
                    Debug.LogError($"Prefab Not prefab, please look:: {allItemTags[j].gameObject}", allItemTags[j].gameObject);
                    EditorApplication.Beep();
                }
            }

            List<string> fullPaths = new List<string>();
            for (int i = 0; i < otherData.Count; i++)
            {
                if (fullPaths.Contains(otherData[i].HierarchyPath) == false)
                {
                    fullPaths.Add(otherData[i].HierarchyPath);
                }
                else
                {
                    Debug.LogError($"Duplicate paths will cause errors when tring to find item on main side : {otherData[i].HierarchyPath}");
                }
            }

            for (int i = 0; i < otherData.Count; i++)
            {
                for (int j = 0; j < Catalogue.Instance.Entries.Count; j++)
                {
                    var renderMetaData = Catalogue.Instance.Entries[j].GetMetaData<RenderMetaData>();

                    if (otherData[i].RealGUID == renderMetaData.Main.Guid)
                    {
                        otherData[i].CatalogueGUID = Catalogue.Instance.Entries[j].Guid;

                        if (Catalogue.Instance.Entries[j].EnvironmentsAvailable.Contains(combinedName) == false)
                        {
                            Debug.LogError($"Item not been added to this Environments : {Catalogue.Instance.Entries[j].FullName}");
                        }
                        break;
                    }
                }
            }

            for (int i = 0; i < otherData.Count; i++)
            {
                if (string.IsNullOrEmpty(otherData[i].CatalogueGUID) == true)
                {
                    Debug.LogError($"Cannot find CatalogueGUID for :{otherData[i].Filepath}");
                }
            }


            List<string> guid = new List<string>();
            int count = 0;
            // enable GPU instancing 
            foreach (var item in otherData)
            {
                if (guid.Contains(item.RealGUID) == false)
                {
                    guid.Add(item.RealGUID);
                    var allItems = otherData.FindAll(e => e.RealGUID == item.RealGUID);
                    if (allItems.Count >= 1)
                    {
                        bool enableInstancing = allItems.Count >= 1;
                        var renderers = allItems[0].GameObjectRef.GetComponentsInChildren<Renderer>(true);

                        foreach (var render in renderers)
                        {
                            foreach (var mat in render.materials)
                            {
                                if (mat.enableInstancing == !enableInstancing)
                                {
                                    mat.enableInstancing = enableInstancing;
                                    count += (allItems.Count * render.materials.Length);
                                }
                            }
                        }
                    }

                    EditorUtility.SetDirty(allItems[0].GameObjectRef);

                }
            }
            AssetDatabase.SaveAssets();
            Debug.LogError($"Mats changed {count}");

            // not overload , as the scenes are differnet 
            var variant = environmentData.GetCurrentVariant();
            string JsonToReplace = $"{dataSource}/BundleStreamingAssets/{variant.CombinedName}/Data/CatalogueReplaceInScene";
            Json.JsonNet.WriteToFile(otherData, JsonToReplace, true);

            JsonToReplace = $"{dataSource}/BundleStreamingAssets/{variant.CombinedName}/Data/VFXReplaceInScene";
            Json.JsonNet.WriteToFile(Environments.EnvironmentSceneGenerator.m_VFXReplaceInScene, JsonToReplace, true);
            Debug.LogError(JsonToReplace);
        }


        private void CheckStatic(CatalogueReplaceInScene data, GameObject root)
        {
            // no point looking at these
            for (int j = 0; j < Catalogue.Instance.Entries.Count; j++)
            {
                var renderMetaData = Catalogue.Instance.Entries[j].GetMetaData<RenderMetaData>();

                if (data.RealGUID == renderMetaData.Main.Guid)
                {
                    var allItems = root.GetComponentsInChildren<Rigidbody>();
                    foreach (var item in allItems)
                    {
                        item.gameObject.SetStaticRecursively(false);
                    }

                    var entry = Catalogue.Instance.Entries[j];
                    var allMetaData = Catalogue.Instance.Entries[j].GetMetaDataTypes();
                    foreach (var metaData in allMetaData)
                    {
                        switch (metaData)
                        {
                            case MetaDataType.ContentButton:
                                Debug.Log("static check ContentButton", root);
                                var button = entry.GetMetaData<ContentButtonMetaData>();
                                button.CollectAllData(root);
                                foreach (var item in button.m_ItemData)
                                {
                                    item.m_ModelGameObject.SetStaticRecursively(false);
                                }
                                break;
                            case MetaDataType.ContentSlider:
                                Debug.Log("static check ContentSlider", root);
                                var slider = entry.GetMetaData<ContentSliderMetaData>();
                                slider.CollectAllData(root);
                                foreach (var item in slider.m_ItemData)
                                {
                                    item.m_ModelGameObject.SetStaticRecursively(false);
                                }
                                break;
                            case MetaDataType.ContentHinge:
                                Debug.Log("static check ContentHinge", root);
                                var hinge = entry.GetMetaData<ContentHingeMetaData>();
                                hinge.CollectAllData(root);
                                foreach (var item in hinge.m_HingeData)
                                {
                                    item.m_HingeGameObject.SetStaticRecursively(false);
                                }
                                break;
                            case MetaDataType.ContentPickUp:
                                Debug.Log("static check ContentPickUp", root);
                                var pickup = entry.GetMetaData<ContentPickUpMetaData>();
                                pickup.CollectAllData(root);
                                root.SetStaticRecursively(false);
                                break;
                            case MetaDataType.ContentFood:
                                Debug.Log("static check ContentPickUp", root);
                                var pickupFood = entry.GetMetaData<ContentFoodMetaData>();
                                pickupFood.CollectAllData(root);
                                root.SetStaticRecursively(false);
                                break;
                            case MetaDataType.ContentDrink:
                                Debug.Log("static check ContentPickUp", root);
                                var pickupDrink = entry.GetMetaData<ContentDrinkMetaData>();
                                pickupDrink.CollectAllData(root);
                                root.SetStaticRecursively(false);
                                break;

                            case MetaDataType.ContentPickUpCable:
                                var cable = entry.GetMetaData<ContentPickUpCableMetaData>();
                                cable.CollectAllData(root);
                                foreach (var item in cable.m_Data)
                                {
                                    item.m_GameObjectPickup.SetStaticRecursively(false);
                                }
                                break;
                            case MetaDataType.ContentPickUpSocket:
                                var socket = entry.GetMetaData<ContentPickUpSocketMetaData>();
                                socket.CollectAllData(root);
                                foreach (var item in socket.m_Data)
                                {
                                    item.m_GameObjectPickup.SetStaticRecursively(false);
                                }
                                break;
                        }
                    }


                }
            }
        }

        private void ReplacePrefabs()
        {
            string dataSource = Device.StreamingPath;
            dataSource = dataSource.SanitiseSlashes();
            InternalReplacePrefabs(dataSource);
        }

        private void InternalReplacePrefabs(string dataSource)
        {
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            string currentGUID = environmentData.GetCurrentVariant().Guid;
            const string CATALOGUE_PATH = "Assets/Artwork/Models/Catalogue/";

            string _PREFAB_PREFAB = $"{GlobalConsts._PREFAB_PREFAB_EXTENTION}";
            int REMOVE_AMOUNT = ".prefab".Length;

            var all = Selection.gameObjects.ToList();
            for (int i = 0; i < all.Count; i++)
            {
                Selection.activeObject = all[i].gameObject;
                EditorGUIUtility.PingObject(Selection.activeObject);


                // checks to see if ends the item is a prefab  but NOT the generated one from the catalogue side
                // if it is, it replaces it with the generated catalougue item
                var realPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(all[i].gameObject);
                if (realPath.StartsWith(CATALOGUE_PATH) == true)
                {
                    if (realPath.EndsWith(_PREFAB_PREFAB) == false)
                    {
                        realPath = realPath.Remove(realPath.Length - REMOVE_AMOUNT, REMOVE_AMOUNT);
                        realPath += _PREFAB_PREFAB;

                        string realGUID = AssetDatabase.AssetPathToGUID(realPath);

                        bool found = false;
                        for (int j = 0; j < Catalogue.Instance.Entries.Count; j++)
                        {
                            var renderMetaData = Catalogue.Instance.Entries[j].GetMetaData<RenderMetaData>();

                            if (realGUID == renderMetaData.Main.Guid)
                            {
                                found = true;
                                int index = all[i].gameObject.transform.GetSiblingIndex();
                                var assignedPrefab = PrefabManager.LoadPrefab(renderMetaData.Main);

                                SceneManager.SetActiveScene(all[i].gameObject.scene);

                                var clone = PrefabUtility.InstantiatePrefab(assignedPrefab) as GameObject;
                                clone.name = $"{clone.name} ({Mathf.Abs(clone.GetInstanceID())})";
                                clone.transform.parent = all[i].gameObject.transform.parent;
                                clone.transform.localPosition = all[i].gameObject.transform.localPosition;
                                clone.transform.localRotation = all[i].gameObject.transform.localRotation;

                                DestroyImmediate(all[i].gameObject);
                                clone.transform.SetSiblingIndex(index);
                                break;
                            }
                        }
                        if (found == false && realPath.EndsWith(".fbx") == true)
                        {
                            Debug.LogError("FBX");
                        }


                    }
                }

                // checks to see if ends with GUID ie
                // Chair 1 6ded8684-7836-4dda-bb87-42e218e14bcf
                // replaces it with the correct catalogue item prefab
                for (int j = 0; j < Catalogue.Instance.Entries.Count; j++)
                {
                    var itemTransform = all[i];
                    var renderMetaData = Catalogue.Instance.Entries[j].GetMetaData<RenderMetaData>();

                    if (itemTransform.name.Contains(Catalogue.Instance.Entries[j].Guid) == true || itemTransform.name.Contains(renderMetaData.Main.Guid) == true)
                    {
                        int index = itemTransform.gameObject.transform.GetSiblingIndex();
                        var assignedPrefab = PrefabManager.LoadPrefab(renderMetaData.Main);

                        SceneManager.SetActiveScene(itemTransform.gameObject.scene);

                        var clone = PrefabUtility.InstantiatePrefab(assignedPrefab) as GameObject;

                        var cloneName = itemTransform.name;
                        clone.name = $"{cloneName}";

                        clone.transform.parent = itemTransform.gameObject.transform.parent;
                        clone.transform.localPosition = itemTransform.gameObject.transform.localPosition;
                        clone.transform.localRotation = itemTransform.gameObject.transform.localRotation;

                        DestroyImmediate(itemTransform.gameObject);
                        clone.transform.SetSiblingIndex(index);
                        break;
                    }
                }

            }
            EditorSceneManager.MarkAllScenesDirty();
            EditorSceneManager.SaveOpenScenes();

        }


        private void CleanStreamingAssetsFolder(string rootPath)
        {
            List<string> directories = Directory.GetDirectories(rootPath, "*.*", SearchOption.TopDirectoryOnly).ToList();
            foreach (string directory in directories)
            {
                if (directory.EndsWith("Bundle") || directory.EndsWith("bundles") || directory.EndsWith("BundleStreamingAssets"))
                {
                    Directory.Delete(directory, true);
                }
            }
        }

        private void ServerDepot(string dataSource)
        {
            var platformNameString = GlobalConsts.CurrentPlatformName();
            var shortDataSource = m_MainProjectFolder.GetCurrentPath().Absolute + $"/{GlobalConsts.BundleFolder}";
            string serverDepot = GlobalConsts.SERVER_DEPOT_ASSETS_EXPLORER;
            serverDepot = serverDepot.Replace(@"/", @"\");   // explorer doesn't like front slashes

            EditorGUILayout.Space();

            if (GUILayout.Button("Copy Catalogue Json files Index and Master to Server", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
            {
#if UNITY_EDITOR
                UploadAllJson(dataSource, platformNameString, null);
#endif
            }


            if (GUILayout.Button("Copy Catalogue Json files to Server (each catalogue json file)", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
            {
#if UNITY_EDITOR
                List<string> files = new List<string>();
                files.Add(dataSource + $"/bundles/{platformNameString}/Master.json");
                files.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/index.json");
                files.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/Combined.json");

                string catalogueData = dataSource + "/BundleStreamingAssets/CatalogueData";
                CopyToServer server = new CopyToServer();


                server.UploadJsonFiles(files, () =>
                {
                    server.UploadJsonFileDirectory(catalogueData, null);
                });
#endif
            }

            if (GUILayout.Button("Copy Catalogue Thumbnails Asset Bundles files to Server", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
            {
#if UNITY_EDITOR
                SaveThumbnailVersions(dataSource, platformNameString);
                CopyToServer server = new CopyToServer();
                List<string> fileNamesJson = new List<string>()
                    {
                        {dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnail_Version.json" },
                        {dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnailgroup_Version.json" },
                    };
                server.UploadJsonFiles(fileNamesJson, () =>
                {
                    List<string> fileNames = new List<string>()
                    {
                        {dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnail.unity3d" },
                        {dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnailgroup.unity3d" },
                    };
                    server.UploadAssetBundles(fileNames, null);
                });
#endif
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Copy Catalogue Asset Bundles files to Server", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
                {
#if UNITY_EDITOR
                    List<string> files = new List<string>();
                    files.Add(dataSource + $"/bundles/{platformNameString}/Master.json");
                    files.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/index.json");
                    files.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/Combined.json");

                    string catalogueData = dataSource + "/BundleStreamingAssets/CatalogueData";
                    CopyToServer server = new CopyToServer();
                    server.UploadAssetBundles(files, () =>
                    {
                        server.UploadJsonFileDirectory(catalogueData, () =>
                        {
                            string prefabs = dataSource + $"/bundles/{platformNameString}/Catalogue/Prefabs";
                            server.UploadAssetDirectory(prefabs, null);
                        });

                    });
                }

                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    float totalSize = 0f;
                    foreach (var item in Catalogue.Instance.Entries)
                    {
                        var data = item.GetMetaData<RenderMetaData>();
                        totalSize += data.SizeOfFileMB[GlobalConsts.GetPlatformIndex(m_Platform)];
                    }

                    EditorUtility.DisplayDialog("Catalogue Info", $"Count: {Catalogue.Instance.Entries.Count},    size: {totalSize.ToString("n2")} MB", "ok");
                }
#endif
            }

            if (GUILayout.Button("Copy Everything to Server (Good internet)", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
            {

                List<string> thumbnailAssets = new List<string>()
                {
                    {dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnail.unity3d" },
                    {dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnailgroup.unity3d" },
                };

                string bundleStreamingAssets = dataSource + "/BundleStreamingAssets";
                string bundles = dataSource + "/Bundle";
                string prefabs = dataSource + $"/bundles/{platformNameString}/Catalogue/Prefabs";

                SaveThumbnailVersions(dataSource, platformNameString);




                CopyToServer server = new CopyToServer();


                server.UploadAssetBundles(thumbnailAssets, () =>
                {
                    server.UploadJsonFileDirectory(bundles, () =>
                    {
                        server.UploadJsonFileDirectory(bundleStreamingAssets, () =>
                        {
                            server.UploadAssetDirectory(prefabs, null);
                        });

                    });

                });


            }



            EditorHelper.Separator();

            var boldtext = new GUIStyle(GUI.skin.label);
            boldtext.fontStyle = FontStyle.BoldAndItalic;
            boldtext.fontSize = 15;
            boldtext.alignment = TextAnchor.MiddleCenter;
            string varientName = PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME_SHORT);
            string varientOverloadedName = PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_OVERLOADED_COMBINED_NAME_SHORT);

            if (varientName == varientOverloadedName)
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField($"Current Variant ({varientName}), file to Asset Server", boldtext, GUILayout.ExpandWidth(true));
                GUILayout.Space(10);
            }
            else
            {
                GUILayout.Space(10);
                EditorGUILayout.LabelField($"Current Variant ({varientName})", boldtext, GUILayout.ExpandWidth(true));
                GUILayout.Space(10);
                EditorGUILayout.LabelField($"Overloaded Environment  ({varientOverloadedName}), file to Asset Server", boldtext, GUILayout.ExpandWidth(true));
                GUILayout.Space(10);
            }


            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("All Variant files to Server", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
                {
                    AllVariantToExternal(dataSource, platformNameString);
                }

                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
                    environmentData.LoadFromFile();
                    var variant = environmentData.GetCurrentVariant();
                    string variantName = variant.OverloadCombinedName;

                    float totalSize = 0f;
                    int count = 0;
                    foreach (var item in Catalogue.Instance.Entries)
                    {
                        if (item.EnvironmentsAvailable.Contains(variantName))
                        {
                            var data = item.GetMetaData<RenderMetaData>();
                            totalSize += data.SizeOfFileMB[GlobalConsts.GetPlatformIndex(m_Platform)];
                            count++;
                        }
                    }

                    EditorUtility.DisplayDialog("Catalogue Info", $"Count: {count},    size: {totalSize.ToString("n2")} MB", "ok");
                }

                m_StartCatalogueGuid = EditorGUILayout.TextField("       StartCatalogueGuid", m_StartCatalogueGuid, GUILayout.MaxWidth(500));

            }

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("All Variant GenericAssetBundles to Server", GUILayout.Width(LARGE_BUTTON_WIDTH)) == true)
                {
                    AllVariantGenericToExternal(dataSource, platformNameString);
                }

                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
                    environmentData.LoadFromFile();
                    var variant = environmentData.GetCurrentVariant();
                    List<string> bundleFiles = new List<string>();
                    string baseDirectory = dataSource + $"/bundles/{platformNameString}/environment/{variant.OverloadCombinedName}/";
                    bundleFiles.Add(baseDirectory + "AudioClipAssetBundle.unity3d");
                    bundleFiles.Add(baseDirectory + "MaterialAssetBundle.unity3d");
                    bundleFiles.Add(baseDirectory + "Texture2DAssetBundle.unity3d");
                    bundleFiles.Add(baseDirectory + "GameObjectAssetBundle.unity3d");
                    bundleFiles.Add(baseDirectory + "VideoClipAssetBundle.unity3d");
                    bundleFiles.Add(baseDirectory + "TextAssetBundle.unity3d");

                    bundleFiles.Add(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnail.unity3d");
                    bundleFiles.Add(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnailgroup.unity3d");

                    string allInfo = "";
                    float totalSize = 0f;
                    foreach (var item in bundleFiles)
                    {
                        if (File.Exists(item) == true)
                        {
                            System.IO.FileInfo info = new System.IO.FileInfo(item);
                            float size = (info.Length / 1024f);
                            size /= 1024;
                            totalSize += size;

                            string name = item;
                            int index = name.LastIndexOf("/");
                            name = name.Substring(index + 1); // +1 removes the /
                            allInfo += $"{name}  Size {size.ToString("n2")} MB \n";
                        }
                        else
                        {
                            Debug.Log($"file {item} does not exist");
                        }
                    }

                    EditorUtility.DisplayDialog("Catalogue Info", $"{allInfo} \n\n Total size {totalSize.ToString("n2")} MB", "ok");
                }

            }
            GUILayout.Space(30);


            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("CatalogueData", GUILayout.Width(MEDIUM_BUTTON_WIDTH)) == true)
                {
                    UploadAllJson(dataSource, platformNameString, null);
                }
                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    EditorUtility.DisplayDialog("NOT DONE Info", $"NOT DONE", "ok");
                }
                GUILayout.Label("", GUILayout.Width(SMALL_BUTTON_WIDTH));
            }
            using (new EditorGUILayout.HorizontalScope())
            {

                if (GUILayout.Button("AudioClipAssetBundle", GUILayout.Width(MEDIUM_BUTTON_WIDTH)) == true)
                {
                    VariantGenericToExternal("AudioClipAssetBundle.unity3d", dataSource, platformNameString);
                }
                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    float size = AssetBundleSize("AudioClipAssetBundle.unity3d", dataSource, platformNameString);
                    EditorUtility.DisplayDialog("AudioClipAssetBundle Info", $"size {size.ToString("n2")} MB", "ok");
                }

                GUILayout.Label("", GUILayout.Width(SMALL_BUTTON_WIDTH));

                if (GUILayout.Button("MaterialAssetBundle", GUILayout.Width(MEDIUM_BUTTON_WIDTH)) == true)
                {
                    VariantGenericToExternal("MaterialAssetBundle.unity3d", dataSource, platformNameString);
                }
                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    float size = AssetBundleSize("MaterialAssetBundle.unity3d", dataSource, platformNameString);
                    EditorUtility.DisplayDialog("MaterialAssetBundle Info", $"size {size.ToString("n2")} MB", "ok");
                }

                GUILayout.Label("", GUILayout.Width(SMALL_BUTTON_WIDTH));
                if (GUILayout.Button("Texture2DAssetBundle", GUILayout.Width(MEDIUM_BUTTON_WIDTH)) == true)
                {
                    VariantGenericToExternal("Texture2DAssetBundle.unity3d", dataSource, platformNameString);
                }
                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    float size = AssetBundleSize("Texture2DAssetBundle.unity3d", dataSource, platformNameString);
                    EditorUtility.DisplayDialog("Texture2DAssetBundle Info", $"size {size.ToString("n2")} MB", "ok");
                }

                GUILayout.Label("", GUILayout.Width(SMALL_BUTTON_WIDTH));
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("GameObjectAssetBundle", GUILayout.Width(MEDIUM_BUTTON_WIDTH)) == true)
                {
                    VariantGenericToExternal("GameObjectAssetBundle.unity3d", dataSource, platformNameString);
                }
                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    float size = AssetBundleSize("GameObjectAssetBundle.unity3d", dataSource, platformNameString);
                    EditorUtility.DisplayDialog("GameObjectAssetBundle Info", $"size {size.ToString("n2")} MB", "ok");
                }
                GUILayout.Label("", GUILayout.Width(SMALL_BUTTON_WIDTH));

                if (GUILayout.Button("VideoClipAssetBundle", GUILayout.Width(MEDIUM_BUTTON_WIDTH)) == true)
                {
                    VariantGenericToExternal("VideoClipAssetBundle.unity3d", dataSource, platformNameString);
                }
                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    float size = AssetBundleSize("VideoClipAssetBundle.unity3d", dataSource, platformNameString);
                    EditorUtility.DisplayDialog("VideoClipAssetBundle Info", $"size {size.ToString("n2")} MB", "ok");
                }
                GUILayout.Label("", GUILayout.Width(SMALL_BUTTON_WIDTH));

                if (GUILayout.Button("TextAssetBundle", GUILayout.Width(MEDIUM_BUTTON_WIDTH)) == true)
                {
                    VariantGenericToExternal("TextAssetBundle.unity3d", dataSource, platformNameString);
                }
                if (GUILayout.Button("?", GUILayout.Width(SMALL_BUTTON_WIDTH)) == true)
                {
                    float size = AssetBundleSize("TextAssetBundle.unity3d", dataSource, platformNameString);
                    EditorUtility.DisplayDialog("TextAssetBundle Info", $"size {size.ToString("n2")} MB", "ok");
                }
                GUILayout.Label("", GUILayout.Width(SMALL_BUTTON_WIDTH));
            }
            GUILayout.Space(10);
        }


        private float AssetBundleSize(string assetBundle, string dataSource, string platformNameString)
        {
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var variant = environmentData.GetCurrentVariant();
            List<string> bundleFiles = new List<string>();
            string baseDirectory = dataSource + $"/bundles/{platformNameString}/environment/{variant.OverloadCombinedName}/";
            bundleFiles.Add(baseDirectory + assetBundle);
            float totalSize = 0f;
            foreach (var item in bundleFiles)
            {
                if (File.Exists(item) == true)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(item);
                    totalSize += info.Length;
                }
            }
            // needs to be done twice 
            totalSize /= 1024f;
            totalSize /= 1024f;
            return totalSize;
        }

        private void UploadAllJson(string dataSource, string platformNameString, System.Action callback)
        {
            CopyToServer server = new CopyToServer();
            List<string> jsonFiles = new List<string>();
            jsonFiles.Add(dataSource + $"/bundles/{platformNameString}/Master.json");
            jsonFiles.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/index.json");
            jsonFiles.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/Combined.json");

            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            foreach (var enviro in environmentData.Environments)
            {
                foreach (var variant in enviro.Variants)
                {
                    string att = dataSource + $"/BundleStreamingAssets/{variant.CombinedName}/Data/AttributeData.json";
                    if (File.Exists(att))
                    {
                        jsonFiles.Add(att);
                    }

                    string cat = dataSource + $"/BundleStreamingAssets/{variant.CombinedName}/Data/CatalogueData.json";
                    if (File.Exists(cat))
                    {
                        jsonFiles.Add(cat);
                    }

                    string mod = dataSource + $"/BundleStreamingAssets/{variant.CombinedName}/Data/ModuleData.json";
                    if (File.Exists(mod))
                    {
                        jsonFiles.Add(mod);
                    }
                    string catToReplace = dataSource + $"/BundleStreamingAssets/{variant.CombinedName}/Data/CatalogueReplaceInScene.json";
                    if (File.Exists(catToReplace))
                    {
                        jsonFiles.Add(catToReplace);
                    }

                    string baseDirectory = dataSource + $"/bundles/{platformNameString}/environment/{variant.CombinedName}/";
                    GenericBundleJsonFiles(dataSource, variant, baseDirectory, ref jsonFiles);
                }
            }
            server.UploadJsonFiles(jsonFiles, callback);
        }

        private void AllVariantGenericToExternal(string dataSource, string platformNameString)
        {
            SaveThumbnailVersions(dataSource, platformNameString);

            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var variant = environmentData.GetCurrentVariant();

            CopyToServer server = new CopyToServer();
            List<string> bundleFiles = new List<string>();
            string baseDirectory = dataSource + $"/bundles/{platformNameString}/environment/{variant.OverloadCombinedName}/";
            bundleFiles.Add(baseDirectory + "AudioClipAssetBundle.unity3d");
            bundleFiles.Add(baseDirectory + "MaterialAssetBundle.unity3d");
            bundleFiles.Add(baseDirectory + "Texture2DAssetBundle.unity3d");
            bundleFiles.Add(baseDirectory + "GameObjectAssetBundle.unity3d");
            bundleFiles.Add(baseDirectory + "VideoClipAssetBundle.unity3d");
            bundleFiles.Add(baseDirectory + "TextAssetBundle.unity3d");

            bundleFiles.Add(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnail.unity3d");
            bundleFiles.Add(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnailgroup.unity3d");

            List<string> jsonFiles = new List<string>();
            GenericBundleJsonFiles(dataSource, variant, baseDirectory, ref jsonFiles);

            jsonFiles.Add(dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnail_Version.json");
            jsonFiles.Add(dataSource + $"/bundles/{platformNameString}/catalogue/cataloguethumbnailgroup_Version.json");
            server.UploadJsonFiles(jsonFiles, () =>
            {
                server.UploadAssetBundles(bundleFiles, null);
            });
        }

        private void VariantGenericToExternal(string filename, string dataSource, string platformNameString)
        {
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var variant = environmentData.GetCurrentVariant();
            CopyToServer server = new CopyToServer();
            List<string> bundleFiles = new List<string>();
            string baseDirectory = dataSource + $"/bundles/{platformNameString}/environment/{variant.OverloadCombinedName}/";
            bundleFiles.Add(baseDirectory + filename);
            server.UploadAssetBundles(bundleFiles, null);
        }

        private void AllVariantToExternal(string dataSource, string platformNameString)
        {
            SaveThumbnailVersions(dataSource, platformNameString);
#if UNITY_EDITOR


            bool found = false;

            var foundEntry = Catalogue.Instance.Entries.Find(e => e.Guid == m_StartCatalogueGuid);
            if (string.IsNullOrEmpty(m_StartCatalogueGuid) == false)
            {
                if (foundEntry != null)
                {
                    int display = 0;

                    if (string.IsNullOrEmpty(foundEntry.ShortName.Text) == true)
                    {
                        display = EditorUtility.DisplayDialogComplex("AllVariantToExternal", $"upload All, found guid", "All", "Cancel", $"From Guid");
                    }
                    else
                    {
                        display = EditorUtility.DisplayDialogComplex("AllVariantToExternal", $"upload All, Guid is {foundEntry.ShortName}", "All", "Cancel", $"{foundEntry.ShortName}");
                    }

                    if (display == 0) // ok ALL
                    {
                        found = true;
                    }
                    else if (display == 1) // Cancel
                    {
                        return;
                    }
                    else // Guid stuff
                    {
                        found = false;
                    }
                }
                else
                {
                    found = true;
                    if (EditorUtility.DisplayDialog("AllVariantToExternal", $"upload all, Guid is invalid", "All", $"Cancel") == false)
                    {
                        return;
                    }
                }
            }

            var depPath = Device.StreamingPath + $"/bundles/{platformNameString}/master";
            var dependencies = Json.JsonNet.ReadFromFile<Dictionary<string, List<BundleLoader.DependencyData>>>(depPath);


            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var variant = environmentData.GetCurrentVariant();
            string variantName = variant.OverloadCombinedName;
            string bundleSource = dataSource + $"/bundles/{platformNameString}/";

            string catalogueData = dataSource + "/BundleStreamingAssets/CatalogueData/entries/";


            Dictionary<string, string> individualItems = new Dictionary<string, string>();
            Dictionary<string, string> individualDependencies = new Dictionary<string, string>();

            foreach (var item in Catalogue.Instance.Entries)
            {
                if (found == false)
                {
                    if (item == foundEntry)
                    {
                        found = true;
                    }
                }

                if (found == true)
                {
                    if (item.EnvironmentsAvailable.Contains(variantName))
                    {

                        var render = item.GetMetaData<RenderMetaData>();
                        string bundleKey = bundleSource + render.Main.Bundle;
                        string jsonValue = catalogueData + item.Guid + "/" + item.Guid;

                        if (individualItems.ContainsKey(bundleKey) == false)
                        {
                            individualItems.Add(bundleKey, jsonValue);
                        }
                    }
                }
            }

            foreach (var dependent in dependencies)
            {
                foreach (var catDataItem in individualItems)
                {
                    if (catDataItem.Key.EndsWith(dependent.Key))
                    {
                        foreach (var sub in dependent.Value)
                        {
                            string bundleKey = bundleSource + sub.BundleName;
                            string jsonValue = catalogueData + sub.GUID + "/" + sub.GUID;
                            if (string.IsNullOrEmpty(sub.GUID) == true)
                            {
                                jsonValue = "";
                            }
                            if (individualItems.ContainsKey(bundleKey) == false && individualDependencies.ContainsKey(bundleKey) == false)
                            {
                                individualDependencies.Add(bundleKey, jsonValue);
                            }
                        }
                    }
                }
            }

            List<string> bundleItems = new List<string>();
            List<string> jsonItems = new List<string>();

            foreach (var item in individualItems)
            {
                bundleItems.Add(item.Key);
                if (string.IsNullOrEmpty(item.Value) == false)
                {
                    jsonItems.Add(item.Value);
                }
            }

            foreach (var item in individualDependencies)
            {
                bundleItems.Add(item.Key);
                if (string.IsNullOrEmpty(item.Value) == false)
                {
                    jsonItems.Add(item.Value);
                }
            }
            Debug.LogError($"individualItems : {individualItems.Count}, individualDependencies :{individualDependencies.Count}");

            CopyToServer server = new CopyToServer();
            List<string> jsonFiles = new List<string>();
            List<string> bundleFiles = new List<string>();
            jsonFiles.Add(dataSource + $"/bundles/{platformNameString}/Master.json");
            jsonFiles.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/index.json");
            jsonFiles.Add(dataSource + $"/BundleStreamingAssets/CatalogueData/entries/Combined.json");

            string att = dataSource + $"/BundleStreamingAssets/{variantName}/Data/AttributeData.json";
            if (File.Exists(att))
            {
                jsonFiles.Add(att);
            }

            string cata = dataSource + $"/BundleStreamingAssets/{variantName}/Data/CatalogueData.json";
            if (File.Exists(cata))
            {
                jsonFiles.Add(cata);
            }

            string mod = dataSource + $"/BundleStreamingAssets/{variantName}/Data/ModuleData.json";
            if (File.Exists(mod))
            {
                jsonFiles.Add(mod);
            }

            string cataloguethumbnail = dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnail.unity3d";
            bundleFiles.Add(cataloguethumbnail);
            jsonFiles.Add(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnail_Version.json");

            string cataloguethumbnails = dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnailgroup.unity3d";
            bundleFiles.Add(cataloguethumbnails);
            jsonFiles.Add(dataSource + $"/bundles/{platformNameString}/Catalogue/cataloguethumbnailgroup_Version.json");



            server.UploadJsonFileDirectory(catalogueData, () =>
            {
                server.UploadJsonFiles(jsonFiles, () =>
                {
                    server.UploadAssetBundles(bundleFiles, () =>
                    {
                        server.UploadJsonFiles(jsonItems, () =>
                        {
                            server.UploadAssetBundles(bundleItems, null);
                        });

                    });
                });

            });
#endif
        }


        public static void UploadSingleCatItem(CatalogueEntry catItem)
        {
            string platformNameString = GlobalConsts.CurrentPlatformName();
            string depPath = Device.StreamingPath + $"/bundles/{platformNameString}/master";
            var dependencies = Json.JsonNet.ReadFromFile<Dictionary<string, List<BundleLoader.DependencyData>>>(depPath);

            string dataSource = Device.StreamingPath;
            dataSource = dataSource.SanitiseSlashes();


            string bundleSource = dataSource + $"/bundles/{platformNameString}/";

            string catalogueData = dataSource + "/BundleStreamingAssets/CatalogueData/entries/";

            Dictionary<string, string> individualItems = new Dictionary<string, string>();
            Dictionary<string, string> individualDependencies = new Dictionary<string, string>();

            var render = catItem.GetMetaData<RenderMetaData>();
            string bundleKey = bundleSource + render.Main.Bundle;
            string jsonValue = catalogueData + catItem.Guid + "/" + catItem.Guid;
            if (individualItems.ContainsKey(bundleKey) == false)
            {
                individualItems.Add(bundleKey, jsonValue);
            }

            foreach (var dependent in dependencies)
            {
                foreach (var catDataItem in individualItems)
                {
                    if (catDataItem.Key.EndsWith(dependent.Key))
                    {
                        foreach (var sub in dependent.Value)
                        {
                            bundleKey = bundleSource + sub.BundleName;
                            jsonValue = catalogueData + sub.GUID + "/" + sub.GUID;
                            if (string.IsNullOrEmpty(sub.GUID) == true)
                            {
                                jsonValue = "";
                            }
                            if (individualItems.ContainsKey(bundleKey) == false && individualDependencies.ContainsKey(bundleKey) == false)
                            {
                                individualDependencies.Add(bundleKey, jsonValue);
                            }
                        }
                    }
                }
            }

            List<string> bundleItems = new List<string>();
            List<string> jsonItems = new List<string>();

            foreach (var item in individualItems)
            {
                bundleItems.Add(item.Key);
                if (string.IsNullOrEmpty(item.Value) == false)
                {
                    jsonItems.Add(item.Value);
                }
            }

            foreach (var item in individualDependencies)
            {
                bundleItems.Add(item.Key);
                if (string.IsNullOrEmpty(item.Value) == false)
                {
                    jsonItems.Add(item.Value);
                }
            }

            CopyToServer server = new CopyToServer();
            server.UploadJsonFiles(jsonItems, () =>
            {
                server.UploadAssetBundles(bundleItems, null);
            });
        }

        private void GenericBundleJsonFiles(string dataSource, Environments.EnvironmentVariantData variant, string baseDirectory, ref List<string> jsonFiles)
        {
            GlobalConsts.AssetBundleVersion data = new GlobalConsts.AssetBundleVersion();

            data.Version = variant.AudioClipVersion;
            data.FileSizeMB = 0;
            if (File.Exists(baseDirectory + "AudioClipAssetBundle.unity3d")) data.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(baseDirectory + "AudioClipAssetBundle.unity3d").Length);
            Json.JsonNet.WriteToFile(data, baseDirectory + "AudioClipAssetBundle_Version", true);
            jsonFiles.Add(baseDirectory + "AudioClipAssetBundle_Version");

            data.Version = variant.MaterialVersion;
            data.FileSizeMB = 0;
            if (File.Exists(baseDirectory + "MaterialAssetBundle.unity3d")) data.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(baseDirectory + "MaterialAssetBundle.unity3d").Length);
            Json.JsonNet.WriteToFile(data, baseDirectory + "MaterialAssetBundle_Version", true);
            jsonFiles.Add(baseDirectory + "MaterialAssetBundle_Version");

            data.Version = variant.Texture2DVersion;
            data.FileSizeMB = 0;
            if (File.Exists(baseDirectory + "Texture2DAssetBundle.unity3d")) data.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(baseDirectory + "Texture2DAssetBundle.unity3d").Length);
            Json.JsonNet.WriteToFile(data, baseDirectory + "Texture2DAssetBundle_Version", true);
            jsonFiles.Add(baseDirectory + "Texture2DAssetBundle_Version");

            data.Version = variant.TextVersion;
            data.FileSizeMB = 0;
            if (File.Exists(baseDirectory + "TextAssetBundle.unity3d")) data.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(baseDirectory + "TextAssetBundle.unity3d").Length);
            Json.JsonNet.WriteToFile(data, baseDirectory + "TextAssetBundle_Version", true);
            jsonFiles.Add(baseDirectory + "TextAssetBundle_Version");

            data.Version = variant.GameObjectVersion;
            data.FileSizeMB = 0;
            if (File.Exists(baseDirectory + "GameObjectAssetBundle.unity3d")) data.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(baseDirectory + "GameObjectAssetBundle.unity3d").Length);
            Json.JsonNet.WriteToFile(data, baseDirectory + "GameObjectAssetBundle_Version", true);
            jsonFiles.Add(baseDirectory + "GameObjectAssetBundle_Version");

            data.Version = variant.VideoClipVersion;
            data.FileSizeMB = 0;
            if (File.Exists(baseDirectory + "VideoClipAssetBundle.unity3d")) data.FileSizeMB = CopyToServer.ConvertBytesToMegabytes(new System.IO.FileInfo(baseDirectory + "VideoClipAssetBundle.unity3d").Length);
            Json.JsonNet.WriteToFile(data, baseDirectory + "VideoClipAssetBundle_Version", true);
            jsonFiles.Add(baseDirectory + "VideoClipAssetBundle_Version");


            data.Version = variant.CatalogueDataVersion;
            data.FileSizeMB = 0;
            Json.JsonNet.WriteToFile(data, dataSource + $"/BundleStreamingAssets/{variant.CombinedName}/Data/CatalogueData_Version", true);
            jsonFiles.Add(dataSource + $"/BundleStreamingAssets/{variant.CombinedName}/Data/CatalogueData_Version");
        }


        private void CreateDirectory(string sourcePath, string destinationPath, out int totalFiles, out int fileIndex, string[] extensions)
        {
            string[] directories = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories);
            for (int i = 0; i < directories.Length; i++)
            {
                string dirPath = directories[i];
                string directoryToCreate = dirPath.Replace(sourcePath, destinationPath);
                if (Directory.Exists(directoryToCreate) == false)
                {
                    Directory.CreateDirectory(directoryToCreate);
                }
                //ProgressBar.Show("Creating Directories", destinationPath, (i / (float)directories.Length));
            }

            totalFiles = 0;
            fileIndex = 0;
            for (int i = 0; i < extensions.Length; i++)
            {
                if (Directory.Exists(sourcePath) == true)
                {
                    string[] files = Directory.GetFiles(sourcePath, extensions[i], SearchOption.AllDirectories);
                    totalFiles += files.Length;
                }
            }
        }

        private void CopyFiles(string sourcePath, string destinationPath, string[] extensions)
        {
            CreateDirectory(sourcePath, destinationPath, out int totalFiles, out int fileIndex, extensions);

            for (int i = 0; i < extensions.Length; i++)
            {
                string[] files = Directory.GetFiles(sourcePath, extensions[i], SearchOption.AllDirectories);
                //Copy all the files & Replaces any files with the same name
                for (int x = 0; x < files.Length; x++)
                {
                    string newPath = files[x];
                    try
                    {
                        string finalDestination = newPath.Replace(sourcePath, destinationPath);
                        finalDestination = finalDestination.SanitiseSlashes();
                        File.Copy(newPath, finalDestination, true);
                    }
                    catch (System.Exception msg)
                    {
                        Debug.LogError($"Could not complete copy for: {newPath} please ensure file destination file is not being used! {msg.Message}");
                    }
                    ProgressBar.Show("Copying Data", $"From {sourcePath} to {destinationPath}", fileIndex / (float)totalFiles);
                    fileIndex++;
                }
            }

            ProgressBar.Clear();

            Debug.Log($"{sourcePath} copied to: \"{destinationPath}\"\n");
        }



        private void CopyFilesVarientAssetBundles(string sourcePath, string destinationPath, string[] extensions)
        {
            int totalFiles = 0;
            int fileIndex = 0;
            CreateDirectory(sourcePath, destinationPath, out totalFiles, out fileIndex, extensions);

            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var Variant = environmentData.GetCurrentVariant();

            for (int i = 0; i < extensions.Length; i++)
            {
                var files = Directory.GetFiles(sourcePath, extensions[i], SearchOption.AllDirectories).ToList();
                string combinedName = Variant?.CombinedName;

                if (true == string.IsNullOrEmpty(combinedName))
                {
                    Debug.LogError("Variant not set, please set a variant in the environment window.");
                    return;
                }

                if (Variant.UseOverloadData == true)
                {
                    combinedName = Variant.OverloadCombinedName;
                }


                var contain = files.FindAll(e => (e.SanitiseSlashes().CaseInsensitiveContains(combinedName) || e.SanitiseSlashes().CaseInsensitiveContains(Variant.Guid) || e.SanitiseSlashes().CaseInsensitiveContains("master") || e.SanitiseSlashes().CaseInsensitiveContains("logos.unity3d")));
                //Copy all the files &Replaces any files with the same name
                for (int x = 0; x < contain.Count; x++)
                {
                    string newPath = contain[x];
                    try
                    {
                        string finalDestination = newPath.Replace(sourcePath, destinationPath);
                        finalDestination = finalDestination.SanitiseSlashes();
                        File.Copy(newPath, finalDestination, true);
                        Debug.Log($"{contain[x]} copied to: \"{finalDestination}\"\n");
                    }
                    catch (System.Exception msg)
                    {
                        Debug.LogError($"Could not complete copy for: {newPath} please ensure file destination file is not being used! {msg.Message}");
                    }
                    ProgressBar.Show("Copying Data", $"From {sourcePath} to {destinationPath}", fileIndex / (float)totalFiles);
                    fileIndex++;
                }
            }
            ProgressBar.Clear();

            Debug.Log($"{sourcePath} copied to: \"{destinationPath}\"\n");
        }

        private void CopyFilesVarient(string sourcePath, string destinationPath, string[] extensions)
        {
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var Variant = environmentData.GetCurrentVariant();

            int totalFiles = 0;
            int fileIndex = 0;
            CreateDirectory(sourcePath, destinationPath, out totalFiles, out fileIndex, extensions);

            string combined = $"{Variant.CombinedName}";
            for (int i = 0; i < extensions.Length; i++)
            {
                var fileList = Directory.GetFiles(sourcePath, extensions[i], SearchOption.AllDirectories).ToList();
                var contain = fileList.FindAll(e => (e.SanitiseSlashes().CaseInsensitiveContains(combined)));
                //Copy all the files & Replaces any files with the same name
                for (int x = 0; x < contain.Count; x++)
                {
                    string newPath = contain[x];
                    try
                    {
                        string finalDestination = newPath.Replace(sourcePath, destinationPath);
                        finalDestination = finalDestination.SanitiseSlashes();
                        File.Copy(newPath, finalDestination, true);
                    }
                    catch (System.Exception msg)
                    {
                        Debug.LogError($"Could not complete copy for: {newPath} please ensure file destination file is not being used! {msg.Message}");
                    }
                    ProgressBar.Show("Copying Data", $"From {sourcePath} to {destinationPath}", fileIndex / (float)totalFiles);
                    fileIndex++;
                }
            }

            ProgressBar.Clear();

            Debug.Log($"{sourcePath} copied to: \"{destinationPath}\"\n");
        }


        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }


        private static void CopyFilesVarientIndexNoMenu(string sourcePath, string destinationPath, Environments.EnvironmentVariantData variant)
        {
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();

            // linkage to EnvironmentDataManager.cs  CreateSeperateJsonFile
            string variantGuid = environmentData.GetCurrentVariant().Guid;
            string item = EditorTools.Environments.EnvironmentDataManager.DataRoot.Append("_" + variantGuid).Absolute + ".json";

            string finalDestonation = EditorTools.Environments.EnvironmentDataManager.DataRoot.Append("index").Absolute + ".json";
            finalDestonation = finalDestonation.Replace(sourcePath, destinationPath);
            File.Copy(item, finalDestonation, true);

            // remove unwanted folders
            string enviromentGuid = environmentData.GetCurrentVariant().Environment.Guid;
            string clean = EditorTools.Environments.EnvironmentDataManager.DataRoot.Absolute.Replace(sourcePath, destinationPath);
            var directorys = Directory.GetDirectories(clean, "*.*");
            for (int i = 0; i < directorys.Length; i++)
            {
                var directory = directorys[i];
                if (directory.EndsWith(enviromentGuid) == false)
                {
                    Directory.Delete(directory, true);
                }
            }
        }


        private void CopyVariantCatalogueJsonFiles(string dataSource, string sourcePath, string destinationPath, string extension, Environments.EnvironmentVariantData variant)
        {
            string[] extensions = new string[1];
            extensions[0] = extension;
            CreateDirectory(sourcePath, destinationPath, out int totalFiles, out int fileIndex, extensions);

            var ss = variant.OverloadCombinedName;
            var entries = Catalogue.Instance.Entries;
            entries = entries.FindAll(e => e.EnvironmentsAvailable.Contains(variant.OverloadCombinedName)); // only correct enviroment items used 

            List<string> copiedFiles = new List<string>();
            string startPath = $"{dataSource}/BundleStreamingAssets/CatalogueData/entries";
            foreach (var item in entries)
            {
                var newPath = startPath + $"/{item.Guid}/{item.Guid}.json";
                newPath = newPath.SanitiseSlashes();
                CopyFile(sourcePath, destinationPath, newPath, copiedFiles);
            }

        }

        private void CopyVariantCatalogueExtraBundleFiles(string bundleSource, string sourcePath, string destinationPath, Environments.EnvironmentVariantData variant)
        {
            var extraList = GlobalConsts.GetExternalSourceAssetBundlesInMask(variant.ExtraBundlesToInclude);
            List<string> copiedFiles = new List<string>();
            foreach (var item in extraList)
            {
                var newPath = $"{bundleSource}/bundles/{GlobalConsts.CurrentPlatformName()}/catalogue/{item}.unity3d";
                newPath = newPath.SanitiseSlashes();
                CopyFile(sourcePath, destinationPath, newPath, copiedFiles);
            }
        }

        private void CopyVariantCatalogueBundleFiles(string bundleSource, string dataSource, string sourcePath, string destinationPath, string extension, Environments.EnvironmentVariantData variant)
        {
            if (variant.IncludeCataloguePrefabs == false)
            {
                return;
            }
            string file = $"{dataSource}/bundles/{GlobalConsts.CurrentPlatformName()}/master";
            file = file.SanitiseSlashes();
            var dependleDependencies = Json.JsonNet.ReadFromFile<Dictionary<string, List<BundleLoader.DependencyData>>>(file);

            string[] extensions = new string[1];
            extensions[0] = extension;
            CreateDirectory(sourcePath, destinationPath, out int totalFiles, out int fileIndex, extensions);

            var entries = Catalogue.Instance.Entries;
            entries = entries.FindAll(e => e.EnvironmentsAvailable.Contains(variant.OverloadCombinedName)); // only correct enviroment items used 




            List<string> copiedFiles = new List<string>();
            foreach (var entry in entries)
            {
                var renderData = entry.GetMetaData<RenderMetaData>();
                if (renderData == null)
                {
                    // does not have RenderMetaData
                    continue;
                }



                if (renderData == null || renderData.Main == null || string.IsNullOrEmpty(renderData.Main.Bundle) == true)
                {
                    Debug.LogError($"Problem with renderData entry.Guid: {entry.Guid}");
                }
                else
                {
                    //copy main catalogue .unity3d
                    var newPath = $"{bundleSource}/{renderData.Main.Bundle}";
                    CopyFile(sourcePath, destinationPath, newPath, copiedFiles);


                    // copy dependnacy .unity3d
                    if (dependleDependencies.ContainsKey(renderData.Main.Bundle) == true)
                    {
                        foreach (var dep in dependleDependencies[renderData.Main.Bundle])
                        {
                            string combinedName = $"{bundleSource}/{dep.BundleName}";
                            CopyFile(sourcePath, destinationPath, combinedName, copiedFiles);
                        }
                    }
                }

                ProgressBar.Show("Copying Data", $"From {sourcePath} to {destinationPath}", fileIndex / (float)totalFiles);
                fileIndex++;
            }


            ProgressBar.Clear();

            Debug.Log($"{sourcePath} copied to: \"{destinationPath}\"\n");
        }

        private static void CopyFile(string sourcePath, string destinationPath, string newPath, List<string> copiedFiles, bool makeDirectory = false)
        {
            string finalDestination = "";
            try
            {
                finalDestination = newPath.Replace(sourcePath, destinationPath);
                finalDestination = finalDestination.SanitiseSlashes();
                if (copiedFiles != null && copiedFiles.Contains(finalDestination) == true)
                {
                    return;
                }
                if (File.Exists(newPath) == true)
                {
                    string newDirectoryName = Path.GetDirectoryName(finalDestination);
                    if (makeDirectory == true && Directory.Exists(newDirectoryName) == false)
                    {
                        Directory.CreateDirectory(newDirectoryName);
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(finalDestination));
                    File.Copy(newPath, finalDestination, true);
                    if (copiedFiles != null)
                    {
                        copiedFiles.Add(finalDestination);
                    }
                    Debug.Log($"CopyFiles :{newPath}, dest :{finalDestination}");
                }
                else
                {
                    Debug.LogError($"Cannot find file {newPath}");
                }
            }
            catch (System.Exception msg)
            {
                Debug.LogError($"Could not complete copy for: {newPath}   : {finalDestination}     please ensure the destination file is not being used! {msg.Message}");
            }
        }


        public void RemoveFolders(string DestinationPath, string[] folderNamesToFind)
        {
            string[] allDirectorys = Directory.GetDirectories(DestinationPath, "*", SearchOption.TopDirectoryOnly);
            List<string> allFoldersToRemove = new List<string>();
            for (int i = 0; i < folderNamesToFind.Length; i++)
            {
                string[] foundFolders = System.Array.FindAll(allDirectorys, x => x.EndsWith(folderNamesToFind[i], true, System.Globalization.CultureInfo.CurrentCulture));
                allFoldersToRemove.AddRange(foundFolders.ToList());
            }
            for (int i = 0; i < allFoldersToRemove.Count; i++)
            {
                if (Directory.Exists(allFoldersToRemove[i]) == true)
                {
                    Directory.Delete(allFoldersToRemove[i], true);
                }
            }
        }

        public static void RemoveUnwantedVariantFolders(string basePath)
        {
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var Variant = environmentData.GetCurrentVariant();

            if (Directory.Exists(basePath) == true)
            {
                string combined = $"{basePath}\\{Variant.CombinedName}";
                if (Variant.UseOverloadData == true)
                {
                    combined = $"{basePath}\\{Variant.OverloadCombinedName}";
                }
                combined = combined.SanitiseSlashes();
                string[] allDirectories = Directory.GetDirectories(basePath, "*", SearchOption.AllDirectories);

                for (int i = 0; i < allDirectories.Length; i++)
                {
                    string directory = allDirectories[i];
                    directory = directory.SanitiseSlashes();
                    if (combined.CaseInsensitiveContains(directory) == false)
                    {
                        if (Directory.Exists(directory) == true)
                        {
                            Directory.Delete(directory, true);
                        }
                    }
                }
            }
        }

        public static void RemoveUnwantedFolders(string basePath)
        {
            if (Directory.Exists(basePath) == true)
            {
                Directory.Delete(basePath, true);
            }
        }

        public static void RemoveUnwantedFile(string file)
        {
            if (File.Exists(file) == true)
            {
                File.Delete(file);
            }
            else
            {
                Debug.LogError($"Could not find file {file}");
            }
        }



        public void RemoveJunkFilesFolders(string DestinationPath)
        {
            var files = Directory.GetFiles(DestinationPath, "Keep.json", SearchOption.AllDirectories).ToList();
            foreach (var file in files)
            {
                File.Delete(file);
            }

            string[] allDirectorys = Directory.GetDirectories(DestinationPath, "*", SearchOption.AllDirectories);
            foreach (var directory in allDirectorys)
            {
                if (Directory.Exists(directory) == true)
                {
                    var filess = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories);
                    if (filess.Length == 0)
                    {
                        Directory.Delete(directory, true);
                    }
                }
            }


            files = Directory.GetFiles(DestinationPath, "*.json", SearchOption.AllDirectories).ToList();
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                file = file.SanitiseSlashes();

                var list = file.SplitList("/");
                var partLookingFor = list[list.Count - 1];
                if (partLookingFor.StartsWith("_") == true)
                {
                    File.Delete(file);
                }
            }
        }

        private void DrawBundleSetupControls()
        {
            var bundleLabels = AssetDatabase.GetAllAssetBundleNames();
            m_ShowLabels = EditorGUILayout.Foldout(m_ShowLabels, $"Asset Bundles ({bundleLabels.Length})", true);
            if (m_ShowLabels == true)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Remove Unused Labels", GUILayout.Width(160)) == true)
                    {
                        AssetDatabase.RemoveUnusedAssetBundleNames();
                    }
                    if (GUILayout.Button("Remove Unused Bundles", GUILayout.Width(160)) == true)
                    {
                        var unusedBundles = AssetBundleBuilder.GetUnusedBundles();
                        using (var progressBar = new ProgressBar("Removing Unused Bundles", unusedBundles))
                        {
                            foreach (var unusedBundle in unusedBundles)
                            {
                                progressBar.UpdateIncrement($"Removing {unusedBundle.name}");

                                var path = unusedBundle.GetAssetPath();
                                AssetDatabase.DeleteAsset(path.Relative);
                                AssetDatabase.DeleteAsset(path.AppendExt(".manifest").Relative);
                            }
                        }
                        FolderCleaner.CleanEmptyFoldersIn(AssetBundleBuilder.StagingFolder(GlobalConsts.CurrentPlatformName()).Absolute);
                    }
                }
                using (new GuiIndentScope())
                {
                    var btnWidth = GUILayout.Width(100);
                    var btnStyle = GUI.skin.button.Clone();
                    btnStyle.fontSize = EditorStyles.miniLabel.fontSize;
                    btnStyle.font = EditorStyles.miniLabel.font;

                    var unusedLabels = AssetDatabase.GetUnusedAssetBundleNames().Hash();
                    foreach (var label in bundleLabels)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            var style = EditorStyles.miniLabel.Clone();
                            if (unusedLabels.Contains(label) == true)
                            {
                                style.normal.textColor = Color.red;
                            }
                            EditorGUILayout.LabelField(label, style);

                            if (GUILayout.Button("Build", btnStyle, btnWidth) == true)
                            {
                                AssetBundleBuilder.BuildBundle(m_Platform, label);
                                Debug.Log($"Asset bundle {{{label}}} build complete\n");
                            }

                            var bundle = AssetBundleBuilder.GetBuiltBundle(label);
                            using (new GuiEnabledScope(bundle != null))
                            {
                                if (GUILayout.Button("Select Bundle", btnStyle, btnWidth) == true)
                                {
                                    EditorHelper.SelectAndPingObject(bundle);
                                }
                            }
                            if (GUILayout.Button("Select Assets", btnStyle, btnWidth) == true)
                            {
                                var assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(label);
                                var assets = new List<UnityEngine.Object>(assetPaths.Length);
                                foreach (var path in assetPaths)
                                {
                                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                                    EditorGUIUtility.PingObject(asset);
                                    assets.Add(asset);
                                }
                                Selection.objects = assets.ToArray();
                            }
                        }
                    }
                }
            }

            EditorGUILayout.Space();
        }
    }
}
