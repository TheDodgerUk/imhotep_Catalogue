using EditorTools.Views;
using EnvironmentHelpers;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentCollisionDataView : MetaDataView<ContentCollisionMetaData>
    {

        private CollisionDataView m_CollisionDataView;
        public ContentCollisionDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_CollisionDataView = new CollisionDataView("Items", m_RootGameObject, m_Entry, m_MetaData.m_CollisionData);
            m_CollisionDataView.ToggleFoldout(false);
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);
        }

        protected override void OnMetaDataGUI()
        {
            m_CollisionDataView.OnGUI();
        }

        private class CollisionDataView : DefaultListView<ContentCollisionMetaData.CollisionData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public CollisionDataView(string name, GameObject root, CatalogueEntry entry, List<ContentCollisionMetaData.CollisionData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentCollisionMetaData.CollisionData DrawElement(ContentCollisionMetaData.CollisionData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentCollisionMetaData.CollisionData dataItem)
            {
                dataItem.m_Colliders.GameObjectRef = StringToGameObject(dataItem.m_Colliders.GameObjectName, root);
                GameObjectToString("ModelGameObject", ref dataItem.m_Colliders.GameObjectRef, ref dataItem.m_Colliders.GameObjectName);

                int oldCount = dataItem.m_ValidGuids.Count;
                int count = EditorGUILayout.IntField("ValidGuid Length", oldCount);
                if(count != oldCount)
                {
                    dataItem.m_ValidGuids.Resize(count);
                }
                for (int i = 0; i < dataItem.m_ValidGuids.Count; i++)
                {
                    dataItem.m_ValidGuids[i] = EditorGUILayout.TextField("   Guid", dataItem.m_ValidGuids[i]);
                }

                EditorGUILayout.Space();
            }
        }
        

    }

}

