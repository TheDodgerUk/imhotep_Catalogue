using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentLightDataView : MetaDataView<ContentLightMetaData>
    {

        private EmmisiveDataView m_EmmisiveDataView;

        public ContentLightDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_EmmisiveDataView = new EmmisiveDataView("Items", m_Entry, m_MetaData.m_EmissiveData);
            m_EmmisiveDataView.ToggleFoldout(false);

            for (int i = 0; i < m_MetaData.m_EmissiveData.Count; i++)
            {
                m_MetaData.m_EmissiveData[i].m_GameObject = StringToGameObject(m_MetaData.m_EmissiveData[i].m_GameObjectName, m_RootGameObject);
            }

        }



        protected override void OnMetaDataGUI()
        {
            m_MetaData.m_CanEdit = EditorGUILayout.Toggle("CanEdit", m_MetaData.m_CanEdit);
            m_MetaData.m_LightEnumType = (ContentLightMetaData.LightEnumType)EditorGUILayout.EnumPopup("LightType", m_MetaData.m_LightEnumType);
            EditorGUILayout.Space();

            m_MetaData.m_Range = EditorGUILayout.Vector2Field("Range", m_MetaData.m_Range);
            m_MetaData.m_Intensity = EditorGUILayout.Vector2Field("Intensity", m_MetaData.m_Intensity);
            

            if (m_MetaData.m_LightEnumType == ContentLightMetaData.LightEnumType.Spot)
            {
                m_MetaData.m_Angle = EditorGUILayout.Vector2Field("Angle", m_MetaData.m_Angle);
            }

            m_EmmisiveDataView.OnGUI();
        }


        private class EmmisiveDataView : DefaultListView<ContentLightMetaData.EmissiveData>
        {
            private readonly CatalogueEntry m_Entry;

            public EmmisiveDataView(string name, CatalogueEntry entry, List<ContentLightMetaData.EmissiveData> list) : base(name, list)
            {
                m_Entry = entry;
            }

            protected override ContentLightMetaData.EmissiveData DrawElement(ContentLightMetaData.EmissiveData renderItem)
            {
                DrawRenderItem(m_Entry, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, ContentLightMetaData.EmissiveData dataItem)
            {

                GameObjectToString("Emmissive Object", ref dataItem.m_GameObject, ref dataItem.m_GameObjectName);

                EditorGUILayout.LabelField("GameObjectToString", dataItem.m_GameObjectName);

                EditorGUILayout.Space();

            }
        }
    }

}

