using EditorTools.Views;
using EnvironmentHelpers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentStagingDataView : MetaDataView<ContentStagingMetaData>
    {
        private StagingItemView m_ButtonItemView;

        public ContentStagingDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_ButtonItemView = new StagingItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_StagingData);
            m_ButtonItemView.ToggleFoldout(false);

            m_MetaData.CollectAllData(m_RootGameObject);
        }

        protected override void OnMetaDataGUI()
        {
            m_MetaData.m_ConectionType = (ContentStagingMetaData.ConectionType)EditorGUILayout.EnumPopup("Connection :", m_MetaData.m_ConectionType);

            EditorGUILayout.LabelField($"Looking for Items Starting : {START}");
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("Auto Generate") == true)
            {
                Generate();
            }
            m_ButtonItemView.OnGUI();
        }

        private const string START = "Staging";

        private void Generate()
        {
            var trans = m_RootGameObject.GetComponentsInChildren<Transform>(true);
            List<Transform> valid = new List<Transform>();
            foreach (var tran in trans)
            {
                Debug.LogError(tran.name);
                if (tran.name.StartsWith(START))
                {
                    valid.Add(tran);
                }
            }
            m_MetaData.m_StagingData.Clear();
            for (int i = 0; i < valid.Count; i++)
            {
                ContentStagingMetaData.StagingData newData = new ContentStagingMetaData.StagingData();

                newData.m_ModelGameObject = valid[i].gameObject;

                m_MetaData.m_StagingData.Add(newData);
                GameObjectToString("Staging", ref newData.m_ModelGameObject, ref newData.m_ModelGameObjectName);
            }

            m_MetaData.m_ConectionType = ContentStagingMetaData.ConectionType.Ladder;
            if (m_RootGameObject.name.CaseInsensitiveContains("Triangular") == true) 
            {
                m_MetaData.m_ConectionType = ContentStagingMetaData.ConectionType.Triangle;
            }

            if (m_RootGameObject.name.CaseInsensitiveContains("Square") == true)
            {
                m_MetaData.m_ConectionType = ContentStagingMetaData.ConectionType.Square;
            }

            if (m_RootGameObject.name.CaseInsensitiveContains("Rectang") == true)
            {
                m_MetaData.m_ConectionType = ContentStagingMetaData.ConectionType.Rectangle;
            }

            Debug.LogError(m_MetaData.m_StagingData.Count);
        }

        private class StagingItemView : DefaultListView<ContentStagingMetaData.StagingData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public StagingItemView(string name, GameObject root, CatalogueEntry entry, List<ContentStagingMetaData.StagingData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentStagingMetaData.StagingData DrawElement(ContentStagingMetaData.StagingData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentStagingMetaData.StagingData dataItem)
            {
                dataItem.m_ModelGameObject = StringToGameObject(dataItem.m_ModelGameObjectName, root);
                GameObjectToString("Staging", ref dataItem.m_ModelGameObject, ref dataItem.m_ModelGameObjectName);
                EditorGUILayout.LabelField("Staging path", dataItem.m_ModelGameObjectName);
                
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

            }
        }
    }

}

