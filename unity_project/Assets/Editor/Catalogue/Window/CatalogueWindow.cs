using EditorTools.Bundles;
using FilePathHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class CatalogueWindow : EditorWindow
    {
        [MenuItem("Window/Custom/Catalogue Editor", false, 0)]
        public static CatalogueWindow ShowCatalogueEditor() => GetWindow<CatalogueWindow>("Catalogue");

        [SerializeField]
        private string m_SelectedPreset;
        [SerializeField]
        private string m_SelectedEntry;

        private List<IEditorView> m_Tabs;
        private int m_TabIndex;


        private CatalogueEntryTab m_CatalogueEntryTab;
        public void ReLoadCatalogueEntryTab() => m_CatalogueEntryTab.OnDataReSelected();
        public void Initialise()
        {
            VideoLoader.CreateInstance();
            VideoLoader.Instance.OnVideoSequenceLoaded(null);

            if (Catalogue.Instance == null)
            {
                Catalogue.CreateInstance();
                using (var progress = new ProgressBar("Loading Catalogue For All"))
                {
                    Catalogue.Instance.LoadFromFile(str => progress.UpdateIncrement(str), c => progress.SetMaxIncrement(c), null);
                }
            }

            m_CatalogueEntryTab = new CatalogueEntryTab(m_SelectedEntry);
            m_CatalogueEntryTab.EntrySelected += id => m_SelectedEntry = id;
            m_CatalogueEntryTab.SaveCatalogue += SaveCatalogueToFile;
            m_CatalogueEntryTab.Repaint += Repaint;

            var presetTab = new CataloguePresetTab(Catalogue.Instance, m_SelectedPreset);
            presetTab.PresetSelected += id => m_SelectedPreset = id;
            presetTab.Repaint += Repaint;

            m_Tabs = new List<IEditorView>
            {
                m_CatalogueEntryTab, presetTab,
                new ProfileDataTab(),
                new BrandTab(Catalogue.Instance),
                new CategoriesTab(Catalogue.Instance),
            };

            EditorHelper.UnfocusControls();
        }

        private void ValidateLighting()
        {
            List<string> lighting = new List<string>();
            foreach (var item in Catalogue.Instance.Entries)
            {
                var data = item.GetMetaData<ContentLightingMetaData>();
                if (data != null)
                {
                    if (lighting.Contains(data.m_JsonFileName) == true)
                    {
                        Debug.LogError($"Duplicate datain lights {data.m_JsonFileName}");
                    }
                    else
                    {
                        lighting.Add(data.m_JsonFileName);
                    }

                }
            }
        }

        private void OnSelectionChange() => Repaint();

        private void OnGUI()
        {
            EditorGUIUtility.wideMode = true;

            if (m_Tabs != null)
            {
                var tabNames = m_Tabs.Extract(t => t.Name).ToArray();
                int index = GUILayout.Toolbar(m_TabIndex, tabNames);
                if (m_TabIndex != index)
                {
                    EditorHelper.UnfocusControls();
                    m_TabIndex = index;
                    m_Tabs[m_TabIndex].OnShow();
                }

                EditorGUILayout.Space();
                m_Tabs[m_TabIndex].OnGUI();
            }
            else
            {
                GUILayout.FlexibleSpace();
            }

            EditorHelper.Separator();

            bool isLoaded = (Catalogue.Instance?.Entries != null);
            if (isLoaded == false)
            {
                RemoveNotification();
                ShowNotification(new GUIContent("Catalogue not loaded"));
            }

            using (new GuiEnabledScope(!EditorApplication.isCompiling))
            {
                DrawGlobalControls(isLoaded);
            }
        }

        private void DrawGlobalControls(bool isLoaded)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new GuiEnabledScope(isLoaded))
                {
                    if (GUILayout.Button("Reload Localisation & External Data") == true)
                    {
                        Localisation.Instance.LoadLanguage(Localisation.Language.English);
                        Catalogue.Instance.OnPostLoad();
                    }
                }
                if (GUILayout.Button("View Catalogue") == true)
                {
                    Initialise();
                }
                using (new GuiEnabledScope(isLoaded))
                {
                    if (GUILayout.Button("Save Catalogue") == true)
                    {
                        var timer = Stopwatch.Diagnostics();
                        ValidateCatalogue();
                        SaveCatalogueToFile(justAdded: false, withVersion: false, false);
                        timer.Stop();
                        EditorApplication.Beep();
                        EditorUtility.DisplayDialog("Completed", $"Save Catalogue, took:{timer.Readable()} ", "Ok");
                    }
                    if (GUILayout.Button("Print brands") == true)
                    {
                        PrintBrands();
                        EditorUtility.DisplayDialog("Completed", $"Print brands to desktop ", "Ok");
                    }
                }

                if (GUILayout.Button("CheckCatalogue") == true)
                {
                    CheckCatalogue(out int errorCount);
                    if (errorCount == 0)
                    {
                        EditorUtility.DisplayDialog("Completed", $"CheckCatalogue No errors", "Ok");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Completed", $"CheckCatalogue errors {errorCount}, check console", "Ok");
                    }
                }
                if (GUILayout.Button("Label Catalogue Folders") == true)
                {
                    PrefabManager.LabelCatalogueFolders();
                }
            }
            EditorGUILayout.Space();
        }

        private void ValidateCatalogue()
        {

            ContentLightingDataView.InitiliseFeaturesList(true);

            var fixtures = ContentLightingDataView.m_FixtureDictionary;

            foreach (var item in fixtures)
            {
                List<string> dulpicateList = new List<string>();
                foreach (var rootKey in fixtures.Keys)
                {
                    if (fixtures[rootKey].Name == item.Value.Name)
                    {
                        dulpicateList.Add(rootKey);
                    }
                }
                if (dulpicateList.Count > 1)
                {
                    Debug.LogError($"Duplicate {dulpicateList.Count}");
                    foreach (var dulpicate in dulpicateList)
                    {
                        Debug.LogError($"Uses the same name, :   key: {dulpicate}");
                    }
                }
            }

            int count = 0;
            foreach (var item in Catalogue.Instance.Entries)
            {
                Debug.Log($" item  {item.FullName}");
                var renderMetaData = item.GetMetaData<RenderMetaData>();


                // remove unwanted MetaDataType , leaving in case need to do it again 
                //////////////List<MetaDataType> items = new List<MetaDataType>();
                //////////////items.Add(MetaDataType.Snapping);
                //////////////items.Add(MetaDataType.Loudspeaker);
                //////////////items.Add(MetaDataType.Mechanical);
                //////////////items.Add(MetaDataType.StagingGizmo);
                //////////////items.Add(MetaDataType.Validation);
                //////////////items.Add(MetaDataType.Stackable);
                //////////////items.Add(MetaDataType.GenerativeSupport);
                //////////////items.Add(MetaDataType.GeneratedStand);
                //////////////items.Add(MetaDataType.GeneratedHoist);
                //////////////items.Add(MetaDataType.GenerateStageDeck);
                //////////////items.Add(MetaDataType.Trussing);
                //////////////items.Add(MetaDataType.Packable);
                //////////////items.Add(MetaDataType.WeightDistribution);
                //////////////items.Add(MetaDataType.InformationLinkage);


                //////////////foreach(var metaDat in items)
                //////////////{
                //////////////    if (item.MetaData.ContainsKey(metaDat) == true)
                //////////////    {
                //////////////        item.MetaData[metaDat] = null;
                //////////////        item.MetaData.Remove(metaDat);
                //////////////    }
                //////////////}

                if (renderMetaData == null || renderMetaData.Main == null)
                {
                    Debug.LogError("renderMetaData error");
                }

                var prefab = LoadPrefab(renderMetaData.Main);
                if (prefab == null)
                {
                    Debug.LogError($"prefab is null for: {item.FullName}");
                    continue;
                }
                var instance = GameObject.Instantiate(prefab) as GameObject;

                foreach (var meta in item.MetaData)
                {
                    meta.Value.CollectAllData(instance);
                }

                // all ContentStagingMetaData use all colliders
                if (item.GetMetaData<ContentStagingMetaData>() != null)
                {
                    ContentPlacementMetaData data = item.GetMetaData<ContentPlacementMetaData>();
                    if (data != null)
                    {
                        if (data.m_UseAllMainColliders == false)
                        {
                            data.m_UseAllMainColliders = true;
                            count++;
                        }

                    }
                }

                var all = instance.GetComponentsInChildren<Transform>(true).ToList();
                foreach (var trans in all)
                {
                    if (trans.name.Contains("/") == true)
                    {
                        Debug.LogError($"{item.FullName} , there is a transform with / in it which will break everythging");
                    }
                }

                var allBoxColliders = instance.GetComponentsInChildren<BoxCollider>(true).ToList();
                foreach (var box in allBoxColliders)
                {
                    if (box.size.x == 0 || box.size.x == 0 || box.size.x == 0)
                    {
                        Debug.LogError($"{item.FullName} , box collider has a zero size somewere , this can cause issues");
                    }
                }



                List<string> lighting = new List<string>();
                var lightingMetaData = item.GetMetaData<ContentLightingMetaData>();
                if (lightingMetaData != null)
                {
                    if (lighting.Contains(lightingMetaData.m_JsonFileName) == true)
                    {
                        Debug.LogError($"{item.FullName} Duplicate datain lights {lightingMetaData.m_JsonFileName}");
                    }
                    else
                    {
                        lighting.Add(lightingMetaData.m_JsonFileName);
                    }
                }


                var audioData = item.GetMetaData<ContentSteamAudioMetaData>();
                if (audioData != null)
                {
                    if (audioData.m_SoundEnumType == ContentSteamAudioMetaData.SoundEnumType.Play)
                    {
                        var audio = prefab.GetComponentInChildren<AudioSource>(true);
                        if (audio == null)
                        {
                            Debug.LogError($"{item.FullName} ContentSteamAudioMetaData needs to have an audio source {item.Guid}");
                        }
                    }
                }
                var meshFilters = prefab.GetComponentsInChildren<MeshFilter>(true);
                for (int i = 0; i < meshFilters.Length; i++)
                {
                    if (meshFilters[i].sharedMesh == null)
                    {
                        Debug.LogError($"{item.FullName}  {meshFilters[i].gameObject.name} meshFilters[i].sharedMesh is null  {item.Guid}");
                    }
                }

                UnityEngine.Object.DestroyImmediate(instance);
            }
            AssetDatabase.SaveAssets();
            Debug.LogError($"Made {count} changes");

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

        private void CheckCatalogue(out int count)
        {
            string languageFile = $"{Application.dataPath}/StreamingAssets/BundleStreamingAssets/LanguageData/english.json";
            var jsonLanguage = Json.JsonNet.ReadFromFile<Dictionary<string, string>>(languageFile);


            string entriesFolder = $"{Application.dataPath}/StreamingAssets/BundleStreamingAssets/CatalogueData/entries";
            string[] dirs = Directory.GetDirectories(entriesFolder, "*", SearchOption.TopDirectoryOnly);

            string entriesFile = $"{entriesFolder}/index.json";
            var json = Json.JsonNet.ReadFromFile<List<string>>(entriesFile);

            count = 0;
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                dir = dir.SanitiseSlashes();
                var dirClipped = dir.Split('/').Last();
                if (json.Contains(dirClipped) == false)
                {
                    Debug.LogError($"Not Found : {dirClipped}");
                    count++;
                }
                else
                {
                    json.Remove(dirClipped);
                }
            }

            count += json.Count;
            foreach (var item in json)
            {
                Debug.LogError($"Extra found in Json: {item}");
            }

            foreach (var item in jsonLanguage)
            {
                if (string.IsNullOrEmpty(item.Value) == true)
                {
                    count++;
                    Debug.LogError($"Extra found languageFile: {item.Key}");
                }
            }


            int f = 0;

        }
        private void PrintBrands()
        {
            SimpleDictionaryList<string, string> brandList = new SimpleDictionaryList<string, string>();
            foreach (var item in Catalogue.Instance.Entries)
            {
                var filters = item.CatalogueFilters;
                foreach (var filter in filters)
                {
                    if (filter.StartsWith("Brand", System.StringComparison.CurrentCultureIgnoreCase) == true)
                    {
                        brandList.AddToList(filter, item.FullName.ToString());
                    }
                }
            }

            var filePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            filePath = $"{filePath}/BrandList.txt";

            StreamWriter sw = new StreamWriter(filePath);

            foreach (var brand in brandList.GetKeys())
            {
                sw.WriteLine($"Brand {brand}");
                var items = brandList.GetList(brand);
                foreach (var item in items)
                {
                    sw.WriteLine($"     {item}");
                }


            }
            //Close the file
            sw.Close();

        }

        private void SaveCatalogueToFile(bool justAdded, bool withVersion, bool showErrors)
        {
            foreach (var item in Catalogue.Instance.Entries)
            {
                var steam = item.GetMetaData<ContentSteamAudioMetaData>();
                if (steam == null)
                {
                    Debug.LogError($"{item.ShortName} ContentSteamAudioMetaData added");
                    item.AddMetaData(MetaDataType.ContentSteamAudio);
                }

                var place = item.GetMetaData<ContentPlacementMetaData>();
                if (place == null)
                {
                    Debug.LogError($"{item.ShortName} ContentPlacementMetaData added");
                    item.AddMetaData(MetaDataType.ContentPlacement);
                }


                var data = item.GetMetaData<RenderMetaData>();
                var assignedPrefabGameObject = CatalogueViewer.PrefabManager.LoadPrefab(data.Main);
                if (assignedPrefabGameObject == null)
                {
                    Debug.LogError($"Should never hit: {data.Main.Bundle}");
                }
                else
                {
                    var allChildren = assignedPrefabGameObject.transform.GetComponentsInChildren<Transform>(true);
                    foreach (var singleItem in allChildren)
                    {
                        if ((singleItem.gameObject != assignedPrefabGameObject) && (singleItem.tag != Layers.UnTaggedTag))
                        {
                            Debug.LogError($"{item.ShortName} Layers.UnTaggedTag added, Tag was {singleItem.tag}");
                            if (singleItem.tag != GlobalConsts.RootCatalogueItemTag)
                            {
                                singleItem.tag = Layers.UnTaggedTag;
                            }
                        }
                    }

                    if (assignedPrefabGameObject.transform.tag != GlobalConsts.RootCatalogueItemTag)
                    {
                        Debug.LogError($"{item.ShortName} RootCatalogueItemTag added, Tag was {assignedPrefabGameObject.transform.tag}");
                        assignedPrefabGameObject.transform.tag = GlobalConsts.RootCatalogueItemTag;
                    }

                    foreach (var singleItem in allChildren)
                    {
                        if (assignedPrefabGameObject.gameObject.layer != Layers.VrInteractionLayer)
                        {
                            Debug.LogError($"{item.ShortName} VrInteractionLayer added");
                            singleItem.gameObject.layer = Layers.VrInteractionLayer;
                        }
                    }

                    var metaDataTypes = item.GetMetaDataTypes();
                    if (metaDataTypes.Contains(MetaDataType.ContentPickUp) ||
                        metaDataTypes.Contains(MetaDataType.ContentDrink) ||
                        metaDataTypes.Contains(MetaDataType.ContentFood))
                    {
                        assignedPrefabGameObject.ForceComponent<Rigidbody>();
                        assignedPrefabGameObject.ForceComponent<CollisionSound>();
                        var sound = assignedPrefabGameObject.ForceComponent<AudioSource>();
                        sound.playOnAwake = false;
                        sound.spatialBlend = 1f;

                        // safety check on all 
                        var all = assignedPrefabGameObject.GetComponentsInChildren<AudioSource>();
                        foreach (var audio in all)
                        {
                            audio.playOnAwake = false;
                            audio.spatialBlend = 1f;
                            if (audio.maxDistance == 500)
                            {
                                audio.maxDistance = 10;
                            }
                        }
                    }

                    // turn off PlacementCollisions
                    var PlacementCollision = assignedPrefabGameObject.transform.Find(GlobalConsts.CollisionObj);
                    PlacementCollision.GetComponentsInChildren<Collider>().ToList().ForEach(e => e.enabled = false);

                    var colliderChild = assignedPrefabGameObject.gameObject.transform.GetChild(0);
                    if (colliderChild.name != GlobalConsts.CollisionObj)
                    {
                        Debug.LogError($"{item.ShortName} CollisionObj added");
                        colliderChild.name = GlobalConsts.CollisionObj;
                    }


                    var modelChild = assignedPrefabGameObject.gameObject.transform.GetChild(1);
                    if (modelChild.name != GlobalConsts.RootModel)
                    {
                        Debug.LogError($"{item.ShortName} RootModel added");
                        modelChild.name = GlobalConsts.RootModel;
                    }

                    var meshColliders = assignedPrefabGameObject.GetComponentsInChildren<MeshCollider>();
                    for (int i = 0; i < meshColliders.Length; i++)
                    {
                        if (meshColliders[i].convex != true)
                        {
                            Debug.LogError($"{item.ShortName} convex = true added");
                            meshColliders[i].convex = true;
                        }
                    }


                    //not needed but will keep RemoveHologram(item, data, assignedPrefab);
                    if ((assignedPrefabGameObject == null) && (showErrors == true))
                    {
                        Debug.LogError($"Error in catalogueItem {item.FullName} cant find the renderer");
                    }
                }
            }

            AssetDatabase.SaveAssets();
            EditorHelper.UnfocusControls();
            Catalogue.Instance.SetCurrentPlatform(GlobalConsts.CurrentPlatformName());


            using (var progress = new ProgressBar("Saving Catalogue", Catalogue.Instance.SaveCount))
            {
                Catalogue.Instance.SaveToFile(justAdded, Catalogue.Instance.Entries, Catalogue.Instance.Presets, showErrors, str => progress.UpdateIncrement(str));
            }
            CleanUnusedGuidsInFolder(Catalogue.Instance);
        }

        private void RemoveHologram(CatalogueEntry item, RenderMetaData data, GameObject assignedPrefab)
        {
            var newItem = Instantiate(assignedPrefab);
            newItem.name = newItem.name.Replace("(Clone)", "");
            var hol = newItem.transform.SearchComponent<Transform>("Hologram");
            if (hol != null)
            {
                hol.gameObject.DestroyObjectImmediate();
            }
            else
            {
                Debug.LogError($"{item.ShortName}");
            }

            string path = AssetDatabase.GUIDToAssetPath(data.Main.Guid);
            PrefabUtility.SaveAsPrefabAsset(newItem, path, out bool yes);

            newItem.DestroyObjectImmediate();
        }

        public static void CleanUnusedGuidsInFolder(Catalogue catalogue)
        {
            var presetIndex = catalogue.Presets.Extract(e => e.Guid).Hash();
            var entryIndex = catalogue.Entries.Extract(e => e.Guid).Hash();
            CleanUnusedGuidsInFolder(presetIndex, Catalogue.PresetsFolder);
            CleanUnusedGuidsInFolder(entryIndex, Catalogue.EntriesFolder);
            AssetDatabase.Refresh();
        }

        private static void CleanUnusedGuidsInFolder(HashSet<string> index, string targetFolder)
        {
            Utils.IO.ForceFolder(targetFolder);

            var folders = Directory.GetDirectories(targetFolder);
            for (int i = 0; i < folders.Length; ++i)
            {
                var folderPath = FilePath.FromAbsolute(folders[i]);
                if (index.Contains(folderPath.FileName) == false)
                {
                    AssetDatabase.MoveAssetToTrash(folderPath.Relative);
                }
            }

            FolderCleaner.CleanEmptyFoldersIn(targetFolder, true);
        }
    }

    public interface IEditorView
    {
        string Name { get; }

        void OnShow();
        void OnGUI();
    }
}
