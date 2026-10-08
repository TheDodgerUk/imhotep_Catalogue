using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentAdaptiveDataView : MetaDataView<ContentAdaptiveMetaData>
    {
        public ContentAdaptiveDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            CollectData(true);
        }

        protected override void OnMetaDataGUI()
        {
            if (GUILayout.Button("View Item", GUILayout.Width(200)) == true)
            {
                TestSceneManager.EditPrefabInScene(m_RootGameObject);
            }

            m_MetaData.m_AxisChoice = (ContentAdaptiveMetaData.AxisChoice)EditorGUILayout.EnumPopup("Axis", m_MetaData.m_AxisChoice);
            CollectData(false);

        }


        private void CollectData(bool initialise)
        {

            var axisList = m_MetaData.UnPackAxisField();

            if (axisList.Count == 1)
            {
                switch (axisList[0])
                {
                    case Axis.X:
                        AssignModAxisSingle(m_MetaData.m_NonModularSingle, "Axis X", initialise);
                        break;
                    case Axis.Y:
                        AssignModAxisSingle(m_MetaData.m_NonModularSingle, "Axis Y", initialise);
                        break;
                    case Axis.Z:
                        AssignModAxisSingle(m_MetaData.m_NonModularSingle, "Axis Z", initialise);
                        break;
                    default:
                        break;
                }
            }
            if (axisList.Count == 2)
            {
                AssignModAxisDouble(m_MetaData.m_NonModularDouble, initialise);
            }
        }


        private void AssignModAxisSingle(ContentAdaptiveMetaData.ModAxisSingle modAxis, string axis, bool initialise)
        {
            AssignModAxisModular(modAxis.m_NonModuleMinus, "Left", initialise);
            AssignModAxisModular(modAxis.m_NonModulePlus, "Right", initialise);
            AssignModAxisModular(modAxis.m_NonModuleCentre, "Center", initialise);
        }


        private void AssignModAxisDouble (ContentAdaptiveMetaData.ModAxisDouble modAxis, bool initialise)
        {
            AssignModAxisModular(modAxis.m_NonModule_NW, "NW", initialise);
            AssignModAxisModular(modAxis.m_NonModule_N, "N", initialise);
            AssignModAxisModular(modAxis.m_NonModule_NE, "NE", initialise);
            AssignModAxisModular(modAxis.m_NonModule_E, "E", initialise);
            AssignModAxisModular(modAxis.m_NonModule_SE, "SE", initialise);
            AssignModAxisModular(modAxis.m_NonModule_S, "S", initialise);
            AssignModAxisModular(modAxis.m_NonModule_SW, "SW", initialise);
            AssignModAxisModular(modAxis.m_NonModule_W, "W", initialise);
            AssignModAxisModular(modAxis.m_NonModuleCentre, "Center", initialise);
        }

        private void AssignModAxisModular(ContentAdaptiveMetaData.Modular item, string itemName, bool initialise)
        {
            if(initialise == true)
            {
                if (string.IsNullOrEmpty(item.m_GameObjectName) == false)
                {
                    item.m_GameObject = StringToGameObject(item.m_GameObjectName, m_RootGameObject);
                }
            }
            else
            {
                bool plus = GameObjectToString($"           {itemName}", ref item.m_GameObject, ref item.m_GameObjectName);
                if (plus == true && item.m_GameObject != null)
                {
                    item.m_Offset = item.m_GameObject.transform.localPosition;
                }

            }
            if (item.m_GameObject != null)
            {
                var bounds = CreateColliders.GetEncapsulatingBounds(item.m_GameObject);
                item.m_ModulePartSize = bounds.size;
            }
        }


    }
}

