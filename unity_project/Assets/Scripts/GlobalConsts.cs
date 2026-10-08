using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System;

public class GlobalConsts
{
    public static readonly List<Type> DO_NOT_REMOVE = new List<Type>()
    {
        typeof(VLB.BeamGeometry),
        typeof(VLB.VolumetricLightBeam),
        typeof(VLB.VolumetricDustParticles),
        typeof(Autohand.GrabbablePoseAdvanced),
        typeof(Autohand.GrabbablePose),
        typeof(Autohand.HandPoseScriptable),
        typeof(CollisionSound),
        //typeof(LiquidWobble),
        //typeof(UnityEngine.VFX.VisualEffect),
        //typeof(UnityEngine.VFX.VFXExposedProperty)
    };

    public static readonly List<Type> REMOVE = new List<Type>()
    {
        //typeof(VLB.BeamGeometry),
        // typeof(VLB.VolumetricLightBeam),
        //typeof(VLB.VolumetricDustParticles),
        //typeof(LiquidWobble),
#if UNITY_ANDROID
        typeof(UnityEngine.VFX.VisualEffect),
#endif
        //typeof(UnityEngine.VFX.VFXExposedProperty)
    };


#if CATALOG_PROGRAM
    public const string ASSET_ARTWORK = "ArtWork\\ArtWorkGroup";
    public const string ASSET_BUNDLES = "AssetBundles";
    public const string ASSET_BUNDLES_STREAMING = "BundleStreamingAssets";


    public static string ENVIRONMENT_COMBINED_NAME = "EnvironmentCombinedName";
    public static string ENVIRONMENT_COMBINED_NAME_SHORT = "EnvironmentCombinedNameShort";
    public static string ENVIRONMENT_OVERLOADED_COMBINED_NAME_SHORT = "EnvironmentOverloadedCombinedNameShort";
    public static string ENVIRONMENT_INDEX = "EnvironmentIndex";
    public static string ENVIRONMENT_VARIANT_INDEX = "EnvironmentVariantIndex";
    public static string _PREFAB_EXTENTION = "_Prefab";
    public static string _PREFAB_PREFAB_EXTENTION = $"{GlobalConsts._PREFAB_EXTENTION}.prefab";

    public static readonly List<string> ArtFolders = new List<string>()
        {
            {"Animations"},
            {"Audio"},
            {"Miscellaneous"},
            {"Models"},
            {"Scene"},
            {"UI"},
        };
#endif


    public const string CollisionObj = "PlacementCollision";
    public const string RootModel = "RootModel";
    public const string BasicPhysicsCollider = "BasicPhysicsCollider";

    public const string CenterOfMass = "CenterOfMass_RigidBody";


    public static readonly string RootCatalogueItemTag = "RootCatalogueItem";
    // Calalogue program and main program need to match, closley 
    public static string BundleFolder = $"bundles/";
    public const string DataFolder = "Data";
    public const string MetaFolder = "metadata";
    public static string BUNDLE_STREAMING_SINGLE => $"BundleStreamingAssets";

    public static string BUNDLE_STREAMING_ENVIROMENT_METADATA => $"{BUNDLE_STREAMING_SINGLE}/EnvironmentData";

    public const string VIDEOCLIP_STREAMING = "VideoClipStreamingAsset";
    public static string BUNDLE_STREAMING => $"{Device.StreamingPath}/{BUNDLE_STREAMING_SINGLE}";
    public static string BUNDLE_STREAMING_LANGUAGES => $"{BUNDLE_STREAMING}/LanguageData/";
    public static string BUNDLE_STREAMING_BRANDS => $"{BUNDLE_STREAMING}/BrandData/";
    public static string BUNDLE_STREAMING_ENVIRONMENT_DATA => $"{BUNDLE_STREAMING}/EnvironmentData";
    public static string BUNDLE_STREAMING_STRUCTURE_FILE => $"{BUNDLE_STREAMING_ENVIRONMENT_DATA}/Structure";

    public static string SERVER_DEPOT_ASSETS_EXPLORER = @"http://ec2-3-8-209-49.eu-west-2.compute.amazonaws.com/builder/v1/";

    //#if def VR_INTERACTION  // wrapped around items to only load VR_INTERACTION, good for non interactions like builder

    public const string GENERIC_MATERIAL = "GenericMaterial_";

    public static readonly List<string> ExternalSourceAssetBundles = new List<string>()
    {
            { "cataloguethumbnail" },
            { "logos" },
            { "hologram" },
            { "weightdistribution" },
            { "cataloguethumbnailgroup" },
    };

    public static List<string> GetExternalSourceAssetBundlesInMask(int mask)
    {
        //https://answers.unity.com/questions/319940/maskfield-selected-values.html
        List<string> newList = new List<string>();
        for (int i = 0; i < ExternalSourceAssetBundles.Count; i++)
        {
            int layer = 1 << i;
            if ((mask & layer) != 0)
            {
                newList.Add(ExternalSourceAssetBundles[i]);
            }
        }
        return newList;
    }

    public enum InteractionType
    {
        Physics,
        ControllerVR,
        Placement,
    }


    public enum ExtraFromServer
    {
        AudioClipAssetBundle,
        GameObjectAssetBundle,
        MaterialAssetBundle,
        Texture2DAssetBundle,
        VideoClipAssetBundle,
        TextAssetBundle,
        CatalogueThumbnail,
        CatalogueThumbnailGroup
    }

