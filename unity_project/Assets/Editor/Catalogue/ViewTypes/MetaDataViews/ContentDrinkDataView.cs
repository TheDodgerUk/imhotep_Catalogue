using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentDrinkDataView : MetaDataView<ContentDrinkMetaData>
    {

        public ContentDrinkDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);        
        }
        protected override void OnMetaDataGUI() 
        {
            m_MetaData.m_WeightKG = EditorGUILayout.FloatField("m_WeightKG", m_MetaData.m_WeightKG);
            m_MetaData.m_LiquidMin = EditorGUILayout.FloatField("m_LiquidMin", m_MetaData.m_LiquidMin);
            m_MetaData.m_LiquidMax = EditorGUILayout.FloatField("m_LiquidMax", m_MetaData.m_LiquidMax);
            

            GameObjectToString($"m_LiquidWobbleGameObjectName", ref m_MetaData.m_LiquidWobbleGameObject, ref m_MetaData.m_LiquidWobbleGameObjectName);


            GameObjectToString($"m_LiquidPouringGameObjectName", ref m_MetaData.m_LiquidPouringGameObject, ref m_MetaData.m_LiquidPouringGameObjectName);
        }

    }
}

