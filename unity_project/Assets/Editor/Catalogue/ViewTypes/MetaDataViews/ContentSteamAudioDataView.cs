using System;
using EditorTools.Views;
using EnvironmentHelpers;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;
using static ContentAudioMetaData;

namespace EditorTools.CatalogueViewer
{
    public class ContentSteamAudioDataView : MetaDataView<ContentSteamAudioMetaData>
    {

        public ContentSteamAudioDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {

            if (m_RootGameObject != null)
            {
                m_MetaData.m_SoundGameObject = StringToGameObject(m_MetaData.m_SoundGameObjectName, m_RootGameObject);
                m_MetaData.m_SplGameObject = StringToGameObject(m_MetaData.m_SplGameObjectName, m_RootGameObject);
            }
        }

        protected override void OnMetaDataGUI()
        {
            m_MetaData.m_SoundEnumType = (ContentSteamAudioMetaData.SoundEnumType)EditorGUILayout.EnumPopup("SoundType", m_MetaData.m_SoundEnumType);
            EditorGUILayout.Space();
            GameObjectToString("BaseSoundGameObject", ref m_MetaData.m_SoundGameObject, ref m_MetaData.m_SoundGameObjectName);
            EditorGUILayout.LabelField("BaseSoundGameObject path", m_MetaData.m_SoundGameObjectName);

            EditorGUILayout.Space();
            GameObjectToString("SplSoundGameObject", ref m_MetaData.m_SplGameObject, ref m_MetaData.m_SplGameObjectName);
            EditorGUILayout.Space();
            var before = m_MetaData.m_MaterialPreset;
            m_MetaData.m_MaterialPreset = (ContentSteamAudioMetaData.MaterialPreset)EditorGUILayout.EnumPopup("MaterialPreset", m_MetaData.m_MaterialPreset);
            m_MetaData.m_ExportAllChildren = EditorGUILayout.Toggle("ExportAllChildren", m_MetaData.m_ExportAllChildren);

            EditorGUILayout.Space();
            if (m_MetaData.m_MaterialPreset == ContentSteamAudioMetaData.MaterialPreset.Custom)
            {
                if(before != m_MetaData.m_MaterialPreset)
                {
                    CopyFrom(ContentSteamAudioMetaData.Values[(int)before]);
                }

                m_MetaData.m_MaterialValue.LowFreqAbsorption = EditorGUILayout.Slider("LowFreqAbsorption  ", m_MetaData.m_MaterialValue.LowFreqAbsorption, 0, 1);
                m_MetaData.m_MaterialValue.MidFreqAbsorption = EditorGUILayout.Slider("MidFreqAbsorption  ", m_MetaData.m_MaterialValue.MidFreqAbsorption, 0, 1);
                m_MetaData.m_MaterialValue.HighFreqAbsorption = EditorGUILayout.Slider("HighFreqAbsorption  ", m_MetaData.m_MaterialValue.HighFreqAbsorption, 0, 1);

                m_MetaData.m_MaterialValue.Scattering = EditorGUILayout.Slider("Scattering  ", m_MetaData.m_MaterialValue.Scattering, 0, 1);

                m_MetaData.m_MaterialValue.LowFreqTransmission = EditorGUILayout.Slider("LowFreqTransmission  ", m_MetaData.m_MaterialValue.LowFreqTransmission, 0, 1);
                m_MetaData.m_MaterialValue.MidFreqTransmission = EditorGUILayout.Slider("HighFreqAbsorption  ", m_MetaData.m_MaterialValue.MidFreqTransmission, 0, 1);
                m_MetaData.m_MaterialValue.HighFreqTransmission = EditorGUILayout.Slider("HighFreqAbsorption  ", m_MetaData.m_MaterialValue.HighFreqTransmission, 0, 1);
            }
        }

        public void CopyFrom(ContentSteamAudioMetaData.MaterialValue other)
        {
            m_MetaData.m_MaterialValue.LowFreqAbsorption = other.LowFreqAbsorption;
            m_MetaData.m_MaterialValue.MidFreqAbsorption = other.MidFreqAbsorption;
            m_MetaData.m_MaterialValue.HighFreqAbsorption = other.HighFreqAbsorption;

            m_MetaData.m_MaterialValue.Scattering = other.Scattering;

            m_MetaData.m_MaterialValue.LowFreqTransmission = other.LowFreqTransmission;
            m_MetaData.m_MaterialValue.MidFreqTransmission = other.MidFreqTransmission;
            m_MetaData.m_MaterialValue.HighFreqTransmission = other.HighFreqTransmission;
        }

    }

}

