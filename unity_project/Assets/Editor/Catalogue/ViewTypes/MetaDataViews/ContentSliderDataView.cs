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
    public class ContentSliderDataView : MetaDataView<ContentSliderMetaData>
    {
        private SliderItemView m_SliderItemView;

        public ContentSliderDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_SliderItemView = new SliderItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_ItemData);
            m_SliderItemView.ToggleFoldout(false);
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);
        }

        protected override void OnMetaDataGUI()
        {
            if (GUILayout.Button("Auto Generate") == true)
            {
                Generate();
            }
            m_SliderItemView.OnGUI();
        }

        private const string START = "Slider";
        private const string LOD = "_LOD";
        private const string END = "_LOD0";
        private const string TARGET = "_Target";
        private void Generate()
        {
            var trans = m_RootGameObject.GetComponentsInChildren<Transform>(true);
            List<Transform> valid = new List<Transform>();
            foreach (var tran in trans)
            {
                if (((tran.name.StartsWith(START, StringComparison.CurrentCultureIgnoreCase)) || (tran.name.EndsWith(START, StringComparison.CurrentCultureIgnoreCase)))
                    && (false == tran.name.EndsWith(TARGET))
                    && ((true == tran.name.EndsWith(END)) || (false == tran.name.Contains(LOD))))
                {
                    if (tran.GetComponents<Component>().Length != 1) // 1 is just a transform
                    {
                        valid.Add(tran);
                    }
                }
            }
            m_MetaData.m_ItemData.Clear();
            for (int i = 0; i < valid.Count; i++)
            {
                ContentSliderMetaData.ItemData newData = new ContentSliderMetaData.ItemData();
                newData.m_MoveType = ContentSliderMetaData.ItemData.MoveType.Amount;
                newData.m_ModelEndLocalPositionfloat = 0.1f;

                var split = valid[i].name.Split('_');
                if (split.Length >= 3)
                {
                    string clean = valid[i].name.Remove(0, START.Length);
                    if (clean.EndsWith(END))
                    {
                        clean = clean.Substring(0, clean.Length - END.Length);
                    }
                    foreach (var tran in trans)
                    {
                        if ((tran.name == $"{START}_{clean}{TARGET}") || (tran.name == $"{START}_{clean}{TARGET}"))
                        {
                            newData.m_MoveType = ContentSliderMetaData.ItemData.MoveType.GameObject;
                            newData.m_ModelEndLocalPositionGameObject = tran.gameObject;
                        }
                    }
                }

                newData.m_ModelGameObject = valid[i].gameObject;
                m_MetaData.m_ItemData.Add(newData);
                GameObjectToString("Slider", ref newData.m_ModelGameObject, ref newData.m_ModelGameObjectName);
            }
        }



        private class SliderItemView : DefaultListView<ContentSliderMetaData.ItemData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public SliderItemView(string name, GameObject root, CatalogueEntry entry, List<ContentSliderMetaData.ItemData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentSliderMetaData.ItemData DrawElement(ContentSliderMetaData.ItemData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentSliderMetaData.ItemData dataItem)
            {
                dataItem.m_ModelGameObject = StringToGameObject(dataItem.m_ModelGameObjectName, root);
                GameObjectToString("Pivot Object", ref dataItem.m_ModelGameObject, ref dataItem.m_ModelGameObjectName);
                EditorGUILayout.LabelField("Pivot Object path", dataItem.m_ModelGameObjectName);

                dataItem.m_MoveType = (ContentSliderMetaData.ItemData.MoveType)EditorGUILayout.EnumPopup("MoveType:", dataItem.m_MoveType);
                switch (dataItem.m_MoveType)
                {
                    case ContentSliderMetaData.ItemData.MoveType.GameObject:
                        GameObjectToString("End", ref dataItem.m_ModelEndLocalPositionGameObject, ref dataItem.m_ModelEndLocalPositionGameObjectName);
                        break;
                    case ContentSliderMetaData.ItemData.MoveType.Amount:
                        dataItem.m_ModelEndLocalPositionfloat = EditorGUILayout.FloatField("ModelMoveAmount", dataItem.m_ModelEndLocalPositionfloat);
                        break;
                    default:
                        break;
                }

                dataItem.m_sTitle = EditorGUILayout.TextField("Title: ", dataItem.m_sTitle);
                dataItem.m_sDescription = EditorGUILayout.TextField("Description: ", dataItem.m_sDescription);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }
    }

}

