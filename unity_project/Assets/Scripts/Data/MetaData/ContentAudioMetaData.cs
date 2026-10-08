using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentAudio)]
public class ContentAudioMetaData : MetaData
{
    public class AudioSubGroupData
    {
        [System.NonSerialized]
        public AudioSource m_AudioSource;

        [SerializeField]
        public string m_AudioSourceName = "";
    }

    public class AudioGroupData
    {
        [System.NonSerialized]
        public GameObject m_BaseSoundGameObject;

        [SerializeField]
        public string m_BaseSoundGameObjectName = "";


        [System.NonSerialized]
        public Collider m_SoundCollider;

        [SerializeField]
        public string m_SoundColliderName = "";

        public List<AudioSubGroupData> m_AudioSubGroupData = new List<AudioSubGroupData>();
    }

    public List<AudioGroupData> m_AudioDatas = new List<AudioGroupData>();
}