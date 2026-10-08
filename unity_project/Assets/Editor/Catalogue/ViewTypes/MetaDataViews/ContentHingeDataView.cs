using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentHingeDataView : MetaDataView<ContentHingeMetaData>
    {

        private HingeDataView m_HingeDataView;
        public ContentHingeDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_HingeDataView = new HingeDataView("Items", m_RootGameObject, m_Entry, m_MetaData.m_HingeData);

            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);
            m_HingeDataView.ToggleFoldout(false);
        }

        protected override void OnMetaDataGUI() 
        {
            m_HingeDataView.OnGUI();
        }

        private class HingeDataView : DefaultListView<ContentHingeMetaData.HingeData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public HingeDataView(string name, GameObject root, CatalogueEntry entry, List<ContentHingeMetaData.HingeData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentHingeMetaData.HingeData DrawElement(ContentHingeMetaData.HingeData renderItem)
            {
                DrawRenderItem(m_Entry, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, ContentHingeMetaData.HingeData dataItem)
            {

                GameObjectToString("Hinge", ref dataItem.m_HingeGameObject, ref dataItem.m_HingeGameObjectName);

                EditorGUILayout.LabelField("Hinge", dataItem.m_HingeGameObjectName);


                dataItem.m_IsConnectedToRoot = EditorGUILayout.Toggle("ConnectedToRoot", dataItem.m_IsConnectedToRoot);

                if (dataItem.m_IsConnectedToRoot == false)
                {
                    GameObjectToString("Connected", ref dataItem.m_ConnectedGameObject, ref dataItem.m_ConnectedGameObjectName);

                    EditorGUILayout.LabelField("Connected", dataItem.m_ConnectedGameObjectName);
                }

                dataItem.m_AngleMin = EditorGUILayout.FloatField("m_AngleMin", dataItem.m_AngleMin);
                dataItem.m_AngleMax = EditorGUILayout.FloatField("m_AngleMax", dataItem.m_AngleMax);

                dataItem.m_Axis = EditorGUILayout.Vector3Field("m_Axis", dataItem.m_Axis);

                EditorGUILayout.Space();

                dataItem.m_UseSpring = EditorGUILayout.Toggle("UseSpring", dataItem.m_UseSpring);
                if(dataItem.m_UseSpring == true)
                {
                    dataItem.m_SpringData.m_Spring = EditorGUILayout.FloatField("   m_Spring", dataItem.m_SpringData.m_Spring);
                    dataItem.m_SpringData.m_Damper = EditorGUILayout.FloatField("   m_Damper", dataItem.m_SpringData.m_Damper);
                    dataItem.m_SpringData.m_TargetPosition = EditorGUILayout.FloatField("   m_TargetPosition", dataItem.m_SpringData.m_TargetPosition);
                }
            }
        }

    }
}

