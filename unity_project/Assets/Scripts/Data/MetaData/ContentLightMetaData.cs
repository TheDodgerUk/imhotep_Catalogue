using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentLight)]
public class ContentLightMetaData : MetaData
{
    public enum LightEnumType
    {
        Spot,
        Point
    }

    [SerializeField]
    public Vector2 m_Range = new Vector2(1, 3);

    [SerializeField]
    public Vector2 m_Intensity = new Vector2(1, 3);

    [SerializeField]
    public Vector2 m_Angle = new Vector2(0, 90);

    [SerializeField]
    public LightEnumType m_LightEnumType = LightEnumType.Spot;

    [SerializeField]
    public bool m_CanEdit = true;

    public class EmissiveData
    {
        [System.NonSerialized]
        public GameObject m_GameObject;

        [SerializeField]
        public string m_GameObjectName;

        [SerializeField]
        public int m_MaterialIndex = 0;
    }


    public List<EmissiveData> m_EmissiveData = new List<EmissiveData>();

}