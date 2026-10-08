using System;
using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using static ContentAudioMetaData;

namespace EditorTools.CatalogueViewer
{
    public class ContentAudioDataView : MetaDataView<ContentAudioMetaData>
    {

        private RootItemsView m_RootItemsView;

        public ContentAudioDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            if (m_RootGameObject != null)
            {

                m_RootItemsView = new RootItemsView("Sound View", m_Entry, m_MetaData.m_AudioDatas, m_RootGameObject);



                for (int i = 0; i < m_MetaData.m_AudioDatas.Count; i++)
                {
                    m_MetaData.m_AudioDatas[i].m_BaseSoundGameObject = StringToGameObject(m_MetaData.m_AudioDatas[i].m_BaseSoundGameObjectName, m_RootGameObject);

                    m_MetaData.m_AudioDatas[i].m_SoundCollider = StringToComponent<Collider>(m_MetaData.m_AudioDatas[i].m_SoundColliderName, m_RootGameObject);
                    for (int x = 0; x < m_MetaData.m_AudioDatas[i].m_AudioSubGroupData.Count; x++)
                    {
                        m_MetaData.m_AudioDatas[i].m_AudioSubGroupData[x].m_AudioSource = StringToComponent<AudioSource>(m_MetaData.m_AudioDatas[i].m_AudioSubGroupData[x].m_AudioSourceName, m_RootGameObject);

                    }
                }
            }
        }


        protected override void OnMetaDataGUI()
        {
            m_RootItemsView.OnGUI();
        }


        private class RootItemsView : DefaultListView<AudioGroupData>
        {
            private readonly CatalogueEntry m_Entry;
            private readonly GameObject m_RootGameObject;
            

            public RootItemsView(string name, CatalogueEntry entry, List<AudioGroupData> list, GameObject root) : base(name, list)
            {
                m_Entry = entry;
                m_RootGameObject = root;
            }

            protected override AudioGroupData DrawElement(AudioGroupData renderItem)
            {
                DrawRenderItem(m_Entry, renderItem, m_RootGameObject);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, AudioGroupData dataItem, GameObject root)
            {
                dataItem.m_BaseSoundGameObject = StringToGameObject(dataItem.m_BaseSoundGameObjectName, root);
                GameObjectToString("BaseSoundGameObject", ref dataItem.m_BaseSoundGameObject, ref dataItem.m_BaseSoundGameObjectName);
                EditorGUILayout.LabelField("BaseSoundGameObject path", dataItem.m_BaseSoundGameObjectName);

                var obj = StringToGameObject(dataItem.m_SoundColliderName, root);
                if(obj != null)
                {
                    dataItem.m_SoundCollider = obj.GetComponent<Collider>();
                }

                ComponentToString("SoundCollider ", ref dataItem.m_SoundCollider, ref dataItem.m_SoundColliderName);
                if(dataItem.m_SoundCollider != null && dataItem.m_SoundCollider.GetComponent<Collider>() == null)
                {
                    dataItem.m_SoundColliderName = "NOT VALID COLIIDER";
                }
                EditorGUILayout.LabelField("SoundCollider path", dataItem.m_SoundColliderName);

                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("", GUILayout.Width(50));

                if (GUILayout.Button("Clear Data", GUILayout.Width(100), GUILayout.Height(15)) == true)
                {
                    if (dataItem.m_BaseSoundGameObject != null)
                    {
                        dataItem.m_AudioSubGroupData = new List<AudioSubGroupData>();
                    }
                }

                if (GUILayout.Button("Collect Data", GUILayout.Width(100), GUILayout.Height(15)) == true)
                {
                    if(dataItem.m_BaseSoundGameObject != null)
                    {
                        var all = dataItem.m_BaseSoundGameObject.transform.GetComponentsInChildren<AudioSource>();

                        dataItem.m_AudioSubGroupData.Resize(all.Length);

                        for (int i = 0; i < all.Length; i++)
                        {
                            dataItem.m_AudioSubGroupData[i] = new AudioSubGroupData();

                            dataItem.m_AudioSubGroupData[i].m_AudioSource = all[i];                           
                            
                            dataItem.m_AudioSubGroupData[i].m_AudioSource.playOnAwake = false;

                            ComponentToString<AudioSource>("", ref dataItem.m_AudioSubGroupData[i].m_AudioSource, ref dataItem.m_AudioSubGroupData[i].m_AudioSourceName, true);
                        }
                    }
                }
                GUILayout.EndHorizontal();

                foreach (var item in dataItem.m_AudioSubGroupData)
                {
                    EditorGUILayout.LabelField("\t\t item  ", item.m_AudioSourceName);
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

            }
        }


    }

}

