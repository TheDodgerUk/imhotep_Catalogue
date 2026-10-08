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
    public class ContentButtonDataView : MetaDataView<ContentButtonMetaData>
    {
        private ButtonItemView m_ButtonItemView;

        public ContentButtonDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_ButtonItemView = new ButtonItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_ItemData);
            m_ButtonItemView.ToggleFoldout(false);
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);

        }

        protected override void OnMetaDataGUI()
        {
            if(GUILayout.Button("Auto Generate") == true)
            {
                Generate();
            }
            m_ButtonItemView.OnGUI();
        }

        private const string START = "Button";
        private const string LOD = "_LOD";
        private const string END = "_LOD0";
        private const string TARGET = "_Target";
        private void Generate()
        {
            if(m_RootGameObject == null)
            {
                m_RootGameObject = GameObject.Find(m_RenderMetaData.Main.Prefab);
                m_MetaData.CollectAllData(m_RootGameObject);
            }
            var trans = m_RootGameObject.GetComponentsInChildren<Transform>(true);
            List<Transform> valid = new List<Transform>();
            foreach (var tran in trans)
            {
                if (((tran.name.StartsWith(START, StringComparison.CurrentCultureIgnoreCase)) || (tran.name.EndsWith(START, StringComparison.CurrentCultureIgnoreCase)))
                    && (false == tran.name.EndsWith(TARGET))
                    && ((true == tran.name.EndsWith(END)) || (false == tran.name.Contains(LOD))))
                {
                    valid.Add(tran);
                }
            }
            m_MetaData.m_ItemData.Clear();
            for (int i = 0; i < valid.Count; i++)
            {
                ContentButtonMetaData.ButtonItemData newData = new ContentButtonMetaData.ButtonItemData();
                newData.m_MoveType = ContentSliderMetaData.ItemData.MoveType.Amount;


                var split = valid[i].name.Split('_');
                if (split.Length >= 2)
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
                GameObjectToString("Button", ref newData.m_ModelGameObject, ref newData.m_ModelGameObjectName);
            }
        }

        private class ButtonItemView : DefaultListView<ContentButtonMetaData.ButtonItemData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public ButtonItemView(string name, GameObject root, CatalogueEntry entry, List<ContentButtonMetaData.ButtonItemData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentButtonMetaData.ButtonItemData DrawElement(ContentButtonMetaData.ButtonItemData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentButtonMetaData.ButtonItemData dataItem)
            {
                dataItem.m_ModelGameObject = StringToGameObject(dataItem.m_ModelGameObjectName, root);
                GameObjectToString("ModelGameObject", ref dataItem.m_ModelGameObject, ref dataItem.m_ModelGameObjectName);
                EditorGUILayout.LabelField("ModelGameObject path", dataItem.m_ModelGameObjectName);

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

                dataItem.m_ButtonType = (ContentButtonMetaData.ButtonItemData.ButtonType)EditorGUILayout.EnumPopup("ButtonType:", dataItem.m_ButtonType);
                dataItem.m_ModelMoveTime = EditorGUILayout.FloatField("ModelMoveTime", dataItem.m_ModelMoveTime);


                EditorGUILayout.Space();
                EditorGUILayout.Space();
                dataItem.m_SimpleButtonType = (ContentButtonMetaData.SimpleButtonType)EditorGUILayout.EnumPopup("m_SimpleButtonType:", dataItem.m_SimpleButtonType);

                if (dataItem.m_SimpleButtonType != ContentButtonMetaData.SimpleButtonType.None)
                {
                    dataItem.m_SimpleButtonPress = (ContentButtonMetaData.ButtonItemData.SimpleButtonPress)EditorGUILayout.EnumPopup("m_SimpleButtonPress:", dataItem.m_SimpleButtonPress);

                    if (dataItem.m_SimpleButtonType == ContentButtonMetaData.SimpleButtonType.SetActive)
                    {
                        GameObjectToString("SetActive", dataItem.m_SetActive);
                    }

                    if (dataItem.m_SimpleButtonType == ContentButtonMetaData.SimpleButtonType.Animation)
                    {
                        GameObjectToString("SetAnimation", dataItem.m_SetAnimation);
                        dataItem.m_SetAnimation.AnimationName = EditorGUILayout.TextField("AnimationNameOn", dataItem.m_SetAnimation.AnimationName);
                    }

                    if (dataItem.m_SimpleButtonType == ContentButtonMetaData.SimpleButtonType.Emission)
                    {
                        GameObjectToString("SetEmmissive", dataItem.m_SetEmmissive);
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                dataItem.m_UseEmmision = EditorGUILayout.Toggle("UseEmmision", dataItem.m_UseEmmision);

                if(dataItem.m_UseEmmision == true)
                {
                    dataItem.m_Up = EditorGUILayout.ColorField("Up Emmision", dataItem.m_Up);
                    dataItem.m_Down = EditorGUILayout.ColorField("Down Emmision", dataItem.m_Down);
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

