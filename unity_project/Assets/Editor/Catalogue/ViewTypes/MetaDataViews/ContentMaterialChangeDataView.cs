using EditorTools.Views;
using EnvironmentHelpers;
using FilePathHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using static ContentMaterialChangeMetaData.MaterialItemData;

namespace EditorTools.CatalogueViewer
{

    public class ContentMaterialChangeDataView : MetaDataView<ContentMaterialChangeMetaData>
    {
        private MaterialItemView m_MaterialItemView;

        public ContentMaterialChangeDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {

            m_MaterialItemView = new MaterialItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_MaterialItemData);

            for (int i = 0; i < m_MetaData.m_MaterialItemData.Count; i++)
            {
                m_MetaData.m_MaterialItemData[i].m_MaterialGameObject = StringToGameObject(m_MetaData.m_MaterialItemData[i].m_MaterialGameObjectName, m_RootGameObject);
            }
        }

        protected override void OnMetaDataGUI()
        {
            m_MaterialItemView.OnGUI();
        }


        private class MaterialItemView : DefaultListView<ContentMaterialChangeMetaData.MaterialItemData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public MaterialItemView(string name, GameObject root, CatalogueEntry entry, List<ContentMaterialChangeMetaData.MaterialItemData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentMaterialChangeMetaData.MaterialItemData DrawElement(ContentMaterialChangeMetaData.MaterialItemData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentMaterialChangeMetaData.MaterialItemData dataItem)
            {
                dataItem.m_MaterialGameObject = StringToGameObject(dataItem.m_MaterialGameObjectName, root);
                bool plus = GameObjectToString("Material Object", ref dataItem.m_MaterialGameObject, ref dataItem.m_MaterialGameObjectName);

                EditorGUILayout.LabelField("Material Object path", dataItem.m_MaterialGameObjectName);
                dataItem.m_MaterialIndex = EditorGUILayout.IntField("Material Index", dataItem.m_MaterialIndex);

                dataItem.m_MaterialType = (MaterialType)EditorGUILayout.EnumPopup("Type", dataItem.m_MaterialType);

                switch (dataItem.m_MaterialType)
                {
                    case MaterialType.Colour:
                        // nothing
                        break;
                    case MaterialType.Material:
                    case MaterialType.Texture:
                        dataItem.m_OrientationType = (ContentMaterialChangeMetaData.MaterialItemData.OrientationType)EditorGUILayout.EnumPopup("Orientation", dataItem.m_OrientationType);
                        dataItem.m_TextureMap = (ContentMaterialChangeMetaData.MaterialItemData.TextureMap)EditorGUILayout.EnumPopup("m_TextureMap", dataItem.m_TextureMap);
                        break;
                    default:
                        break;
                }
                

                EditorGUILayout.Space();
            }
        }
    }

}

