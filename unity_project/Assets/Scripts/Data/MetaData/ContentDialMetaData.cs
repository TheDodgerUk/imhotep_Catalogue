using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentDial)]
public class ContentDialMetaData : MetaData
{
    [System.Serializable]
    public class StringItemData : InstructionData
    {
        [System.NonSerialized]
        public GameObject m_RotationDialTargetGameObject;

        [SerializeField]
        public string m_RotationDialTargetGameObjectName = "";

        [SerializeField]
        public float m_RotationAmount;
    }

    [System.Serializable]
    public class PatchItemData
    {
        [SerializeField]
        public string m_sPatchName;

        [SerializeField]
        public Dictionary<string, float> m_DialRotations = new Dictionary<string, float>();
    }

    [SerializeField]
    public List<StringItemData> m_DialData = new List<StringItemData>();

    [SerializeField]
    public List<PatchItemData> m_PatchData = new List<PatchItemData>();

    public override void CollectAllData(GameObject root)
    {
        if (root != null)
        {
            foreach (StringItemData item in m_DialData)
            {
                item.m_ModelGameObject = StringToGameObject(root, item.m_ModelGameObjectName);
                if (string.IsNullOrEmpty(item.m_RotationDialTargetGameObjectName) == false)
                {
                    item.m_RotationDialTargetGameObject = StringToGameObject(root, item.m_RotationDialTargetGameObjectName);
                }
            }
        }
    }
}