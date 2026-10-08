using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
#if CATALOG_PROGRAM
using UnityEditor;
using UnityEngine.SceneManagement;
#endif
#endif

namespace Museum
{
    public enum CachedObject
    {
        Museum_Plinth_Large,
        Museum_Plinth_Medium,
        Museum_Plinth_Small,
    };

    public class PlinthConsts
    {
        public const string CATALOGE_DISPLAY_POINT = "DisplayPoint";
    }

    public class MuseumFloorDataContainer
    {
        private List<MuseumFloorData> m_MuseumFloorDataList = new List<MuseumFloorData>();
        public MuseumFloorData GetData(string name) { return m_MuseumFloorDataList.Find(x => x.m_Name.Equals(name)); }
        public void Add(MuseumFloorData plinthDataRef) { m_MuseumFloorDataList.Add(plinthDataRef); }
        public int GetCount() { return m_MuseumFloorDataList.Count; }
        public MuseumFloorData GetAt(int index) { return m_MuseumFloorDataList[index]; }
        public void Remove(MuseumFloorData plinthDataRef) { m_MuseumFloorDataList.Remove(plinthDataRef); }
    }


    public class MuseumFloorData
    {
        [SerializeField]
        public string m_Name;

        [SerializeField]
        public string m_Enviroment;

        [SerializeField]
        public Color m_EmissiveColour = Color.red;

        [SerializeField]
        public List<string> m_LiftVideos = new List<string>();

#if UNITY_EDITOR
#if CATALOG_PROGRAM
        [System.NonSerialized]
        public List<SceneAsset> m_OptionalScenesList = new List<SceneAsset>();
#endif
#endif
        [SerializeField]
        public List<string> m_OptionalScenesListNames = new List<string>();

        [SerializeField]
        public List<PlinthItemData> m_ItemDataList = new List<PlinthItemData>();
    }


    public class PlinthItemData
    {
        [SerializeField]
        public CachedObject m_PlinthType;

        [SerializeField]
        public Vector3 m_Position;

        [SerializeField]
        public Quaternion m_Rotation;

        [SerializeField]
        public string m_ProductShortName;
    }
}

