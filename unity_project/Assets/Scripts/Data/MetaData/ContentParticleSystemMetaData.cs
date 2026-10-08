using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[System.Serializable, MetaData(MetaDataType.ContentParticleSystem)]
public class ContentParticleSystemMetaData : MetaData
{

    [System.NonSerialized]
    public VisualEffect m_ParticleSystemVisualEffect;

    [System.NonSerialized]
    public GameObject m_ParticleSystemGameObject;

    [SerializeField]
    public string m_ParticleSystemName = "";
}