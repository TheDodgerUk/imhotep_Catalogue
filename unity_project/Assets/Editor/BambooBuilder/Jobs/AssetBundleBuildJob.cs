using EditorTools.Bundles;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BuildSystem
{
    public class BuildAssetBundleJob
    {
        public static void ArtTestEmpty()
        {
            var repeatingLog = new RepeatingLogger();
            repeatingLog.Start("Art test placeholder");
            repeatingLog.Stop();
        }

        public static void BuildAssetBundles()
        {
            // parse command line args
            var args = new AssetBundleParameters();

            // build bundles
            Debug.Log("Starting Asset Bundle Build");
            var repeatingLog = new RepeatingLogger();
            repeatingLog.Start("Building Asset Bundles");

            var buildTarget = EditorUserBuildSettings.activeBuildTarget;
            List<string> dependencies = new List<string>();
            AssetBundleBuilder.BuildBundles(buildTarget, args.CleanBuild, onlyManifest:false);
            repeatingLog.Stop();

            if (string.IsNullOrEmpty(args.TargetFolder) == true)
            {
                Debug.Log("Target folder path was empty. Will not copy artifacts");
                return;
            }

            if (Utils.IO.ForceFolder(args.TargetFolder) == false)
            {
                Debug.Log($"Failed to create folder at {args.TargetFolder}");
                return;
            }

            // copy bundles + data to destination folders
            AssetBundleBuilder.CopyToDestination(args.BundlePath, args.CleanBuild);
            Debug.Log($"Copied Asset Bundles to {args.BundlePath}");

            Utils.Editor.CopyFiles(Catalogue.DataFolder, args.DataPath, args.CleanBuild, "json");
            Debug.Log($"Copied Data to {args.DataPath}");
        }

        private class AssetBundleParameters : CommandLineParameters
        {
            public readonly string BundlePath = string.Empty;
            public readonly string DataPath = string.Empty;

            public string TargetFolder = string.Empty;
            public bool CleanBuild = false;

            public AssetBundleParameters() : base()
            {
                var m_Platform = EditorUserBuildSettings.activeBuildTarget;
                BundlePath = $"{TargetFolder}{GlobalConsts.BundleFolder}{GlobalConsts.CurrentPlatformName()}";
                DataPath = $"{TargetFolder}{GlobalConsts.DataFolder}";
            }

            protected override void ParseArgument(string argument, string[] args, int i)
            {
                switch (argument)
                {
                    case "targetFolder":
                        TargetFolder = args[i + 1] + @"\";
                        break;
                    case "cleanBuild":
                        CleanBuild = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
