using EditorTools.Environments;
using FilePathHelper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Bundles
{
    public class AssetBundleBuilder
    {
        public const string BundleNameStart = "Environment";
        public const string BundleExtension = "unity3d";
        public const string BundleSearchExtension = "*.unity3d";
        public static readonly List<BuildTarget> ValidPlatforms = new List<BuildTarget> { BuildTarget.StandaloneWindows64, BuildTarget.Android, BuildTarget.StandaloneOSX, BuildTarget.iOS, BuildTarget.WebGL };

        //public static readonly FilePath StagingFolder = FilePath.FromRelative("Assets/_bundles");
        public static FilePath StagingFolder(string platform)
        {
            return FilePath.FromRelative($"Assets/StreamingAssets/bundles/{platform}");//FilePath.FromRelative("Assets/_bundles");
        }

        public static bool BuildBundle(BuildTarget platform, string label)
        {
            string currentTarget = GlobalConsts.CurrentPlatformName();
            if (ValidPlatforms.Contains(EditorUserBuildSettings.activeBuildTarget) == true)
            {
                Utils.IO.ForceFolder(StagingFolder(currentTarget).Absolute);

                var options = GetOptions(true);
                var builds = new[]
                {
                    new AssetBundleBuild
                    {
                       assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(label),
                       assetBundleName = label,
                    },
                };
                var manifest = BuildPipeline.BuildAssetBundles(StagingFolder(currentTarget).Relative, builds, options, platform);
                //if (manifest != null)
                //{
                //    //manifest.de
                //    var map = AssetBundleDependencyMap.Load(StagingFolder(currentTarget));
                //    map.Add(manifest);
                //    map.Save(StagingFolder(currentTarget));
                //    AssetDatabase.Refresh();
                //    return true;
                //}
            }
            return false;
        }

        private static AssetBundleManifest m_Manifest;

        public static bool BuildBundles(BuildTarget platform, bool cleanBuild, bool onlyManifest)
        {
            bool returnValue = false;
            string currentTarget = GlobalConsts.PlatformName(platform);

            if (ValidPlatforms.Contains(platform) == true)
            {
                AssetBundleCreationValidation.CreateFileStructureForAssignBundles();
                AssetBundleCreationValidation.CheckAndAssignBundles();
                EditorUtility.DisplayProgressBar("Building Bundles", "", 0);

                if (cleanBuild == true)
                {
                    Utils.IO.CleanFolder(StagingFolder(currentTarget).Absolute);
                }
                Utils.IO.ForceFolder(StagingFolder(currentTarget).Absolute);

                var options = GetOptions(cleanBuild);

                if (onlyManifest == false)
                {
                    Debug.Log($"currentTarget  {currentTarget}    activeBuildTarget {EditorUserBuildSettings.activeBuildTarget}".ToUpper());
                    m_Manifest = BuildPipeline.BuildAssetBundles(StagingFolder(currentTarget).Relative, options, EditorUserBuildSettings.activeBuildTarget);
                }

                if (m_Manifest != null)
                {



                    var map = new AssetBundleDependencyMap();
                    map.Add(m_Manifest, Catalogue.Instance, currentTarget);
                    map.Save(StagingFolder(currentTarget));
                    AssetDatabase.Refresh();

                    foreach (var item in Catalogue.Instance.Entries)
                    {
                        var data = item.GetMetaData<RenderMetaData>();
                        var assignedPrefab = CatalogueViewer.PrefabManager.LoadPrefab(data.Main);
                        if (assignedPrefab == null)
                        {
                            Debug.LogError($"Error in catalogueItem '{item.FullName.ToString()}'  '{item.ShortName.ToString()}'   '{item.Guid}'   cant find the renderer");
                        }
                    }

                    returnValue = true;
                }
                else
                {
                    Debug.LogError("Manifest is null");
                }

                AllNonCompressed(currentTarget);
            }
            else
            {
                Debug.LogError("Unsupported Platform");
            }

            return returnValue;
        }


        private static void VarientNonCompressed(string currentTarget)
        {
            EnvironmentDataManager environmentData = new EnvironmentDataManager();
            environmentData.LoadFromFile();

            string environmentName = environmentData.GetCurrentVariant().Environment.Name.ToString();
            string variantDataName = environmentData.GetCurrentVariant().Name.ToString();
            int totalRealCount = NonCompressed(currentTarget, environmentName, variantDataName);
            Debug.Log($"VarientNonCompressed, actually {totalRealCount}");
        }

        private static void AllNonCompressed(string currentTarget)
        {
            EnvironmentDataManager environmentData = new EnvironmentDataManager();
            environmentData.LoadFromFile();
            List<EnvironmentSceneData> Environments = environmentData.Environments;
            int totalTriedCount = 0;
            int totalRealCount = 0;
            for (int i = 0; i < Environments.Count; i++)
            {
                var enviro = Environments[i];
                foreach (var varient in enviro.Variants)
                {
                    totalTriedCount++;
                    string environmenName = enviro.Name.Text;
                    string variantDataName = varient.Name.Text;
                    totalRealCount += NonCompressed(currentTarget, environmenName, variantDataName);
                }
            }
            Debug.Log($"NonCompressed tried to make {totalTriedCount}, actually {totalRealCount}");
        }

        private static int NonCompressed(string currentTarget, string environmenName, string variantDataName)
        {
            int count = 0;
            foreach (var key in GlobalConsts.ExtraAssetBundlesNonCompressed.Keys)
            {
                string environmentDataStructure = $"{Application.dataPath}/{key}";
                string structurePath = $"{environmentDataStructure}/{environmenName}/{variantDataName}";
                var singleCount = CreateOnlyAssetInFile(currentTarget, structurePath, BuildAssetBundleOptions.UncompressedAssetBundle);
                count += singleCount;
            }
            return count;
        }

        private static AssetBundleManifest CreateOnlyAssetsInBuild(string currentTarget, BuildAssetBundleOptions options)
        {
            Environments.EnvironmentDataManager environmentData = new Environments.EnvironmentDataManager();
            environmentData.LoadFromFile();
            var variant = environmentData.GetCurrentVariant();

            List<string> fullAsset = UnityEditor.AssetDatabase.GetAllAssetBundleNames().ToList();

            List<string> contains = new List<string>();


            string guid = variant.Guid;
            contains = fullAsset.FindAll(e => (e.SanitiseSlashes().CaseInsensitiveContains(variant.CombinedName) || e.SanitiseSlashes().CaseInsensitiveContains(guid) || e.SanitiseSlashes().CaseInsensitiveContains("Brand")));


            List<string> dependencies = new List<string>();
            List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
            foreach (var item in contains)
            {
                dependencies.AddRange(UnityEditor.AssetDatabase.GetAssetBundleDependencies(item, true).ToList());
            }


            contains.AddRange(dependencies);
            contains = contains.Distinct().ToList();

            foreach (var item in contains)
            {
                assetBundleBuilds.Add(CreateAssetBundleBuildFromAssetBundleName(item));
            }

            return BuildPipeline.BuildAssetBundles(StagingFolder(currentTarget).Relative, assetBundleBuilds.ToArray(), options, EditorUserBuildSettings.activeBuildTarget);
        }


        private static int CreateOnlyAssetInFile(string outPutTarget, string FileToDo, BuildAssetBundleOptions options)
        {
            List<string> fullAsset = UnityEditor.AssetDatabase.GetAllAssetBundleNames().ToList();
            List<string> contains = new List<string>();
            var cleanFileName = FileToDo.Replace(Application.dataPath, "");

            contains = fullAsset.FindAll(e => (e.CaseInsensitiveContains(cleanFileName)));

            List<AssetBundleBuild> assets = new List<AssetBundleBuild>();
            foreach (var item in contains)
            {
                var items = CreateAssetBundleBuildFromAssetBundleName(item);
                assets.Add(items);
            }
            if (assets.Count > 0)
            {
                BuildPipeline.BuildAssetBundles(StagingFolder(outPutTarget).Relative, assets.ToArray(), options, EditorUserBuildSettings.activeBuildTarget);
            }
            return assets.Count;
        }


        private static AssetBundleBuild CreateAssetBundleBuildFromAssetBundleName(string assetBundleName)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            string bundleName;
            string variant;
            GetAssetBundleNameAndVariantByReservedVariants(assetBundleName, out bundleName, out variant);

            build.assetBundleName = bundleName + ".unity3d";
            build.assetBundleVariant = "";// variant;
            build.assetNames = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            return build;
        }


        private static void GetAssetBundleNameAndVariantByReservedVariants(string originAssetBundleName, out string assetBundleName, out string assetBundleVariant)
        {
            assetBundleName = originAssetBundleName;
            assetBundleVariant = "";

            string extension = Path.GetExtension(assetBundleName);
            if (!string.IsNullOrEmpty(extension))
            {
                extension = extension.Substring(1);
                assetBundleName = Path.Combine(Path.GetDirectoryName(assetBundleName), Path.GetFileNameWithoutExtension(assetBundleName));
                assetBundleVariant = extension;
            }
        }

        public static Object GetBuiltBundle(string bundleLabel)
        {
            var path = StagingFolder(GlobalConsts.CurrentPlatformName()).Append(bundleLabel);
            return AssetDatabase.LoadAssetAtPath<Object>(path.Relative);
        }

        public static List<Object> GetUnusedBundles()
        {
            string rootPath = StagingFolder(GlobalConsts.CurrentPlatformName()).Absolute;
            var paths = Utils.Editor.GetFiles(rootPath, BundleExtension);
            var labels = AssetDatabase.GetAllAssetBundleNames().Hash();
            paths.RemoveAll(p => labels.Contains(p.Strip($"{rootPath}/").Absolute));
            return paths.Extract(p => AssetDatabase.LoadAssetAtPath<Object>(p.Relative));
        }

        public static void CopyToDestination(string destinationFolder, bool cleanCopy)
        {
            Utils.Editor.CopyFiles(StagingFolder(GlobalConsts.CurrentPlatformName()).Absolute, destinationFolder, cleanCopy, BundleExtension, "json");
        }

        public static bool IsAnyPlatformBuilt(BuildTarget target) => Utils.IO.FolderExists(StagingFolder(GlobalConsts.CurrentPlatformName()).Absolute);

        private static BuildAssetBundleOptions GetOptions(bool forceRebuild)
        {
            var options = BuildAssetBundleOptions.ChunkBasedCompression;
            if (forceRebuild == true)
            {
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
            }
            return options;
        }
    }
}
