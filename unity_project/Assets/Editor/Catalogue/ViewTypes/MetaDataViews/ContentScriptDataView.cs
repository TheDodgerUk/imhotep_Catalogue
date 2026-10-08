using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentScriptDataView : MetaDataView<ContentScriptMetaData>
    {
        public ContentScriptDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
        }

        protected override void OnMetaDataGUI()
        {
            m_MetaData.m_AddScript = (ContentScriptMetaData.AddScript)EditorGUILayout.EnumPopup("Add Script", m_MetaData.m_AddScript);
        }
    }

}

