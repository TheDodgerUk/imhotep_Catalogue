using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace EditorTools.Environments
{
    public class EnvironmentVariantData
    {
        public string OverloadEnvironmentName()
        {
            if (UseOverloadData == true)
            {
                return Localisation.Instance.Get($"{EnvironmentData.EnvironmentKey}_{OverloadEnvironmentGuid}_name");
            }
            else
            {
                return Localisation.Instance.Get($"{EnvironmentData.EnvironmentKey}_{Environment.Guid}_name");
            }
        }

        public string OverloadVariantName()
        {
            if (UseOverloadData == true)
            {
                return Localisation.Instance.Get($"{EnvironmentData.VariantKey}_{OverloadVariantGuid}_name");
            }
            else
            {
                return Localisation.Instance.Get($"{EnvironmentData.VariantKey}_{Guid}_name");
            }
        }


        public bool OriginalFoldout = false;
        public bool GeneratedFoldout = false;
        public bool OptionalFoldout = false;

        public EnvironmentSceneData Environment { get; set; }
        public LocalisedValue Name { get; private set; }

        public string CombinedName => $"{Environment.Name.ToString()}\\{Name.ToString()}".SanitiseSlashes();
        public string OverloadCombinedName => $"{OverloadEnvironmentName().ToString()}\\{OverloadVariantName().ToString()}".SanitiseSlashes();

        public string Guid;
        public List<SceneAsset> OriginalScenes;
        public List<SceneAsset> GeneratedScenes;
        public List<SceneAsset> OptionalScenes;


        public int CatalogueDataVersion = 0;
        public int AudioClipVersion = 0;
        public int GameObjectVersion = 0;
        public int MaterialVersion = 0;
        public int Texture2DVersion = 0;
        public int VideoClipVersion = 0;
        public int TextVersion = 0;

        public GlobalConsts.InteractionType InteractionType = GlobalConsts.InteractionType.Physics;
        public bool UseOverloadData;
        public string OverloadEnvironmentGuid;
        public string OverloadVariantGuid;


        public bool IncludeInBuild;
        public int ExtraBundlesToInclude;
        public int ExtraBundlesFromServer;
        public bool IncludeCataloguePrefabs;
        public bool CataloguePrefabsFromServer;
        public bool CatalogueListFromServer;
        public bool IncludeGenericAssetBundles;
        public bool SingleCatalogueItemOnly;
        public bool IncludeInUnitTests;
        public List<string> AttributesAllowed;
        public EnvironmentData.GenericLightingSettings m_GenericLighting;
        public EnvironmentData.OptionalBaking m_OptionalBaking;
        public List<EnvironmentData.BakeQuality> m_BakeQuality;

        public EnvironmentVariantData()
        {
            Guid = Utils.Code.GenerateGuid();
            OriginalScenes = new List<SceneAsset>();
            GeneratedScenes = new List<SceneAsset>();
            OptionalScenes = new List<SceneAsset>();

            IncludeInBuild = true;
            ExtraBundlesToInclude = -1;
            ExtraBundlesFromServer = -1;
            IncludeCataloguePrefabs = true;
            CataloguePrefabsFromServer = true;
            CatalogueListFromServer = true;
            IncludeGenericAssetBundles = true;
            IncludeInUnitTests = true;
            AttributesAllowed = new List<string>();
            m_GenericLighting = new EnvironmentData.GenericLightingSettings();
            m_OptionalBaking = new EnvironmentData.OptionalBaking();
            m_BakeQuality =  new List<EnvironmentData.BakeQuality>();
            OnPostLoad();
        }

        public void OnPostLoad() => Name = new LocalisedValue(EnvironmentData.VariantKey, Guid, "name");

        public void SetScenes(List<SceneAsset> scenes)
        {
            OriginalScenes = scenes;
            OnScenesAdded();
        }

        public void AddScenes(List<SceneAsset> scenes)
        {
            OriginalScenes.AddRange(scenes);
            OnScenesAdded();
        }

        private void OnScenesAdded()
        {
            OriginalScenes = OriginalScenes.DistinctList();
            OriginalScenes.Sort(Comparisons.NaturalByName);
        }



        // Optional
        public void SetOptionalScenes(List<SceneAsset> scenes)
        {
            OptionalScenes = scenes;
            OnOptionalScenesAdded();
        }

        public void AddOptionalScenes(List<SceneAsset> scenes)
        {
            OptionalScenes.AddRange(scenes);
            OnOptionalScenesAdded();
        }

        private void OnOptionalScenesAdded()
        {
            OptionalScenes = OptionalScenes.DistinctList();
            OptionalScenes.Sort(Comparisons.NaturalByName);
        }


    }
}
