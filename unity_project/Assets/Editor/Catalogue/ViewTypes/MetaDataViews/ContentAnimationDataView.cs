using EditorTools.Views;

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentAnimationDataView : MetaDataView<ContentAnimationMetaData>
    {
        private RenderItemView m_AnimationNames;

        public ContentAnimationDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_AnimationNames = new RenderItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_AnimationData);
            m_AnimationNames.ToggleFoldout(false);

            for (int i = 0; i < m_MetaData.m_AnimationData.Count; i++)
            {
                m_MetaData.m_AnimationData[i].AnimationGameObject = StringToGameObject(m_MetaData.m_AnimationData[i].AnimationGameObjectName, m_RootGameObject);
            }
        }


        protected override void OnMetaDataGUI()
        {
            m_MetaData.m_AnimationType = (ContentAnimationMetaData.AnimationType)EditorGUILayout.EnumPopup("Animation Type", m_MetaData.m_AnimationType);
            m_AnimationNames.OnGUI();
        }


        private class RenderItemView : DefaultListView<ContentAnimationMetaData.StringItemData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public RenderItemView(string name, GameObject root, CatalogueEntry entry, List<ContentAnimationMetaData.StringItemData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentAnimationMetaData.StringItemData DrawElement(ContentAnimationMetaData.StringItemData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentAnimationMetaData.StringItemData renderItem)
            {
                renderItem.AnimationGameObject = StringToGameObject(renderItem.AnimationGameObjectName, root);
                bool plus = GameObjectToString("Animation Object", ref renderItem.AnimationGameObject, ref renderItem.AnimationGameObjectName);

                EditorGUILayout.LabelField("Animation Object path", renderItem.AnimationGameObjectName);

                int count = EditorGUILayout.IntField("Animation Name Length", renderItem.AnimationName.Count);
                renderItem.AnimationName.Resize(count);
                for (int i = 0; i < count; i++)
                {
                    renderItem.AnimationName[i] = EditorGUILayout.TextField($"Anim Name {i}", renderItem.AnimationName[i]);
                }
                EditorGUILayout.Space();

            }
        }
    }


}