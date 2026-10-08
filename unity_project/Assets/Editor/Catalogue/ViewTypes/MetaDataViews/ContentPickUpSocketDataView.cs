using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentPickUpSocketDataView : MetaDataView<ContentPickUpSocketMetaData>
    {

        private SingleDataView m_SingleDataView;

        public ContentPickUpSocketDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_SingleDataView = new SingleDataView("Items", m_Entry, m_MetaData.m_Data);
            m_SingleDataView.ToggleFoldout(false);
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);
        }

        protected override void OnMetaDataGUI() 
        {
            m_SingleDataView.OnGUI();
        }


        private class SingleDataView : DefaultListView<ContentPickUpSocketMetaData.SocketData>
        {
            private readonly CatalogueEntry m_Entry;

            public SingleDataView(string name, CatalogueEntry entry, List<ContentPickUpSocketMetaData.SocketData> list) : base(name, list)
            {
                m_Entry = entry;
            }

            protected override ContentPickUpSocketMetaData.SocketData DrawElement(ContentPickUpSocketMetaData.SocketData renderItem)
            {
                DrawRenderItem(m_Entry, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, ContentPickUpSocketMetaData.SocketData dataItem)
            {
                dataItem.m_Weight = EditorGUILayout.FloatField("Weight", dataItem.m_Weight);

                EditorGUILayout.LabelField("    PickUpObject", EditorStyles.boldLabel);
                dataItem.m_UseRoot = EditorGUILayout.Toggle("m_UseRoot", dataItem.m_UseRoot);
                if (dataItem.m_UseRoot == false)
                {
                    GameObjectToString("GameObjectPickup", ref dataItem.m_GameObjectPickup, ref dataItem.m_GameObjectPickUpName);
                }
                GameObjectToString("PickUpSocket", ref dataItem.m_GameObjectPickUpSocket, ref dataItem.m_GameObjectPickUpSocketName);
                dataItem.IsSocketed = EditorGUILayout.Toggle("IsSocketed", dataItem.IsSocketed);

                EditorGUILayout.LabelField("    PickUpObject sockets to, will be fixed", EditorStyles.boldLabel);
                dataItem.m_SocketType = (ContentPickUpSocketMetaData.SocketType)EditorGUILayout.EnumPopup("SocketType:", dataItem.m_SocketType);
                if (dataItem.m_SocketType == ContentPickUpSocketMetaData.SocketType.OneOnly)
                {
                    GameObjectToString("GameObjectSocket", ref dataItem.m_GameObjectSocket, ref dataItem.m_GameObjectSocketName);
                }
                else
                {
                    dataItem.m_GameObjectSocketName = EditorGUILayout.TextField("SocketName", dataItem.m_GameObjectSocketName);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("-----------------------------------------------------------------", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }

    }
}

