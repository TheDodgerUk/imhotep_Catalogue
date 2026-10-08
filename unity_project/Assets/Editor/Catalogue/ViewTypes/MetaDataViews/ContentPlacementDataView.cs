using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentPlacementDataView : MetaDataView<ContentPlacementMetaData>
    {
        private ContentSnappingItemsView m_SnappingItemsView;

        public ContentPlacementDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews) 
        {
            m_SnappingItemsView = new ContentSnappingItemsView("SnappingItemsView", catalogue, m_Entry, m_MetaData.m_SnappingItemData);
            m_SnappingItemsView.ToggleFoldout(false);
            if (m_RootGameObject != null)
            {
                m_MetaData.m_ExtraPivotPointGameObject = StringToGameObject(m_MetaData.m_ExtraPivotPoint, m_RootGameObject);
                m_MetaData.m_OutlineOverrideGameObject = StringToGameObject(m_MetaData.m_OutlineOverrideGameObjectName, m_RootGameObject);
            }
        }

        protected override void OnMetaDataGUI()
        {
            m_MetaData.Placement = (ContentPlacementMetaData.EnumPlacement)EditorGUILayout.EnumPopup("Placement", m_MetaData.Placement);
            m_MetaData.Method = (ContentPlacementMetaData.EnumMethod)EditorGUILayout.EnumPopup("Method", m_MetaData.Method);
            m_MetaData.m_MainColliderEnabledInVR = EditorGUILayout.Toggle("Main collider in VR", m_MetaData.m_MainColliderEnabledInVR);
            m_MetaData.m_UseAllMainColliders = EditorGUILayout.Toggle("Use Other MainCollider, when moving", m_MetaData.m_UseAllMainColliders);

            m_MetaData.m_SnappingDistance = EditorGUILayout.FloatField("SnappingDistance", m_MetaData.m_SnappingDistance);
            m_MetaData.m_SnapToOthers = EditorGUILayout.Toggle("SnapToOthers", m_MetaData.m_SnapToOthers);
            EditorGUILayout.Space();

            int oldPointEnumField = m_MetaData.m_SnappingPointEnumField;
            if (m_MetaData.Placement != ContentPlacementMetaData.EnumPlacement.WallHanging)
            {
                if (m_MetaData.Placement.IsWall())
                {
                    m_MetaData.m_SnappingPointEnumField = EditorGUILayout.MaskField("SnappingPointEnum", m_MetaData.m_SnappingPointEnumField, System.Enum.GetNames(typeof(ContentPlacementMetaData.SnappingPointEnumWall)));
                }
                else
                {
                    m_MetaData.m_SnappingPointEnumField = EditorGUILayout.MaskField("SnappingPointEnum", m_MetaData.m_SnappingPointEnumField, System.Enum.GetNames(typeof(ContentPlacementMetaData.SnappingPointEnumFloor)));
                }
            }
            else
            {
                m_MetaData.m_SnappingPointEnumField = 0;
            }

            if(oldPointEnumField != m_MetaData.m_SnappingPointEnumField)
            {
                var items = m_MetaData.UnPackSnappingPointField();
                foreach(var item in items)
                {
                    Debug.Log($"m_MetaData.m_SnappingPointEnumField is {item}");
                }
            }

            EditorGUILayout.Space();

            m_MetaData.m_SnapToOnlyTheseOthers = EditorGUILayout.Toggle("SnapToOnlyTheseOthers", m_MetaData.m_SnapToOnlyTheseOthers);
            if (m_MetaData.m_SnapToOnlyTheseOthers == true)
            {
                m_SnappingItemsView.OnGUI();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GameObjectToString("Extra PivotPoint", ref m_MetaData.m_ExtraPivotPointGameObject, ref m_MetaData.m_ExtraPivotPoint);
            m_MetaData.m_ExtraPivotPointAxis = (Axis)EditorGUILayout.EnumPopup("ExtraPivotPointAxis", m_MetaData.m_ExtraPivotPointAxis); 
            EditorGUILayout.Space();
            GameObjectToString("m_OutlineOverride", ref m_MetaData.m_OutlineOverrideGameObject, ref m_MetaData.m_OutlineOverrideGameObjectName);

        }
    }


    class ContentSnappingItemsView : DefaultListView<ContentPlacementMetaData.SnappingItemData>
    {
        private readonly CatalogueEntry m_Entry;
        private readonly Catalogue m_Catalogue;

        public ContentSnappingItemsView(string name, Catalogue cat, CatalogueEntry entry, List<ContentPlacementMetaData.SnappingItemData> list) : base(name, list)
        {
            m_Catalogue = cat;
            m_Entry = entry;
        }

        protected override ContentPlacementMetaData.SnappingItemData DrawElement(ContentPlacementMetaData.SnappingItemData renderItem)
        {
            DrawRenderItem(m_Catalogue, m_Entry, renderItem);
            return renderItem;
        }

        public static void DrawRenderItem(Catalogue cat, CatalogueEntry entry, ContentPlacementMetaData.SnappingItemData dataItem)
        {

            dataItem.m_GuidsToSnap = EditorGUILayout.TextField("m_GuidsToSnap", dataItem.m_GuidsToSnap);
            var newEntry = cat.Entries.FindLast(e => e.Guid == dataItem.m_GuidsToSnap);
            if (newEntry != null)
            {
                EditorGUILayout.LabelField($"{newEntry.ShortName}");
            }
            EditorGUILayout.Space();

        }
    }

}

