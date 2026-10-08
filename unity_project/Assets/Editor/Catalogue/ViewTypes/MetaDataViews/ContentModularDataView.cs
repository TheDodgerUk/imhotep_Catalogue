using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentModularDataView : MetaDataView<ContentModularMetaData>
    {
        private ModularItemView m_ModularItemView;

        public ContentModularDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_ModularItemView = new ModularItemView("Items", m_Entry, m_MetaData.m_Modular);

            for (int i = 0; i < m_MetaData.m_Modular.Count; i++)
            {
                m_MetaData.m_Modular[i].m_ConnectionPointGameObject = StringToGameObject(m_MetaData.m_Modular[i].m_ConnectionPointGameObjectName, m_RootGameObject);
                m_MetaData.m_Modular[i].m_ItemsRootGameObject = StringToGameObject(m_MetaData.m_Modular[i].m_ItemsRootGameObjectName, m_RootGameObject);
            }
        }

        protected override void OnMetaDataGUI()
        {
            m_ModularItemView.OnGUI();
        }


        private class ModularItemView : DefaultListView<ContentModularMetaData.ModularData>
        {
            private readonly CatalogueEntry m_Entry;

            public ModularItemView(string name, CatalogueEntry entry, List<ContentModularMetaData.ModularData> list) : base(name, list)
            {
                m_Entry = entry;
            }

            protected override ContentModularMetaData.ModularData DrawElement(ContentModularMetaData.ModularData renderItem)
            {
                DrawRenderItem(m_Entry, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, ContentModularMetaData.ModularData dataItem)
            {
                GameObjectToString("m_ConnectionPointGameObject", ref dataItem.m_ConnectionPointGameObject, ref dataItem.m_ConnectionPointGameObjectName);
                GameObjectToString("m_ItemsRootGameObject", ref dataItem.m_ItemsRootGameObject, ref dataItem.m_ItemsRootGameObjectName);
                EditorGUILayout.Space();
            }
        }
    }
}

