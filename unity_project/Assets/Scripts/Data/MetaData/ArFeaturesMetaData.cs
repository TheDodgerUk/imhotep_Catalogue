using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ArFeatures)]
public class ArFeaturesMetaData : MetaData
{
    [SerializeField]
    public bool m_bSupportsExplosion = false;

    [SerializeField]
    public bool m_bSupportsXRay = false;

    [SerializeField]
    public bool m_bSupportsInteraction = false;

    [SerializeField]
    public bool m_bSupportsInstruction = false;

    [SerializeField]
    public bool m_bSupportsArGuide = false;

    [SerializeField]
    public bool m_bSupportsPatches = false;

    [SerializeField]
    public bool m_bHasInfo = false;

    [SerializeField]
    public List<string> m_XrayObjectNames = new List<string>();

    public List<GameObject> m_XrayGameObjects = new List<GameObject>();
}