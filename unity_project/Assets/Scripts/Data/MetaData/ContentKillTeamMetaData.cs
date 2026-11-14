using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentKillTeam)]
public class ContentKillTeamMetaData : MetaData
{

    public enum TeamType
    {
        Kommandos = 0,
        WolfScouts = 1,
        Deathwatch = 2,
        PlagueMarines = 3,
        AngelsOfDeath = 4,

        Battleclade = 5,
        BladesOfKhaine = 6,
        Blooded = 7,
        BroodBrothers = 8,
        CanoptekCircle = 9,
        CelestianInsidiants = 10,
        ChaosCult = 11,
        CorsairVoidscarred = 12,
        DeathKorps = 13,
        ElucidianStarstriders = 14,
        ExactionSquad = 15,
        ExoditeDragonMasters = 16,
        FarstalkerKinband = 17,
        FellgorRavagers = 18,
        GellerpoxInfected = 19,
        Goremongers = 20,
        HandOfTheArchon = 21,
        HearthkynSalvagers = 22,
        HernkynYaegirs = 23,
        HierotekCircle = 24,
        HunterClade = 25,
        ImperialNavyBreachers = 26,
        InquisitorialAgents = 27,
        Kasrkin = 28,
        Legionaries = 29,
        Mandrakes = 30,
        Murderwing = 31,
        NemesisClaw = 32,
        Novitiates = 33,
        Pathfinders = 34,
        PhobosStrikeTeam = 35,
        Ratlings = 36,
        Raveners = 37,
        Sanctifiers = 38,
        ScoutSquad = 39,
        SpectreSquad = 40,
        TempestusAquilon = 41,
        VespidStingwings = 42,
        VoidDancerTroupe = 43,
        Warpcoven = 44,
        WreckaKrew = 45,
        Wyrmblade = 46,
        XV26StealthBattlesuits = 47
    }
    public enum WeaponType
    {
        Melee,
        Ranged
    }
    [System.Serializable]
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
        public bool rotateX = true;

        [TextArea]
        public string abilities;
    }

    [System.Serializable]
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

    public List<ContentKillTeamAPMetaData.ApType> m_ApList;
    public ContentKillTeamMetaData()
    {
        m_OperativeStats = new OperativeStats();
        m_Weapons = new List<WeaponStats>();
        m_ApList = new List<ContentKillTeamAPMetaData.ApType>();
    }

}