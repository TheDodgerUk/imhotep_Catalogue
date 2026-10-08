using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentFoodDataView : MetaDataView<ContentFoodMetaData>
    {

        private FoodItemView m_FoodItemView;
        public ContentFoodDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_FoodItemView = new FoodItemView("FoodPart", m_RootGameObject, m_Entry, m_MetaData.m_FoodParts);
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);

        }
        protected override void OnMetaDataGUI() 
        {
            m_FoodItemView.OnGUI();
        }

        private class FoodItemView : DefaultListView<ContentFoodMetaData.FoodParts>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public FoodItemView(string name, GameObject root, CatalogueEntry entry, List<ContentFoodMetaData.FoodParts> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentFoodMetaData.FoodParts DrawElement(ContentFoodMetaData.FoodParts renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentFoodMetaData.FoodParts dataItem)
            {
                GameObjectToString("FoodPart", dataItem.m_FoodPart);
                dataItem.m_Weight = EditorGUILayout.FloatField("Weight", dataItem.m_Weight);


                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }
    }

}

