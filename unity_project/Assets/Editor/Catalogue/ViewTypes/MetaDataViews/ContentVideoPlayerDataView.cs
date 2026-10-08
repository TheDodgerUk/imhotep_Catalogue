using EditorTools.Views;
using EnvironmentHelpers;
using FilePathHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{

    public class ContentVideoPlayerDataView : MetaDataView<ContentVideoPlayerMetaData>
    {

        public ContentVideoPlayerDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_MetaData.m_ScreenGameObject = StringToGameObject(m_MetaData.m_ScreenGameObjectName, m_RootGameObject);
        }

        protected override void OnMetaDataGUI()
        {
            m_MetaData.m_ScreenGameObject = StringToGameObject(m_MetaData.m_ScreenGameObjectName, m_RootGameObject);
            GameObjectToString("GameObject", ref m_MetaData.m_ScreenGameObject, ref m_MetaData.m_ScreenGameObjectName);
            EditorGUILayout.LabelField("GameObjectToString", m_MetaData.m_ScreenGameObjectName);

            m_MetaData.m_OrientationType = (ContentVideoPlayerMetaData.OrientationType)EditorGUILayout.EnumPopup("OrientationType", m_MetaData.m_OrientationType);
            m_MetaData.m_DisplayType = (ContentVideoPlayerMetaData.DisplayType)EditorGUILayout.EnumPopup("DisplayType", m_MetaData.m_DisplayType);

        }
    }

}

