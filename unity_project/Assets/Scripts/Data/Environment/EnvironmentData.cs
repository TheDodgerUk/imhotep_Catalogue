using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentData
{
    public const string VariantKey = "environment_variant";
    public const string EnvironmentKey = "environment";

    public string EnvironmentName => Localisation.Instance.Get($"{EnvironmentKey}_{EnvironmentGuid}_name");
    public string VariantName => Localisation.Instance.Get($"{VariantKey}_{VariantGuid}_name");

    public string OverloadEnvironmentName()
    {
        if(UseOverloadData == true)
        {
            return Localisation.Instance.Get($"{EnvironmentKey}_{OverloadEnvironmentGuid}_name");
        }
        else
        {
            return Localisation.Instance.Get($"{EnvironmentKey}_{EnvironmentGuid}_name");
        }
    }

    public string OverloadVariantName()
    {
        if (UseOverloadData == true)
        {
            return Localisation.Instance.Get($"{VariantKey}_{OverloadVariantGuid}_name");
        }
        else
        {
            return Localisation.Instance.Get($"{VariantKey}_{VariantGuid}_name");
        }
    }


    public string EnvironmentGuid;
    public string VariantGuid;
    public List<string> Chunks;
    public List<string> OptionalChunks;


    public GlobalConsts.InteractionType InteractionType;
    public bool UseOverloadData;
    public string OverloadEnvironmentGuid;
    public string OverloadVariantGuid;


    public int CatalogueDataVersion = 0;
    public int AudioClipVersion = 0;
    public int GameObjectVersion = 0;
    public int MaterialVersion = 0;
    public int Texture2DVersion = 0;
    public int VideoClipVersion = 0;
    public int TextVersion = 0;


    public bool IncludeInBuild;
    public bool IncludeInUnitTests;
    public bool IncludeCataloguePrefabs;
    public bool CataloguePrefabsFromServer;
    public bool CatalogueListFromServer;
    public bool IncludeGenericAssetBundles;
    public bool SingleCatalogueItemOnly;
    public int ExtraBundlesToInclude;
    public int ExtraBundlesFromServer;
    public GenericLightingSettings m_GenericLighting;
    public EnvironmentData.OptionalBaking m_OptionalBaking;
    public List<EnvironmentData.BakeQuality> m_BakeQuality;

    public List<string> ModulesAllowed = new List<string>();
    public List<string> AttributesAllowed = new List<string>();

    public class GenericLightingSettings
    {
        public string SkyBoxMaterial;
        public Color AmbientColour;
    }

    public class OptionalBaking
    {
        public List<bool> ToBeBaked = new List<bool>();
    }

    public class BakeQuality
    {
        public LightBakeQualityEnum m_QualityEnum = LightBakeQualityEnum.High;
        public int m_Index = 0;
        public bool m_IsBaked = true;
    }
    public enum LightBakeQualityEnum
    {
        High,
        Low,
    }

}
