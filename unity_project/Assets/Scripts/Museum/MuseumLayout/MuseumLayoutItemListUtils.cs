using FilePathHelper;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Museum
{
    public static class MuseumLayoutListUtils
    {
        public static string JSON_SAVEASSETPATH => $"{Device.StreamingPath}/Museum/Turbo sound Museum/Data/";
        public const string LOADASSETPATH = "Layouts/";


        public static MuseumFloorData Create(string _name)
        {
            MuseumFloorData il = new MuseumFloorData();
            if (il != null)
            {
                il.m_Name = _name;
                return il;
            }
            else
            {
                return null;
            }
        }



        private static void DeleteFiles()
        {
            string[] fileArray = Directory.GetFiles(Application.dataPath + JSON_SAVEASSETPATH, "*.json");
            for (int i = 0; i < fileArray.Length; i++)
            {
                File.Delete(fileArray[i]);
            }
        }


        public static void Save(MuseumFloorDataContainer data)
        {
#if UNITY_EDITOR
            DeleteFiles();

            if (null != data)
            {
                for (int i = 0; i < data.GetCount(); i++)
                {
                    MuseumFloorData museumFloorDataRef = data.GetAt(i);
                    museumFloorDataRef.m_OptionalScenesListNames.Clear();
                    for (int sceneIndex = 0; sceneIndex < museumFloorDataRef.m_OptionalScenesList.Count; sceneIndex++)
                    {
                        museumFloorDataRef.m_OptionalScenesListNames.Add(museumFloorDataRef.m_OptionalScenesList[sceneIndex].name);
                    }
                    string fileName = Application.dataPath + JSON_SAVEASSETPATH + museumFloorDataRef.m_Name;
                    Json.JsonNet.WriteToFile(museumFloorDataRef, fileName, true);
                }
            }
#endif
        }

        public static MuseumFloorDataContainer LoadData()
        {
            MuseumFloorDataContainer museumFloorDataContainerRef = new MuseumFloorDataContainer();      
            FindAndAssignCorrectSceneAsset(museumFloorDataContainerRef);
            return museumFloorDataContainerRef;
        }


        private static void FindAndAssignCorrectSceneAsset(MuseumFloorDataContainer museumFloorDataContainerRef)
        {
#if UNITY_EDITOR
            string[] fileArray = Directory.GetFiles(Application.dataPath + JSON_SAVEASSETPATH, "*.json");
            for (int i = 0; i < fileArray.Length; i++)
            {
                string noFileExtention = fileArray[i].Replace(".json", "");
                MuseumFloorData data = Json.JsonNet.ReadFromFile<MuseumFloorData>(noFileExtention);

                data.m_OptionalScenesList.Clear();
                for (int sceneIndex = 0; sceneIndex < data.m_OptionalScenesListNames.Count; sceneIndex++)
                {
                    string stringToFind = data.m_OptionalScenesListNames[sceneIndex];
                    string stringToFindWithExtention = stringToFind + ".unity";

                    string[] guidArray = AssetDatabase.FindAssets($"{stringToFind} t:scene", null);
                    foreach (string guid in guidArray)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        if ((path.EndsWith(stringToFindWithExtention)) && (false == path.Contains("_")))
                        {
                            data.m_OptionalScenesList.Add(AssetDatabase.LoadAssetAtPath<SceneAsset>(path));
                        }
                    }
                }
                museumFloorDataContainerRef.Add(data);
            }
#endif
        }

    }
}