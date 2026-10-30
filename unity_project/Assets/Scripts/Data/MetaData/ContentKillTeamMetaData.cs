using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentKillTeam)]
public class ContentKillTeamMetaData : MetaData
{

    public enum TeamType
    {
        Kommandos,
        WolfScouts,
        Deathwatch,
        PlagueMarines,
        AngelsOfDeath,
    }
    public enum WeaponType
    {
        Melee,
        Ranged
    }
    public class OperativeStats
    {
        [SerializeField]
        public TeamType teamType;
        [SerializeField]
        public string operativeName;

        [SerializeField]
        public int numberToGenerate = 1;
        [SerializeField]
        public int actionPointLimit = 3;
        [SerializeField]
        public float moveInches =4;
        [SerializeField]
        public int save = 3;             // 3 means 3+
        [SerializeField]
        public int maxWounds = 13;
        [SerializeField]
        public int currentWounds = 13;


        [SerializeField]
        public bool rotateX = false;

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