using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentVideoPlayer)]
public class ContentVideoPlayerMetaData : MetaData
{
    public enum OrientationType
    {
        Portrait,
        Landscape,
    }

    public enum DisplayType
    {
        TV,
        LED,
    }

    [System.NonSerialized]
    public GameObject m_ScreenGameObject;

    [SerializeField]
    public string m_ScreenGameObjectName;

    [SerializeField]
    public OrientationType m_OrientationType = OrientationType.Landscape;

    [SerializeField]
    public DisplayType m_DisplayType = DisplayType.TV;
}