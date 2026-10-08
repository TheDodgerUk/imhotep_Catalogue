using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentGameObjectCycleDataView : MetaDataView<ContentGameObjectCycleMetaData>
    {
        private CycleDataView m_CycleDataView;

        public ContentGameObjectCycleDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_CycleDataView = new CycleDataView("Items", m_RootGameObject, m_Entry, m_MetaData.m_CycleData);
            m_CycleDataView.ToggleFoldout(false);

            for (int i = 0; i < m_MetaData.m_CycleData.Count; i++)
            {
                m_MetaData.m_CycleData[i].m_GameObject = StringToGameObject(m_MetaData.m_CycleData[i].m_GameObjectName, m_RootGameObject);
            }

        }

        protected override void OnMetaDataGUI()
        {
            m_CycleDataView.OnGUI();
        }

        private class CycleDataView : DefaultListView<ContentGameObjectCycleMetaData.CycleData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public CycleDataView(string name, GameObject root, CatalogueEntry entry, List<ContentGameObjectCycleMetaData.CycleData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentGameObjectCycleMetaData.CycleData DrawElement(ContentGameObjectCycleMetaData.CycleData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentGameObjectCycleMetaData.CycleData dataItem)
            {
                dataItem.m_GameObject = StringToGameObject(dataItem.m_GameObjectName, root);
                GameObjectToString("GameObject Cycle", ref dataItem.m_GameObject, ref dataItem.m_GameObjectName);

                EditorGUILayout.LabelField("GameObjectToString", dataItem.m_GameObjectName);

                EditorGUILayout.Space();

            }
        }
    }

}

