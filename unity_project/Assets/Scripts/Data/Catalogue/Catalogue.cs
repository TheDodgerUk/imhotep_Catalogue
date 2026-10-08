using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Catalogue : Singleton<Catalogue>
{
    private const float DIFFERNCE = 2f;

    //must be same in CatalogueProgram and main program 
    public static readonly string DataFolder = $"{GlobalConsts.BUNDLE_STREAMING}";
    public static readonly string MetaFolder = $"{Device.StreamingPath}/{GlobalConsts.MetaFolder}";
    public static readonly string CatalogueFolder = $"{DataFolder}/CatalogueData";
    public static readonly string EntryCombinedPath = $"{CatalogueFolder}/entries/Combined";
    public static readonly string DescriptionPath = $"{CatalogueFolder}/description";
    public static readonly string ProfilesFolder = $"{CatalogueFolder}/profiles";
    public static readonly string EntriesFolder = $"{CatalogueFolder}/entries";
    public static readonly string PresetsFolder = $"{CatalogueFolder}/presets";
    public static readonly string EntriesIndexPath = $"{EntriesFolder}/index";
    public static readonly string PresetsIndexPath = $"{PresetsFolder}/index";
    public static readonly string CatalogueFiltersFolder = $"{CatalogueFolder}/filters";

    private const JsonLibraryType JsonLib = JsonLibraryType.JsonNet;
    public const string LocalisationKey = "catalogue";

    public CatalogueDescription Description { get; private set; }
    public List<CataloguePreset> Presets { get; private set; }
    public List<CatalogueEntry> Entries { get; private set; }
    public int SaveCount => Entries.Count * 2 + Presets.Count;

    private string m_Platform;
    public void SetCurrentPlatform(string platform) => m_Platform = platform;

    protected override void Construct()
    {
        if (Localisation.Instance == null)
        {
            Localisation.CreateInstance();
            Localisation.Instance.LoadLanguage(Localisation.Language.English);
        }
    }

    public void LoadFromFile(Action<string> onLoad = null, Action<int> onCount = null, Action OnLoaded = null)
    {
        Localisation.Instance.LoadLanguage(Localisation.Language.English);

        // set the json lib for future reads
        using (new JsonLibraryScope(JsonLib))
        {
            // load catalogue description
            Description = Json.Library.ReadFromFile<CatalogueDescription>(DescriptionPath);
            Presets = LoadFromIndex<CataloguePreset>(PresetsIndexPath, GetPresetPath, "Presets", onLoad, l =>
            {
                onCount?.Invoke(l);
            });
            Entries = LoadFromIndex<CatalogueEntry>(EntriesIndexPath, GetEntryPath, "Entries", onLoad, l =>
            {
                onCount?.Invoke(Presets.Count + l * 2);
            });

            Entries.RemoveAll(e => e == null);
            Presets.RemoveAll(e => e == null);

            foreach (var entry in Entries)
            {
                onLoad?.Invoke($"EditorMetaData: [{entry.Guid}]");

                var renderMetaData = entry.GetMetaData<RenderMetaData>();
                if (renderMetaData != null)
                {
                    string path = GetMetaPath<RenderMetaData>(entry.Guid);
                    var editorData = Json.Library.ReadFromFile<EditorRenderData>(path);
                    if (editorData != null)
                    {
                        editorData.Apply(renderMetaData, entry);
                    }
                    else
                    {
                        Debug.LogWarning($"Could not find EditorRenderMetaData for [{entry.Guid}] - {entry.FullName}\n");
                    }
                }
            }
        }

        OnLoaded?.Invoke();
        OnPostLoad();
    }

    private List<T> LoadFromIndex<T>(string indexPath, Func<string, string> getPath, string label, Action<string> onLoad, Action<int> onCount) where T : class
    {
        // load the index and each entry
        var index = Json.Library.ReadFromFile<List<string>>(indexPath);
        onCount?.Invoke(index.Count);

        var items = new List<T>(index.Count);
        foreach (var id in index)
        {
            onLoad?.Invoke($"{label}: [{id}]");
            string path = getPath(id);
            if (Utils.IO.FileExists($"{path}.json") == true)
            {
                items.Add(Json.Library.ReadFromFile<T>(path));
            }
            else
            {
                Debug.LogWarning($"Failed to load Catalogue Entry for {id}\nJson file could not be found!");
            }
        }
        return items;
    }

    public void SaveToFile(bool onlyLastAdded, List<CatalogueEntry> EntriesRef, List<CataloguePreset> PresetsRef, bool showErrors, Action<string> onSave)
    {
        OnPreSave();

        Dictionary<string, List<GlobalConsts.CatalogueData>> GUIDPerEnvironment = new Dictionary<string, List<GlobalConsts.CatalogueData>>();

        // set the json lib for future writes
        using (new JsonLibraryScope(JsonLib))
        {

            // save index files for preset and entries

            IndexWriter.Write(EntriesRef, EntriesIndexPath, e => e.Guid, e => e.FullName.Text);
            IndexWriter.Write(PresetsRef, PresetsIndexPath, e => e.Guid, e => e.FullName.Text);

            Json.Library.WriteToFile(Description, DescriptionPath, true);

            List<CatalogueEntry> EntriesRefCopy = new List<CatalogueEntry>();
            List<CataloguePreset> PresetsRefCopy = new List<CataloguePreset>();

            if (onlyLastAdded == true)
            {
                EntriesRefCopy.Add(EntriesRef.Last());
                PresetsRefCopy.Add(PresetsRef.Last());
            }
            else
            {
                foreach (var entry in EntriesRef)
                {
                    EntriesRefCopy.Add(entry);
                }
                foreach (var entry in PresetsRef)
                {
                    PresetsRefCopy.Add(entry);
                }
            }

            bool foundAllFiles = true;
            // set SizeOfFileMB data
            foreach (var entry in EntriesRefCopy)
            {
                var metaData = entry.GetMetaData<RenderMetaData>();

                string fileName = metaData.Main.Bundle;
                fileName = $"{Application.streamingAssetsPath}/Bundles/" + m_Platform + "/" + fileName;
                if (File.Exists(fileName) == true)
                {
                    System.IO.FileInfo info = new System.IO.FileInfo(fileName);
                    if (metaData.SizeOfFileMB.Length != GlobalConsts.PlatformNamesLength)
                    {
                        Array.Resize(ref metaData.SizeOfFileMB, GlobalConsts.PlatformNamesLength);
                    }
                    float newSize = (info.Length / 1024f);
                    newSize /= 1024f;

                    float dif = Mathf.Abs(metaData.SizeOfFileMB[GlobalConsts.GetPlatformIndex(m_Platform)] - newSize);

                    if (metaData.SizeOfFileMB[GlobalConsts.GetPlatformIndex(m_Platform)] == 0 || dif > DIFFERNCE)
                    {
                        metaData.SizeOfFileMB[GlobalConsts.GetPlatformIndex(m_Platform)] = newSize;
                    }
                }
                else
                {
                    if (showErrors == true)
                    {
                        Debug.LogError($"Cannot find file {fileName}     {entry.Guid}");
                        foundAllFiles = false;
                    }
                }

                try
                {
                    if (metaData.Main.Bundle.CaseInsensitiveContains("assets/artwork/models/catalogue") == true)
                    {
                        if (showErrors == true)
                        {
                            Debug.LogError($"Have to redo the item {entry.Guid},   {entry.FullName} as the Guid is messed up. remove from catalogue");
                            foundAllFiles = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Something gone wrong , need to track down were it gone wrong");
                }

            }



            // save each entry as json
            foreach (var entry in EntriesRefCopy)
            {
                Json.Library.WriteToFile(entry, GetEntryPath(entry.Guid), true);
                onSave($"Entry: [{entry.Guid}]");
                foreach (string environment in entry.EnvironmentsAvailable)
                {
                    if (false == GUIDPerEnvironment.ContainsKey(environment))
                    {
                        GUIDPerEnvironment.Add(environment, new List<GlobalConsts.CatalogueData>());
                    }

                    GlobalConsts.CatalogueData newData = new GlobalConsts.CatalogueData();
                    newData.GUID = entry.Guid;
                    newData.ReferenceNumber = entry.ReferenceNumber;
                    //Store GUID of this product for this environment
                    GUIDPerEnvironment[environment].Add(newData);
                }
            }
            // save each preset as json
            foreach (var preset in PresetsRefCopy)
            {
                Json.Library.WriteToFile(preset, GetPresetPath(preset.Guid), true);
                onSave($"Preset: [{preset.Guid}]");
            }

            // save any editor meta data
            foreach (var entry in EntriesRefCopy)
            {
                var renderMetaData = entry.GetMetaData<RenderMetaData>();
                if (renderMetaData != null)
                {
                    var editorData = new EditorRenderData(renderMetaData);
                    string path = GetMetaPath<RenderMetaData>(entry.Guid);
                    Json.Library.WriteToFile(editorData, path, true);
                }
                onSave($"Editor MetaData: [{entry.Guid}]");
            }

            List<CatalogueEntry> catList = new List<CatalogueEntry>();
            foreach (var entry in EntriesRef)
            {
                catList.Add(entry);
            }
            string fileDestination = EntryCombinedPath;
            Json.Library.WriteToFile(catList, fileDestination, true);

            SaveCataloguePerEnvironment(GUIDPerEnvironment);
            Localisation.Instance.SaveLanguage(Localisation.Language.English);
            if (foundAllFiles == false)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("Error", "There is a problem, check console logs", "Ok");
#endif
            }

            List<string> longNames = new List<string>();
            foreach (var entry in EntriesRefCopy)
            {
                longNames.Add(entry.FullName.ToString());
            }
            Json.Library.WriteToFile(longNames, $"D:/Work/list", true);
        }
    }

    private void SaveCataloguePerEnvironment(Dictionary<string, List<GlobalConsts.CatalogueData>> catalogueMasks)
    {
        string rootFolder = $"{GlobalConsts.BUNDLE_STREAMING}/";

        foreach (KeyValuePair<string, List<GlobalConsts.CatalogueData>> kvp in catalogueMasks)
        {
            string folder = $"{rootFolder}{kvp.Key}/{GlobalConsts.DataFolder}";
            if (true == System.IO.Directory.Exists(folder))
            {
                //Folder exists create file
                Debug.Log($"writing data : {folder}/CatalogueData");
                Json.JsonNet.WriteToFile(kvp.Value, $"{folder}/CatalogueData", true);
            }
            else
            {
                //Most likely tried to store the name of a environment which is not in the environment catalogue and so cannot be loaded
                //Ensure the environment exists and is in the environment catalogue
                Debug.LogError("Could not data for: " + kvp.Value + " environment does not exist!!");
            }
        }
    }

    public void OnPostLoad()
    {
        foreach (var entry in Entries)
        {
            entry.OnPostLoad();
        }
        foreach (var preset in Presets)
        {
            preset.OnPostLoad();
        }
        Description.OnPostLoad();
    }

    public void OnPreSave()
    {
        Localisation.Instance.PurgeKeys(LocalisationKey);
        foreach (var entry in Entries)
        {
            entry.OnPreSave();
        }
        foreach (var preset in Presets)
        {
            preset.OnPreSave();
        }
        Description.OnPreSave();

        Localisation.Instance.VerifyData(Entries);
    }

    public static string GetExternalMetaDataPath(string guid, MetaDataType type)
    {
        return GetEntryPath(guid, type.ToString().ToLower());
    }
    public static string GetExternalMetaDataPath(string guid, ExternalMetaDataType type)
    {
        return GetEntryPath(guid, type.ToString().ToLower());
    }

    public static T LoadProfile<T>(string profileName) where T : class => Json.JsonNet.ReadFromFile<T>(GetProfilePath<T>(profileName));

    public static string GetProfileFolder<T>() => GetProfileFolder(typeof(T));
    public static string GetProfileFolder(Type type) => $"{ProfilesFolder}/{type.Name.ToLowerUnderscored()}";
    public static string GetProfilePath<T>(string name) => $"{GetProfileFolder<T>()}/{name}";

    public static string GetEntryPath(string guid, string suffix) => $"{GetEntryPath(guid)}_{suffix}";
    public static string GetEntryPath(string guid) => $"{EntriesFolder}/{guid}/{guid}";
    public static string GetPresetPath(string guid) => $"{PresetsFolder}/{guid}/{guid}";

    private string GetMetaPath<T>(string guid) where T : MetaData => $"{MetaFolder}/{typeof(T).Name.ToLower()}/{guid}";
}
