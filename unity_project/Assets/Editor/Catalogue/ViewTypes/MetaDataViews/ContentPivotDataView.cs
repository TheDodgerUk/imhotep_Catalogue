using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentPivotDataView : MetaDataView<ContentPivotMetaData>
    {
        private PivotItemView m_PivotNames;

        public ContentPivotDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_PivotNames = new PivotItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_PivotData);
            m_PivotNames.ToggleFoldout(false);

            for (int i = 0; i < m_MetaData.m_PivotData.Count; i++)
            {
                m_MetaData.m_PivotData[i].m_ModelPivotGameObject = StringToGameObject(m_MetaData.m_PivotData[i].m_ModelPivotName, m_RootGameObject);
            }

        }

        protected override void OnMetaDataGUI()
        {
            m_PivotNames.OnGUI();
        }

        private class PivotItemView : DefaultListView<ContentPivotMetaData.StringItemData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public PivotItemView(string name, GameObject root, CatalogueEntry entry, List<ContentPivotMetaData.StringItemData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentPivotMetaData.StringItemData DrawElement(ContentPivotMetaData.StringItemData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentPivotMetaData.StringItemData dataItem)
            {

                dataItem.m_ModelPivotGameObject = StringToGameObject(dataItem.m_ModelPivotName, root);
                bool plus = GameObjectToString("Pivot Object", ref dataItem.m_ModelPivotGameObject, ref dataItem.m_ModelPivotName);

                EditorGUILayout.LabelField("Pivot Object path", dataItem.m_ModelPivotName);


                dataItem.m_Min = EditorGUILayout.FloatField("m_Min", dataItem.m_Min);
                dataItem.m_Max = EditorGUILayout.FloatField("m_Max", dataItem.m_Max);

                dataItem.m_Axis = (Axis)EditorGUILayout.EnumPopup("m_Axis", dataItem.m_Axis);


                EditorGUILayout.Space();

            }
        }
    }

}

