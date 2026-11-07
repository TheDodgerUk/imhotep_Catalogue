using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentKillTeamAP)]
public class ContentKillTeamAPMetaData : MetaData
{
    public enum ApType
    {
        Move,
        Shoot,
        Melee
    }

    public ApType apType;

    [SerializeField]
    public ContentKillTeamMetaData.TeamType KillTeamType;
}