using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentPickUpDataView : MetaDataView<ContentPickUpMetaData>
    {

        private IgnoreColliderView m_IgnoreColliderView;
        public ContentPickUpDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_IgnoreColliderView = new IgnoreColliderView("Items", m_RootGameObject, m_Entry, m_MetaData.m_IgnoreColliders);
            m_IgnoreColliderView.ToggleFoldout(false);
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);
        }
        protected override void OnMetaDataGUI() 
        { 
            m_MetaData.m_WeightKG = EditorGUILayout.FloatField("m_WeightKG", m_MetaData.m_WeightKG);
            m_MetaData.m_ThrowPower = EditorGUILayout.FloatField("m_ThrowPower", m_MetaData.m_ThrowPower);
            m_IgnoreColliderView.OnGUI();
        }

        private class IgnoreColliderView : DefaultListView<ColliderGameObjectData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public IgnoreColliderView(string name, GameObject root, CatalogueEntry entry, List<ColliderGameObjectData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ColliderGameObjectData DrawElement(ColliderGameObjectData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, GameObjectData dataItem)
            {
                GameObjectToString("Ignore Collider", dataItem);



            }
        }

    }
}

