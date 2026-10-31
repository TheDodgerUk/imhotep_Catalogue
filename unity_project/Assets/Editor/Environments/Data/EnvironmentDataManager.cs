using FilePathHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Environments
{
    public class EnvironmentDataManager
    {
        public static string ENVIRONMENT_VARIANT_INDEX = "EnvironmentVariantIndex";

        private static string OPIONAL_EXTENTION = "_Optional";
        public static FilePath MetaRoot => FilePath.FromStreaming($"{GlobalConsts.MetaFolder}/environments");
        public static FilePath DataRoot => FilePath.FromStreaming($"{GlobalConsts.BUNDLE_STREAMING_ENVIROMENT_METADATA}");

        public static string rootFolder = $"{GlobalConsts.BUNDLE_STREAMING}/";

        public List<EnvironmentSceneData> Environments = new List<EnvironmentSceneData>();

        public EnvironmentDataManager()
        {
            if (Catalogue.Instance == null)
            {
                Catalogue.CreateInstance();
                using (var progress = new ProgressBar("Loading Catalogue For All"))
                {
                    Catalogue.Instance.LoadFromFile(str => progress.UpdateIncrement(str), c => progress.SetMaxIncrement(c), null);
                }
            }

            if (Localisation.Instance == null)
            {
                Localisation.CreateInstance();
                Localisation.Instance.LoadLanguage(Localisation.Language.English);
            }
        }

        public void SetVariant(EnvironmentVariantData data)
        {
            int total = 0;
            for (int i = 0; i < Environments.Count; i++)
            {
                EnvironmentSceneData environment = Environments[i];
                for (int x = 0; x < environment.Variants.Count; x++)
                {
                    EnvironmentVariantData variantData = environment.Variants[x];
                    bool variantName = (data.Guid == variantData.Guid);
                    bool environmentName = (data.Environment.Guid == variantData.Environment.Guid);
                    if (variantName && environmentName)
                    {
                        Debug.Log($"ENVIRONMENT_VARIANT_INDEX set to {total}");
                        PlayerPrefs.SetInt(ENVIRONMENT_VARIANT_INDEX, total);
                        GetCurrentVariant();
                        return;
                    }
                    total++;
                }
            }
            Debug.LogError("SetVariant is invalid");
        }

        public EnvironmentVariantData GetCurrentVariant()
        {
            int lookFor = PlayerPrefs.GetInt(ENVIRONMENT_VARIANT_INDEX);
            int total = 0;
            for (int i = 0; i < Environments.Count; i++)
            {
                EnvironmentSceneData environment = Environments[i];
                for (int x = 0; x < environment.Variants.Count; x++)
                {
                    EnvironmentVariantData variant = environment.Variants[x];
                    if (lookFor == total)
                    {
                        PlayerPrefs.SetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME, $"{EnvironmentDataManager.rootFolder}{variant.Environment.Name }/{ variant.Name}");
                        PlayerPrefs.SetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME_SHORT, $"{variant.Environment.Name }/{ variant.Name}");

                        PlayerPrefs.SetString(GlobalConsts.ENVIRONMENT_OVERLOADED_COMBINED_NAME_SHORT, $"{variant.OverloadCombinedName}");

                        Debug.LogError($"PlayerPrefs  ENVIRONMENT_COMBINED_NAME_SHORT   {variant.Environment.Name }/{ variant.Name}");
                        if(string.IsNullOrEmpty(variant.Environment.Name.ToString()) || string.IsNullOrEmpty(variant.Name.ToString()))
                        {
                            EditorUtility.DisplayDialog("Error", "Both Environment name and variant name need to be valid ", "ok");
                        }
                        return variant;
                    }
                    total++;
                }
            }
            return null;
        }


        public string GetVarientFullPath()
        {
            var variant = GetCurrentVariant();
            return $"{EnvironmentDataManager.rootFolder}{variant.Environment.Name }/{ variant.Name}";
        }

        public void LoadFromFile()
        {
            using (new JsonLibraryScope(JsonLibraryType.JsonNet))
            {
                // does environment data exist?
                var indexPath = DataRoot.Append("index");
                if (Utils.IO.FileExists(indexPath.AppendExt(".json").Absolute) == false)
                {
                    Debug.LogWarning($"Could not find Environment Index file. File not found at \"{indexPath.Absolute}\"\n");
                    return;
                }

                // load index file 
                var index = Json.Library.ReadFromFile<string[]>(indexPath.Absolute);

                var dict = new Dictionary<string, EnvironmentSceneData>();
                Environments = new List<EnvironmentSceneData>();

                foreach(var guid in index)
                {
                    string metaPath = MetaRoot.Append(guid).Absolute;
                    string dataPath = DataRoot.Append(guid).Absolute;
                    
                    var metaData = Json.Library.ReadFromFile<List<string>>(metaPath);
                    var metaOptionalData = Json.Library.ReadFromFile<List<string>>(metaPath + OPIONAL_EXTENTION);
                    var data = Json.Library.ReadFromFile<EnvironmentData>(dataPath);

                    // add variant to environment key
                    if (dict.ContainsKey(data.EnvironmentGuid) == false)
                    {
                        // create list of variants for dict and variant to use
                        var sceneData = new EnvironmentSceneData
                        {
                            Variants = new List<EnvironmentVariantData>(),
                            Guid = data.EnvironmentGuid,
                        };

                        // set name
                        sceneData.OnPostLoad();

                        // add to dict + list
                        dict.Add(data.EnvironmentGuid, sceneData);
                        Environments.Add(sceneData);
                    }

                    // create variant from this data
                    var variantData = new EnvironmentVariantData
                    {
                        Guid = data.VariantGuid,
                        OriginalScenes = metaData.Extract(EnvironmentSceneManager.FindScene),
                        GeneratedScenes = EnvironmentSceneManager.LoadScenes(data),
                        OptionalScenes = new List<SceneAsset>(),
                        CatalogueDataVersion = data.CatalogueDataVersion,
                        AudioClipVersion = data.AudioClipVersion,
                        GameObjectVersion = data.GameObjectVersion,
                        MaterialVersion = data.MaterialVersion,
                        Texture2DVersion = data.Texture2DVersion,
                        TextVersion = data.TextVersion,
                        VideoClipVersion = data.VideoClipVersion,
                        UseOverloadData = data.UseOverloadData,
                        OverloadEnvironmentGuid = data.OverloadEnvironmentGuid,
                        OverloadVariantGuid = data.OverloadVariantGuid,
                        InteractionType = data.InteractionType,
                        IncludeInBuild = data.IncludeInBuild,
                        IncludeInUnitTests = data.IncludeInUnitTests,
                        ExtraBundlesToInclude = data.ExtraBundlesToInclude,
                        ExtraBundlesFromServer = data.ExtraBundlesFromServer,
                        IncludeCataloguePrefabs = data.IncludeCataloguePrefabs,
                        CataloguePrefabsFromServer = data.CataloguePrefabsFromServer,
                        CatalogueListFromServer = data.CatalogueListFromServer,
                        IncludeGenericAssetBundles = data.IncludeGenericAssetBundles,
                        SingleCatalogueItemOnly = data.SingleCatalogueItemOnly,
                        AttributesAllowed = data.AttributesAllowed,
                        m_GenericLighting = data.m_GenericLighting,
                        m_OptionalBaking = data.m_OptionalBaking,
                        m_BakeQuality = data.m_BakeQuality
                    };

                    // wrapped this in a safety as it keeps breaking without it 

                    if (null == variantData.m_OptionalBaking)
                    {
                        variantData.m_OptionalBaking = new EnvironmentData.OptionalBaking();
                    }

                    if (null == variantData.m_BakeQuality)
                    {
                        variantData.m_BakeQuality = new List<EnvironmentData.BakeQuality>();
                    }

                    if (null != metaOptionalData)
                    {
                        variantData.OptionalScenes = metaOptionalData.Extract(EnvironmentSceneManager.FindScene);
                    }

                    variantData.OnPostLoad();
                    variantData.Environment = dict[data.EnvironmentGuid];
                    dict[data.EnvironmentGuid].Variants.Add(variantData);
                }
            }
        }

        public void SaveToFile()
        {
            OnPreSave();

            using (new JsonLibraryScope(JsonLibraryType.JsonNet))
            {
                // get data in correct format for saving
                var variants = Environments.Extract(e => e.Variants.Extract(v => v.OriginalScenes)).Flatten();
                var metaData = variants.Extract(v => v.Extract(s => s.GetAssetGuid()));

                var OptinalVariants = Environments.Extract(e => e.Variants.Extract(v => v.OptionalScenes)).Flatten();
                var OptinalMetaData = OptinalVariants.Extract(v => v.Extract(s => s.GetAssetGuid()));

                var environmentData = Environments.Extract(e =>
                {
                    return e.Variants.Extract(v => new EnvironmentData
                    {
                        Chunks = EnvironmentSceneGenerator.GetGeneratedSceneNames(v),
                        OptionalChunks = EnvironmentSceneGenerator.GetOptionalGeneratedSceneNames(v),
                        EnvironmentGuid = e.Guid,
                        VariantGuid = v.Guid,

                        CatalogueDataVersion = v.CatalogueDataVersion,
                        AudioClipVersion = v.AudioClipVersion,
                        GameObjectVersion = v.GameObjectVersion,
                        MaterialVersion = v.MaterialVersion,
                        Texture2DVersion = v.Texture2DVersion,
                        TextVersion = v.TextVersion,
                        VideoClipVersion = v.VideoClipVersion,
                        InteractionType = v.InteractionType,
                        UseOverloadData = v.UseOverloadData,
                        OverloadEnvironmentGuid = v.OverloadEnvironmentGuid,
                        OverloadVariantGuid = v.OverloadVariantGuid,

                        IncludeInBuild = v.IncludeInBuild,
                        IncludeInUnitTests = v.IncludeInUnitTests,
                        IncludeCataloguePrefabs = v.IncludeCataloguePrefabs,
                        CatalogueListFromServer = v.CatalogueListFromServer,
                        CataloguePrefabsFromServer = v.CataloguePrefabsFromServer,
                        IncludeGenericAssetBundles = v.IncludeGenericAssetBundles,
                        SingleCatalogueItemOnly = v.SingleCatalogueItemOnly,
                        ExtraBundlesFromServer = v.ExtraBundlesFromServer,
                        ExtraBundlesToInclude = v.ExtraBundlesToInclude,
                        AttributesAllowed = v.AttributesAllowed,
                        m_GenericLighting = v.m_GenericLighting,
                        m_OptionalBaking = v.m_OptionalBaking,
                        m_BakeQuality = v.m_BakeQuality

                    });
                }).Flatten();

                Debug.Log("Creating index files");
                // save index file
                string indexPath = DataRoot.Append("index").Absolute;
                var index = environmentData.Extract(e => string.Concat(e.EnvironmentGuid, "/", e.VariantGuid));
                IndexWriter.Write(environmentData, indexPath, e =>
                {
                    return string.Concat(e.EnvironmentGuid, "/", e.VariantGuid);
                }, e =>
                {
                    return string.Concat(e.EnvironmentName, " - ", e.VariantName);
                });

                CreateSeperateJsonFile(environmentData);
                //Json.Library.WriteToFile(index, indexPath, true);

                // save individual environment files
                for (int i = 0; i < environmentData.Count; ++i)
                {
                    Json.Library.WriteToFile(metaData[i], MetaRoot.Append(index[i]).Absolute, true);
                    Json.Library.WriteToFile(OptinalMetaData[i], MetaRoot.Append(index[i]).Absolute + OPIONAL_EXTENTION, true);
                    Json.Library.WriteToFile(environmentData[i], DataRoot.Append(index[i]).Absolute, true);
                }

                Localisation.Instance.SaveLanguage(Localisation.Language.English);

                string environmentDataStructure = EnvironmentDataManager.rootFolder;
                GlobalConsts.EnvironmentDataStructure folderStructure = new GlobalConsts.EnvironmentDataStructure();
                for (int i = 0; i < Environments.Count; i++)
                {
                    EnvironmentSceneData environment = Environments[i];
                    for (int x = 0; x < environment.Variants.Count; x++)
                    {
                        EnvironmentVariantData variantData = environment.Variants[x];

                        var data = new GlobalConsts.InternalEnvironmentDataStructure();
                        data.Path = $"{environment.Name.Text}/{variantData.Name.Text}";
                        folderStructure.EnvironmentPath.Add(data);
                        SaveAttributeData(data.Path, variantData);
                    }
                }

                Json.JsonNet.WriteToFile(folderStructure, GlobalConsts.BUNDLE_STREAMING_STRUCTURE_FILE, true);

            }
        }

        private static void CreateSeperateJsonFile(List<EnvironmentData> environmentData)
        {
            //linkage to BundleBuilderWindow.cs  CopyFilesVarientIndex
            foreach (var single in environmentData)
            {
                List<EnvironmentData> singleList = new List<EnvironmentData>();
                singleList.Add(single);
                string singlePath = DataRoot.Append("_" + single.VariantGuid).Absolute;
                IndexWriter.Write<EnvironmentData>(singleList, singlePath, e =>
                {
                    return string.Concat(e.EnvironmentGuid, "/", e.VariantGuid);
                }, e =>
                {
                    return string.Concat(e.EnvironmentName, " - ", e.VariantName);
                });
            }
        }


        private void SaveAttributeData(string folderName, EnvironmentVariantData variantData)
        {
            string variantPath = $"{rootFolder}{folderName}/{GlobalConsts.DataFolder}";
            Json.JsonNet.WriteToFile(variantData.AttributesAllowed, variantPath + "/AttributeData", true);
        }

        private void OnPreSave()
        {
            Localisation.Instance.PurgeKeys(EnvironmentData.EnvironmentKey);
            Localisation.Instance.PurgeKeys(EnvironmentData.VariantKey);

            Environments.ForEach(s => s.Variants.ForEach(v => v.Name.Flush()));
            Environments.ForEach(s => s.Name.Flush());
        }
    }
}
