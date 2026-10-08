using FilePathHelper;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System;

namespace EditorTools.Bundles
{
    public class AssetBundleDependencyMap : Dictionary<string, BundleLoader.DependencyData[]>
    {
        private Dictionary<string, string> m_NameHash = new Dictionary<string, string>();
        public AssetBundleDependencyMap() { }

        public void Save(FilePath folderPath)
        {
            Json.JsonNet.WriteToFile(this, GetPath(folderPath), true);
        }

        public void Add(AssetBundleManifest manifest, Catalogue cat, string platformName)
        {
            var bundles = manifest.GetAllAssetBundles();
            foreach (string bundle in bundles)
            {
                Add(manifest, bundle, cat, platformName);
            }
        }

        private void Add(AssetBundleManifest manifest, string bundle, Catalogue cat, string platformName)
        {
            int currentIndex = GlobalConsts.CurrentPlatformIndex();

            var dependencies = manifest.GetDirectDependencies(bundle);
            if (dependencies.Length > 0)
            {
                List<BundleLoader.DependencyData> dataList = new List<BundleLoader.DependencyData>();
                for(int i = 0; i < dependencies.Length; i++)
                {
                    BundleLoader.DependencyData newData = new BundleLoader.DependencyData();
                    newData.BundleName = dependencies[i];
                    string guid = "";
                    string prefabName = "";
                    int version = -1;
                    float[] size = new float[GlobalConsts.PlatformNamesLength];
                    float newSize = 0;
                    if(GetData(cat, dependencies[i], ref guid, ref version, ref prefabName, ref size) == true)
                    {
                        newData.PrefabName = prefabName;
                        newData.GUID = guid;
                        newData.Version = version;
                        if(size.Length != GlobalConsts.PlatformNamesLength)
                        {
                            Array.Resize(ref size, GlobalConsts.PlatformNamesLength);
                        }
                        newSize = size[currentIndex];
                    }
                    string fileName = newData.BundleName;
                    fileName = $"{Application.streamingAssetsPath}/Bundles/" + platformName + "/" + fileName;


                    System.IO.FileInfo info = new System.IO.FileInfo(fileName);

                    newSize = (info.Length / 1024f);
                    newSize /= 1024f;
                
                    if(newData.SizeMB == 0 || newSize == 0)
                    {
                        newData.SizeMB = newSize;
                    }
                    else
                    {
                        if(Mathf.Abs(newData.SizeMB - newSize) > 0.1f)
                        {
                            newData.SizeMB = newSize;
                        }
                    }


                    dataList.Add(newData);
                }
                this[bundle] = dataList.ToArray();
            }
        }




        private bool GetData(Catalogue cat, string dependency, ref string guid, ref int version, ref string prefabName, ref float[] size)
        {
            for(int i = 0; i < cat.Entries.Count; i++)
            {
                var data = cat.Entries[i].GetMetaData<RenderMetaData>();
                if(data != null)
                {
                    if(data.Main.Bundle == dependency)
                    {
                        guid = cat.Entries[i].Guid;
                        version = cat.Entries[i].Version;
                        prefabName = data.Main.Prefab;
                        size = (float[])data.SizeOfFileMB.Clone();
                        return true;
                    }
                }
            }
            return false;
        }



        public static AssetBundleDependencyMap Load(FilePath folderPath)
        {
            return Json.JsonNet.ReadFromFile<AssetBundleDependencyMap>(GetPath(folderPath));
        }

        private static string GetPath(FilePath folder) => folder.Append("master").Absolute;
    }
}
