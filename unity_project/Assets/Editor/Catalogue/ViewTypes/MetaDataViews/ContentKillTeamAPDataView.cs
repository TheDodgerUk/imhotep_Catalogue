using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentKillTeamAPDataView : MetaDataView<ContentKillTeamAPMetaData>
    {

        public ContentKillTeamAPDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {

        }
        protected override void OnMetaDataGUI()
        {
            m_MetaData.apType = (ContentKillTeamAPMetaData.ApType)EditorGUILayout.EnumFlagsField("AP TYPE", m_MetaData.apType);
            m_MetaData.KillTeamType = (ContentKillTeamMetaData.TeamType)EditorGUILayout.EnumFlagsField("teamType", m_MetaData.KillTeamType);
        }


    }
}

