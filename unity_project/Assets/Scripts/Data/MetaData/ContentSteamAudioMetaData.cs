using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentSteamAudio)]
public class ContentSteamAudioMetaData : MetaData
{
    public enum SoundEnumType
    {
        None,
        Create,
        Modify,
        Play,       
    }

    public static readonly MaterialValue[]  Values = new MaterialValue[]
        {
            new MaterialValue(0.10f, 0.20f, 0.30f, 0.05f, 0.100f, 0.050f, 0.030f),
            new MaterialValue(0.03f, 0.04f, 0.07f, 0.05f, 0.015f, 0.015f, 0.015f),
            new MaterialValue(0.05f, 0.07f, 0.08f, 0.05f, 0.015f, 0.002f, 0.001f),
            new MaterialValue(0.01f, 0.02f, 0.02f, 0.05f, 0.060f, 0.044f, 0.011f),
            new MaterialValue(0.60f, 0.70f, 0.80f, 0.05f, 0.031f, 0.012f, 0.008f),
            new MaterialValue(0.24f, 0.69f, 0.73f, 0.05f, 0.020f, 0.005f, 0.003f),
            new MaterialValue(0.06f, 0.03f, 0.02f, 0.05f, 0.060f, 0.044f, 0.011f),
            new MaterialValue(0.12f, 0.06f, 0.04f, 0.05f, 0.056f, 0.056f, 0.004f),
            new MaterialValue(0.11f, 0.07f, 0.06f, 0.05f, 0.070f, 0.014f, 0.005f),
            new MaterialValue(0.20f, 0.07f, 0.06f, 0.05f, 0.200f, 0.025f, 0.010f),
            new MaterialValue(0.13f, 0.20f, 0.24f, 0.05f, 0.015f, 0.002f, 0.001f),
            new MaterialValue()
        };

    [SerializeField]
    public enum MaterialPreset
    {
        Generic,
        Brick,
        Concrete,
        Ceramic,
        Gravel,
        Carpet,
        Glass,
        Plaster,
        Wood,
        Metal,
        Rock,
        Custom
    }

    public class MaterialValue
    {
        [SerializeField]
        public float LowFreqAbsorption;
        [SerializeField]
        public float MidFreqAbsorption;
        [SerializeField]
        public float HighFreqAbsorption;

        // Scattering coefficients.
        [SerializeField]
        public float Scattering;

        // Scattering coefficients.
        [SerializeField]
        public float LowFreqTransmission;
        [SerializeField]
        public float MidFreqTransmission;
        [SerializeField]
        public float HighFreqTransmission;

        public MaterialValue(float aLow, float aMid, float aHigh, float scattering, float tLow, float tMid, float tHigh)
        {
            LowFreqAbsorption = aLow;
            MidFreqAbsorption = aMid;
            HighFreqAbsorption = aHigh;

            Scattering = scattering;

            LowFreqTransmission = tLow;
            MidFreqTransmission = tMid;
            HighFreqTransmission = tHigh;
        }
        public MaterialValue()
        {
        }
    }

    [SerializeField]
    public SoundEnumType m_SoundEnumType = SoundEnumType.None;

    [System.NonSerialized]
    public GameObject m_SoundGameObject;

    [SerializeField]
    public string m_SoundGameObjectName = GlobalConsts.RootModel;


    [System.NonSerialized]
    public GameObject m_SplGameObject;

    [SerializeField]
    public string m_SplGameObjectName = "";


    [SerializeField]
    public MaterialPreset m_MaterialPreset;

    [SerializeField]
    public MaterialValue m_MaterialValue = new MaterialValue();

    [SerializeField]
    public bool m_ExportAllChildren = true;

    public override void CollectAllData(GameObject root)
    {
        if (root != null)
        {
            m_SoundGameObject = StringToGameObject(root, m_SoundGameObjectName);
            m_SplGameObject = StringToGameObject(root, m_SplGameObjectName, false);
        }
    }
}