    public static List<ExtraFromServer> GetExtraFromServerInMask(int mask)
    {
        //https://answers.unity.com/questions/319940/maskfield-selected-values.html
        List<ExtraFromServer> newList = new List<ExtraFromServer>();
        for (int i = 0; i < ExternalSourceAssetBundles.Count; i++)
        {
            int layer = 1 << i;
            if ((mask & layer) != 0)
            {
                newList.Add((ExtraFromServer)i);
            }
        }
        return newList;
    }


    public class InternalEnvironmentDataStructure
    {
        [SerializeField]
        public string Path;
    }
    public class EnvironmentDataStructure
    {
        [SerializeField]
        public List<InternalEnvironmentDataStructure> EnvironmentPath = new List<InternalEnvironmentDataStructure>();
    }

    public static readonly Dictionary<string, string[]> ExtraAssetBundles = new Dictionary<string, string[]>()
        {
            {ExtraFromServer.AudioClipAssetBundle.ToString(), new string[] { ".wav", ".mp3" } },
            {ExtraFromServer.MaterialAssetBundle.ToString(), new string[] { ".mat"} },
            {ExtraFromServer.Texture2DAssetBundle.ToString(), new string[] { ".png", ".jpg" } },
            {ExtraFromServer.GameObjectAssetBundle.ToString(), new string[] { ".prefab" } },
        };

    public static readonly Dictionary<string, string[]> ExtraAssetBundlesNonCompressed = new Dictionary<string, string[]>()
        {
            {ExtraFromServer.VideoClipAssetBundle.ToString(), new string[] { ".mp4", ".mov" } },
        };

    public static readonly List<string> ExtraStreamingAssets = new List<string>()
        {
            {"Data" },
            {"VideoClipStreamingAsset"},
        };

    public static readonly Dictionary<string, string[]> ExtraAssetBundlesSubDirectorys = new Dictionary<string, string[]>()
        {
            {ExtraFromServer.TextAssetBundle.ToString(), new string[] { ".json", ".dat", ".xml" } },
        };


    public static string GetChecksum(string file)
    {
        using (FileStream stream = File.OpenRead(file))
        {
            SHA256Managed sha = new SHA256Managed();
            byte[] checksum = sha.ComputeHash(stream);
            return System.BitConverter.ToString(checksum).Replace("-", String.Empty);
        }
    }

    public static string GetChecksum(byte[] bytes)
    {
        SHA256Managed sha = new SHA256Managed();
        byte[] checksum = sha.ComputeHash(bytes);
        return System.BitConverter.ToString(checksum).Replace("-", String.Empty);
    }

#if UNITY_EDITOR
    private static readonly Dictionary<BuildTarget, string> platformNames = new Dictionary<BuildTarget, string>()
        {
            { BuildTarget.StandaloneWindows64, "Windows" },
            { BuildTarget.Android, "Android" },
            { BuildTarget.StandaloneOSX, "OSX" },
            { BuildTarget.iOS, "iOS" },
            { BuildTarget.WebGL, "WebGL" },
        };


    public static int PlatformNamesLength => platformNames.Count;
    public static string PlatformName(BuildTarget target) => platformNames[target];
    public static string CurrentPlatformName() => platformNames[EditorUserBuildSettings.activeBuildTarget];
    public static int CurrentPlatformIndex() => GetPlatformIndex(EditorUserBuildSettings.activeBuildTarget);

    public static int GetPlatformIndex(BuildTarget platform)
    {
        switch (platform)
        {
            case BuildTarget.StandaloneWindows64: return 0; break;
            case BuildTarget.Android: return 1; break;
            case BuildTarget.StandaloneOSX: return 2; break;
            case BuildTarget.iOS: return 3; break;
            case BuildTarget.WebGL: return 4; break;
            default:
                break;
        }
        return 0;
    }


#else
    public static int PlatformNamesLength => 3;
#endif


    public static int GetPlatformIndex(string platform)
    {
        switch (platform)
        {
            case "Windows": return 0; break;
            case "Android": return 1; break;
            case "OSX": return 2; break;
            case "iOS": return 3; break;
            case "WebGL": return 4; break;
            default:
                Debug.LogError($"platform not found {platform}");
                break;
        }
        return 0;
    }

    public class AssetBundleVersion
    {
        public int Version = 0;
        public float FileSizeMB;
    }

    [System.Serializable]
    public class VFXReplaceInScene
    {
        [SerializeField]
        public string VfxName;
        [SerializeField]
        public string HierarchyPath;
        [SerializeField]
        public string HierarchyPathRoot;
        [SerializeField]
        public string Scene;
    }


    [System.Serializable]
    public class CatalogueReplaceInScene
    {
        [NonSerialized]
        public GameObject GameObjectRef;

        [SerializeField]
        public string RealGUID;
        [SerializeField]
        public string Filepath;
        [SerializeField]
        public string HierarchyPath;
        [SerializeField]
        public string HierarchyPathRoot;
        [SerializeField]
        public string CatalogueGUID;
        [SerializeField]
        public string Scene;

        [SerializeField]
        public Vector3 Position;
        [SerializeField]
        public Quaternion Rotation;
    }

    public class CatalogueData
    {
        public string GUID;
        public string ReferenceNumber;
    }

}
