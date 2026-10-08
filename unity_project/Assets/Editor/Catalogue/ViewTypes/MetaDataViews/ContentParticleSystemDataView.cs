using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentParticleSystemDataView : MetaDataView<ContentParticleSystemMetaData>
    {


        public ContentParticleSystemDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {


            if (m_RootGameObject != null)
            {
                m_MetaData.m_ParticleSystemGameObject = StringToGameObject(m_MetaData.m_ParticleSystemName, m_RootGameObject);
            }
        }

        protected override void OnMetaDataGUI()
        {

            GameObjectToString("ParticleSystem", ref m_MetaData.m_ParticleSystemGameObject, ref m_MetaData.m_ParticleSystemName);
        }
    }




}

