using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentScript)]
public class ContentScriptMetaData : MetaData
{
    public enum AddScript
    {
        AnalogueClock,
        DigitalClock,
        DeskFan,
        NurfGun,
        SuperSoaker,
        FireExtinguisher,
        EnvironmentSpecials,
    }

    [SerializeField]
    public AddScript m_AddScript = AddScript.AnalogueClock;

#if !CATALOG_PROGRAM
    public void AddItem(GameObject root)
    {
        switch (m_AddScript)
        {
            case AddScript.AnalogueClock:
                root.ForceComponent<AnalogueClock>();
                break;

            case AddScript.DigitalClock:
                root.ForceComponent<DigitalClock>();
                break;
            default:
                break;
        }
    }
#endif
}