using EditorTools.Environments;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Bundles
{
    public static class AssetBundleCreationValidation
    {
        public enum  FolderType
        {
            Bundle,
            Streaming,
            ArtWork,
        }


        public static void CreateFileStructureForAssignBundles()
        {
            foreach (var item in GlobalConsts.ExtraAssetBundles)
            {
                CreateFileStructureForAssetBundle(item.Key);
            }

            foreach (var item in GlobalConsts.ExtraAssetBundlesNonCompressed)
            {
                CreateFileStructureForAssetBundle(item.Key);
            }

            // same its just not asset bundles 
            foreach(var item in GlobalConsts.ExtraStreamingAssets)
            {
                CreateFileStructureForAssetBundle(item, FolderType.Streaming);
            }

            // same its just not asset bundles 
            foreach (var item in GlobalConsts.ArtFolders)
            {
                CreateFileStructureForAssetBundle(item, FolderType.ArtWork);
            }

            foreach (var item in GlobalConsts.ExtraAssetBundlesSubDirectorys)
            {
                CreateFileStructureForAssetBundle(item.Key);
            }
            
        }

        public static void CheckAndAssignBundles()
        {
            foreach(var item in GlobalConsts.ExtraAssetBundles)
            {
                ChangeFilesToAssetBundles(item.Key, item.Value);
            }

            foreach (var item in GlobalConsts.ExtraAssetBundlesNonCompressed)
            {
                ChangeFilesToAssetBundles(item.Key, item.Value);
            }

            foreach (var item in GlobalConsts.ExtraAssetBundlesSubDirectorys)
            {
                ChangeFilesToAssetBundlesSubDirectorys(item.Key, item.Value);
            }

            List<string> items = new List<string>();
            items.Add(".json");
            ChangeFilesToAssetBundlesSubDirectorys($"{Device.StreamingPath}/BundleStreamingAssets/CatalogueData/entries", "CatalogueJsonAsset.unity3d", items.ToArray());
        }


        private static void CreateFileStructureForAssetBundle(string folderName, FolderType folderType = FolderType.Bundle)
        {
            if (string.IsNullOrEmpty(folderName) == false)
            {
                List<string> fullPathBundleList = GenerateFullPathFromBase(folderName, folderType);

                for (int i = 0; i < fullPathBundleList.Count; i++)
                {
                    CreateEnvironmentCatalogueFolder(fullPathBundleList[i]);
                }
            }
            else
            {
                Debug.LogError("folderName is empty");
            }
        }



        private static List<string> GenerateFullPathFromBase(string folderName, FolderType folderType = FolderType.Bundle)
        {
            List<string> fullPathList = new List<string>();
            EnvironmentDataManager environmentData = new EnvironmentDataManager();
            environmentData.LoadFromFile();
            List<EnvironmentSceneData> Environments = environmentData.Environments;

            string environmentDataStructure = $"{ Application.dataPath}/{GlobalConsts.ASSET_BUNDLES}";
            switch (folderType)
            {
                case FolderType.Bundle:
                    break;
                case FolderType.Streaming:
                    environmentDataStructure = $"{Application.streamingAssetsPath}/{GlobalConsts.ASSET_BUNDLES_STREAMING}";
                    break;
                case FolderType.ArtWork:
                    environmentDataStructure = $"{Application.dataPath}/{GlobalConsts.ASSET_ARTWORK}";
                    fullPathList.Add($"{environmentDataStructure}Generic/{folderName}");
                    break;
                default:
                    break;
            }

            GlobalConsts.EnvironmentDataStructure folderStructure = new GlobalConsts.EnvironmentDataStructure();
            for (int i = 0; i < Environments.Count; i++)
            {
                EnvironmentSceneData environment = Environments[i];
                for (int x = 0; x < environment.Variants.Count; x++)
                {
                    EnvironmentVariantData variantData = environment.Variants[x];
                    string structurePath = $"{environmentDataStructure}/{environment.Name.Text}/{variantData.Name.Text}/{folderName}";
                    structurePath = structurePath.SanitiseSlashes();
                    fullPathList.Add(structurePath);
                }
            }
            if(folderType == FolderType.ArtWork)
            {
                string structurePath = $"{environmentDataStructure}/GENERIC/{folderName}";
                structurePath = structurePath.SanitiseSlashes();
                fullPathList.Add(structurePath);
            }
            return fullPathList;
        }


        private static void CreateEnvironmentCatalogueFolder(string structurePath)
        {
            string variantPath = $"{structurePath}";
            // not needed but if want to see what missing , this is what needed Debug.LogError($" CreateDirectory {variantPath}      file {variantPath + "/keep.json"}");
            variantPath = variantPath.SanitiseSlashes();
            System.IO.Directory.CreateDirectory(variantPath);
            System.IO.FileStream fs = System.IO.File.Create(variantPath + "/keep.json");
            fs.Close();
        }


        
        private static void ChangeFilesToAssetBundlesSubDirectorys(string basePathName, string[] extentions)
        {
            foreach (var item in extentions)
            {
                if (item[0] != '.')
                {
                    Debug.LogError("Extentions need to start with a fullStop");
                }
            }

            string fullBaseFileNames = $"{Application.dataPath}";

            List<string> fullPathList = GenerateFullPathFromBase(basePathName);
            for (int i = 0; i < fullPathList.Count; i++)
            {
                string[] fullFileNames = Directory.GetFiles(fullPathList[i], "*.*", SearchOption.AllDirectories);
                for (int fileNameIndex = 0; fileNameIndex < fullFileNames.Length; fileNameIndex++)
                {
                    string fullFileName = fullFileNames[fileNameIndex];
                    fullFileName = fullFileName.SanitiseSlashes();
                    string extention = Path.GetExtension(fullFileName);
                    string dd = extention;
                    //Debug.LogError($"{basePathName} {fullFileName}");
                    if (true == Array.Exists(extentions, x => x.Equals(extention)))
                    {
                        string shortNameFileName = "Assets" + fullFileNames[fileNameIndex].Replace(fullBaseFileNames, "");
                        AssetImporter asset = AssetImporter.GetAtPath(shortNameFileName);
                        if (null != asset)
                        {
                            string shortPathName = fullPathList[i].Replace(fullBaseFileNames, "");
                            shortPathName = shortPathName.Replace($"/{GlobalConsts.ASSET_BUNDLES}/", "/");
                            string fullPath = $"{AssetBundleBuilder.BundleNameStart}{shortPathName}.unity3d";
                            //Debug.LogError($" {shortNameFileName}   {fullPath}");
                            fullPath = fullPath.SanitiseSlashes();
                            asset.SetAssetBundleNameAndVariant(fullPath, "");
                            if (string.IsNullOrEmpty(fullPath) == true)
                            {
                                Debug.LogError("ERROR");
                            }
                        }
                    }
                }
            }
        }

        private static void ChangeFilesToAssetBundles(string basePathName, string[] extentions)
        {
            foreach(var item in extentions)
            {
                if(item[0] != '.')
                {
                    Debug.LogError("Extentions need to start with a fullStop");
                }
            }

            string fullBaseFileNames = $"{Application.dataPath}";

            List<string> fullPathList = GenerateFullPathFromBase(basePathName);
            for (int i = 0; i < fullPathList.Count; i++)
            {
                string[] fullFileNames = Directory.GetFiles(fullPathList[i]);
                for (int fileNameIndex = 0; fileNameIndex < fullFileNames.Length; fileNameIndex++)
                {
                    string fullFileName = fullFileNames[fileNameIndex];
                    string extention = Path.GetExtension(fullFileName);
                    string dd = extention;
                    //Debug.LogError($"{basePathName} {fullFileName}");
                    if (true == Array.Exists(extentions, x => x.Equals(extention)))
                    {
                        string shortNameFileName = "Assets" + fullFileNames[fileNameIndex].Replace(fullBaseFileNames, "");
                        AssetImporter asset = AssetImporter.GetAtPath(shortNameFileName);
                        if (null != asset)
                        {
                            string shortPathName = fullPathList[i].Replace(fullBaseFileNames, "");
                            shortPathName = shortPathName.Replace($"/{GlobalConsts.ASSET_BUNDLES}/", "/");
                            string fullPath = $"{AssetBundleBuilder.BundleNameStart}{shortPathName}.unity3d";
                            //Debug.LogError($" {shortNameFileName}   {fullPath}");
                            fullPath = fullPath.SanitiseSlashes();
                            asset.SetAssetBundleNameAndVariant(fullPath, "");
                            if(string.IsNullOrEmpty(fullPath) == true)
                            {
                                Debug.LogError("ERROR");
                            }
                        }
                    }
                }
            }
        }


        private static void ChangeFilesToAssetBundlesSubDirectorys(string basePathName, string bundleName, string[] extentions)
        {
            foreach (var item in extentions)
            {
                if (item[0] != '.')
                {
                    Debug.LogError("Extentions need to start with a fullStop");
                }
            }

                string fullBaseFileNames = $"{Application.dataPath}";


                string[] fullFileNames = Directory.GetFiles(basePathName, "*.*", SearchOption.AllDirectories);
                for (int fileNameIndex = 0; fileNameIndex < fullFileNames.Length; fileNameIndex++)
                {
                    string fullFileName = fullFileNames[fileNameIndex];
                    fullFileName = fullFileName.SanitiseSlashes();
                    string extention = Path.GetExtension(fullFileName);
                    string dd = extention;
                    //Debug.LogError($"{basePathName} {fullFileName}");
                    if (true == Array.Exists(extentions, x => x.Equals(extention)))
                    {
                        fullFileNames[fileNameIndex] = fullFileNames[fileNameIndex].SanitiseSlashes();
                        string shortNameFileName = "Assets" + fullFileNames[fileNameIndex].Replace(fullBaseFileNames, "");
                        AssetImporter asset = AssetImporter.GetAtPath(shortNameFileName);
                        if (null != asset)
                        {
                            string shortPathName = basePathName.Replace(fullBaseFileNames, "");
                            shortPathName = shortPathName.Replace($"/{GlobalConsts.ASSET_BUNDLES}/", "/");
                            string fullPath = $"{AssetBundleBuilder.BundleNameStart}{shortPathName}.unity3d";
                            //Debug.LogError($" {shortNameFileName}   {fullPath}");
                            fullPath = fullPath.SanitiseSlashes();
                            asset.SetAssetBundleNameAndVariant(bundleName, "");
                            if (string.IsNullOrEmpty(fullPath) == true)
                            {
                                Debug.LogError("ERROR");
                            }
                        }
                    }
               
            }
        }
    }
}
