using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentLightingDataView : MetaDataView<ContentLightingMetaData>
    {

        private static string FixturesPath => $"{Application.dataPath}/AssetBundles/HouseBuilder/Builder/TextAssetBundle";
        public static Dictionary<string, Fixtures.Fixture> m_FixtureDictionary = new Dictionary<string, Fixtures.Fixture>();

        int m_NameIndex = 0;
        private List<string> m_NameList = new List<string>();



        public ContentLightingDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            InitiliseFeaturesList(false);


            if (m_NameList.Count == 0)
            {
                foreach (var item in m_FixtureDictionary.Keys)
                {
                    m_NameList.Add(item);
                    m_NameList.Sort();
                }
            }

            if (string.IsNullOrEmpty(m_MetaData.m_JsonFileName) == false)
            {
                m_NameIndex = m_NameList.FindIndex(e => e == m_MetaData.m_JsonFileName);
                if(m_NameIndex == -1)
                {
                    Debug.LogError($"Count not find {m_MetaData.m_JsonFileName}");
                }
            }
            else
            {
                m_NameIndex = 0;               
            }
            ReBuildData();
        }


        public static void InitiliseFeaturesList(bool forceReload)
        {
            if (m_FixtureDictionary.Count == 0 || forceReload == true)
            {
                m_FixtureDictionary.Clear();
                string folder = FixturesPath; // this folder needs to be put on the server
                if (Directory.Exists(folder) == true)
                {
                    var files = Directory.GetFiles($"{folder}", "*.json", SearchOption.AllDirectories);
                    for (int i = 1; i < files.Length; i++)
                    {
                        string name = files[i].SanitiseSlashes().Remove(0, folder.Length + 1); // +1 to remove the first /
                        name = name.Replace(".json", "");
                        string readText = File.ReadAllText(files[i]);
                        var data = Fixtures.Fixture.FromJson(readText);
                        name = name.Replace(".json", "");
                        if (m_FixtureDictionary.ContainsKey(name) == false)
                        {
                            m_FixtureDictionary.Add(name, data);
                        }
                        else
                        {
                            Debug.LogError($"m_FixtureDictionary allready conatins key :{name}");
                        }
                    }
                }
                else
                {
                    Debug.LogError($"Cannot find folder:  {FixturesPath} ");
                }
            }
        }



        protected override void OnMetaDataGUI()
        {
            if (GUILayout.Button("Rebuild FeatureList") == true)
            {
                m_FixtureDictionary.Clear();
                InitiliseFeaturesList(true);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Open json file") == true)
            {
                string jsonfile = $"{FixturesPath}/{m_NameList[m_NameIndex]}.json";
                System.Diagnostics.Process.Start(jsonfile);
            }


            int temp = EditorGUILayout.Popup("Items", m_NameIndex, m_NameList.ToArray());
            if (m_NameIndex != temp)
            {
                m_NameIndex = temp;
                ReBuildData();
            }

            if (m_MetaData.m_Fixure != null && m_MetaData.m_Fixure.Modes != null)
            {
                for (int i = 0; i < m_MetaData.m_Fixure.Modes.Count; i++)
                {
                    EditorGUILayout.LabelField(m_MetaData.m_Fixure.Modes[i].Name);
                    for (int j = 0; j < m_MetaData.m_Fixure.Modes[i].Channels.Count; j++)
                    {
                        string name = m_MetaData.m_Fixure.Modes[i].Channels[j].String;
                        m_MetaData.m_GameObjectLinks[i].m_GameObjectLink[j].m_CapabilityType = name;
                        var data = m_MetaData.m_GameObjectLinks[i].m_GameObjectLink[j];
                        GameObjectToString($"     {name}", ref data.m_GameObject, ref data.m_GameObjectName);
                    }
                    EditorGUILayout.Space();
                }
            }


            m_MetaData.m_InvertPan = EditorGUILayout.Toggle("InvertPan", m_MetaData.m_InvertPan);
            m_MetaData.m_InvertTilt = EditorGUILayout.Toggle("InvertTilt", m_MetaData.m_InvertTilt);

            m_MetaData.m_SpeedPan = EditorGUILayout.FloatField("SpeedPan", m_MetaData.m_SpeedPan);
            m_MetaData.m_SpeedTilt = EditorGUILayout.FloatField("SpeedTilt", m_MetaData.m_SpeedTilt);


            if (true == GUILayout.Button("Open file", GUILayout.Width(150)))
            {
                string path = $"{FixturesPath}/{m_NameList[m_NameIndex]}";
                System.Diagnostics.Process.Start($"{path}.json");
            }
        }

        private void ReBuildData()
        {
            if (m_NameList.Count != 0)
            {
                m_MetaData.m_JsonFileName = m_NameList[m_NameIndex];
                m_MetaData.m_Fixure = m_FixtureDictionary[m_MetaData.m_JsonFileName];

                // create save structure 
                m_MetaData.m_GameObjectLinks.Resize(m_MetaData.m_Fixure.Modes.Count);
                for (int i = 0; i < m_MetaData.m_Fixure.Modes.Count; i++)
                {
                    if (m_MetaData.m_GameObjectLinks[i] == null)
                    {
                        m_MetaData.m_GameObjectLinks[i] = new ContentLightingMetaData.GameObjectLinks();
                    }

                    m_MetaData.m_GameObjectLinks[i].m_GameObjectLink.Resize(m_MetaData.m_Fixure.Modes[i].Channels.Count);
                    for (int j = 0; j < m_MetaData.m_Fixure.Modes[i].Channels.Count; j++)
                    {
                        if (m_MetaData.m_GameObjectLinks[i].m_GameObjectLink[j] == null)
                        {
                            m_MetaData.m_GameObjectLinks[i].m_GameObjectLink[j] = new ContentLightingMetaData.GameObjectLink();
                        }
                    }
                }

                if (m_MetaData.m_Fixure != null && m_MetaData.m_Fixure.Modes != null)
                {
                    for (int i = 0; i < m_MetaData.m_Fixure.Modes.Count; i++)
                    {
                        EditorGUILayout.LabelField(m_MetaData.m_Fixure.Modes[i].Name);
                        for (int j = 0; j < m_MetaData.m_Fixure.Modes[i].Channels.Count; j++)
                        {
                            string name = m_MetaData.m_Fixure.Modes[i].Channels[j].String;
                            var data = m_MetaData.m_GameObjectLinks[i].m_GameObjectLink[j];
                            data.m_GameObject = StringToGameObject(data.m_GameObjectName, m_RootGameObject);
                        }
                    }
                }
            }
        }

    }

}

