using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentKillTeam)]
public class ContentKillTeamMetaData : MetaData
{

    public enum WeaponType
    {
        Melee,
        Ranged
    }
    public class OperativeStats
    {
        [SerializeField]
        public string operativeName;
        [SerializeField]
        public int actionPointLimit;
        [SerializeField]
        public float moveInches;
        [SerializeField]
        public int save;             // 3 means 3+
        [SerializeField]
        public int maxWounds;
        [SerializeField]
        public int currentWounds;

       

        [TextArea]
        public string abilities;
    }

    public class WeaponStats
    {
        [SerializeField]
        public WeaponType weaponType;
        [SerializeField]
        public string weaponName;

        [SerializeField]
        public int attacks;
        [SerializeField]            
        public int hit;              // 3 means 3+
        [SerializeField]
        public int normalDamage;
        [SerializeField]
        public int criticalDamage;

        public string[] weaponRules;


    }

    public OperativeStats m_OperativeStats;
    public List<WeaponStats> m_Weapons;


    public ContentKillTeamMetaData()
    {
        m_OperativeStats = new OperativeStats();
        m_Weapons = new List<WeaponStats>();
    }

}