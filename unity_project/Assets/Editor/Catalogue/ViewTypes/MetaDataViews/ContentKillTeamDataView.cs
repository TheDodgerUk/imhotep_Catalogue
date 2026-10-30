using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;


namespace EditorTools.CatalogueViewer
{
    public class ContentKillTeamDataView : MetaDataView<ContentKillTeamMetaData>
    {

        private WeaponItemView m_WeaponItemView;
        public ContentKillTeamDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_WeaponItemView = new WeaponItemView("Items", m_RootGameObject, m_Entry, m_MetaData.m_Weapons);
            m_WeaponItemView.ToggleFoldout(false);

            ////for (int i = 0; i < m_MetaData.weapons.Count; i++)
            ////{
            ////    m_MetaData.weapons[i].AnimationGameObject = StringToGameObject(m_MetaData.m_AnimationData[i].AnimationGameObjectName, m_RootGameObject);
            ////}
        }
        protected override void OnMetaDataGUI()
        {
            m_MetaData.m_OperativeStats.teamType = (ContentKillTeamMetaData.TeamType)EditorGUILayout.EnumFlagsField("teamType", m_MetaData.m_OperativeStats.teamType);
            m_MetaData.m_OperativeStats.operativeName = EditorGUILayout.TextField("Name", m_MetaData.m_OperativeStats.operativeName);
            m_MetaData.m_OperativeStats.numberToGenerate = EditorGUILayout.IntField("numberToGenerate", m_MetaData.m_OperativeStats.numberToGenerate);
            m_MetaData.m_OperativeStats.actionPointLimit = EditorGUILayout.IntField("actionPointLimit", m_MetaData.m_OperativeStats.actionPointLimit);
            m_MetaData.m_OperativeStats.moveInches = EditorGUILayout.FloatField("moveInches", m_MetaData.m_OperativeStats.moveInches);
            m_MetaData.m_OperativeStats.save = EditorGUILayout.IntField("save", m_MetaData.m_OperativeStats.save);
            m_MetaData.m_OperativeStats.maxWounds = EditorGUILayout.IntField("maxWounds", m_MetaData.m_OperativeStats.maxWounds);
            m_MetaData.m_OperativeStats.currentWounds = EditorGUILayout.IntField("currentWounds", m_MetaData.m_OperativeStats.currentWounds);
            m_MetaData.m_OperativeStats.rotateX = EditorGUILayout.Toggle("rotateX", m_MetaData.m_OperativeStats.rotateX);
            m_WeaponItemView.OnGUI();
        }

        private class WeaponItemView : DefaultListView<ContentKillTeamMetaData.WeaponStats>
        {
            private readonly CatalogueEntry m_Entry;
            private GameObject m_RootObject;

            public WeaponItemView(string name, GameObject root, CatalogueEntry entry, List<ContentKillTeamMetaData.WeaponStats> list) : base(name, list)
            {
                m_Entry = entry;
                m_RootObject = root;
            }

            protected override ContentKillTeamMetaData.WeaponStats DrawElement(ContentKillTeamMetaData.WeaponStats renderItem)
            {
                DrawRenderItem(m_Entry, m_RootObject, renderItem);
                return renderItem;
            }

            public static void DrawRenderItem(CatalogueEntry entry, GameObject root, ContentKillTeamMetaData.WeaponStats weaponItem)
            {

                
                {
                    weaponItem.weaponName = EditorGUILayout.TextField($"Weapon Name", weaponItem.weaponName);
                    weaponItem.weaponType = (ContentKillTeamMetaData.WeaponType)EditorGUILayout.EnumPopup($"Weapon Type", weaponItem.weaponType);
                    weaponItem.attacks = EditorGUILayout.IntField($"Attacks", weaponItem.attacks);
                    weaponItem.hit = EditorGUILayout.IntField($"Hit", weaponItem.hit);
                    weaponItem.normalDamage = EditorGUILayout.IntField($"Normal Damage ", weaponItem.normalDamage);
                    weaponItem.criticalDamage = EditorGUILayout.IntField($"Critical Damage", weaponItem.criticalDamage);
                }
                EditorGUILayout.Space();

            }
        }

    }
}

