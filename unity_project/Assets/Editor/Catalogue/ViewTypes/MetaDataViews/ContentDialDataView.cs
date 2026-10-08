using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentDialDataView : MetaDataView<ContentDialMetaData>
    {
        private DialItemView m_DialItemView;
        private PatchItemView m_PatchItemView;

        public ContentDialDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_DialItemView = new DialItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_DialData);
            m_DialItemView.ToggleFoldout(false);

            m_PatchItemView = new PatchItemView("Patches", m_Entry, m_MetaData.m_PatchData);
            m_PatchItemView.ToggleFoldout(false);
            OnFoldout((fold) => m_MetaData.CollectAllData(m_RootGameObject));
            m_MetaData.CollectAllData(m_RootGameObject);
        }

        protected override void OnMetaDataGUI()
        {
            if (GUILayout.Button("Auto Generate") == true)
            {
                Generate();
            }

            EditorGUILayout.Space();

            GUILayout.Label("Dials:");
            m_DialItemView.OnGUI();

            EditorGUILayout.Space();

            GUILayout.Label("Patches:");
            m_PatchItemView.OnGUI();
        }

        private const string START = "Dial_";
        private const string START_LOWERCASE = "dial_";
        private const string LOD = "_LOD";
        private const string END = "_LOD0";
        private const string TARGET = "_Target";

        private void Generate()
        {
            var trans = m_RootGameObject.GetComponentsInChildren<Transform>(true);
            List<Transform> valid = new List<Transform>();
            foreach (var tran in trans)
            {
                if (((tran.name.StartsWith(START)) || (tran.name.StartsWith(START_LOWERCASE)))
                  && (false == tran.name.EndsWith(TARGET))
                  && ((true == tran.name.EndsWith(END)) || (false == tran.name.Contains(LOD))))
                {
                    valid.Add(tran);
                }
            }
            m_MetaData.m_DialData.Clear();
            for (int i = 0; i < valid.Count; i++)
            {
                ContentDialMetaData.StringItemData newData = new ContentDialMetaData.StringItemData();

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
                        if ((tran.name == $"{START}{clean}{TARGET}") || (tran.name == $"{START_LOWERCASE}{clean}{TARGET}"))
                        {
                            newData.m_RotationDialTargetGameObject = tran.gameObject;
                        }
                    }
                }

                newData.m_ModelGameObject = valid[i].gameObject;
                newData.m_RotationAmount = 270f;
                m_MetaData.m_DialData.Add(newData);
                GameObjectToString("Dial", ref newData.m_ModelGameObject, ref newData.m_ModelGameObjectName);
            }
        }


        private class DialItemView : DefaultListView<ContentDialMetaData.StringItemData>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public DialItemView(string name, GameObject root, CatalogueEntry entry, List<ContentDialMetaData.StringItemData> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentDialMetaData.StringItemData DrawElement(ContentDialMetaData.StringItemData renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentDialMetaData.StringItemData dataItem)
            {
                dataItem.m_ModelGameObject = StringToGameObject(dataItem.m_ModelGameObjectName, root);
                GameObjectToString("Pivot Object", ref dataItem.m_ModelGameObject, ref dataItem.m_ModelGameObjectName);
                EditorGUILayout.LabelField("Pivot Object path", dataItem.m_ModelGameObjectName);

                dataItem.m_RotationDialTargetGameObject = StringToGameObject(dataItem.m_RotationDialTargetGameObjectName, root);
                GameObjectToString("Target", ref dataItem.m_RotationDialTargetGameObject, ref dataItem.m_RotationDialTargetGameObjectName);
                dataItem.m_RotationAmount = EditorGUILayout.FloatField("RotationAmount", dataItem.m_RotationAmount);

                dataItem.m_sTitle = EditorGUILayout.TextField("Title: ", dataItem.m_sTitle);
                dataItem.m_sDescription = EditorGUILayout.TextField("Description: ", dataItem.m_sDescription);

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }


        private class PatchItemView : DefaultListView<ContentDialMetaData.PatchItemData>
        {
            private readonly CatalogueEntry m_Entry;

            public PatchItemView(string name, CatalogueEntry entry, List<ContentDialMetaData.PatchItemData> list) : base(name, list)
            {
                m_Entry = entry;
            }

            protected override ContentDialMetaData.PatchItemData DrawElement(ContentDialMetaData.PatchItemData renderItem)
            {
                DrawRenderItem(m_Entry, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, ContentDialMetaData.PatchItemData dataItem)
            {
                string sDialName;

                dataItem.m_sPatchName = EditorGUILayout.TextField("Patch Name: ", dataItem.m_sPatchName);

                foreach (ContentDialMetaData.StringItemData item in entry.GetMetaData<ContentDialMetaData>().m_DialData)
                {
                    sDialName = item.m_ModelGameObjectName;
                    sDialName = sDialName.Substring(sDialName.LastIndexOf('/') + 1);

                    if (false == dataItem.m_DialRotations.ContainsKey(sDialName))
                    {
                        dataItem.m_DialRotations.Add(sDialName, 0f);
                    }

                    EditorGUILayout.LabelField("Dial: ", sDialName);
                    dataItem.m_DialRotations[sDialName] = EditorGUILayout.FloatField(dataItem.m_DialRotations[sDialName]);
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
        }
    }
}
