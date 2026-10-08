using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentStaging)]
public class ContentStagingMetaData : MetaData
{
    public enum ConectionType
    {
        Ladder = 2,
        Triangle = 3,
        Square = 4,
        Rectangle = 12,
    }

    [SerializeField]
    public class StagingData : InstructionData
    {
        [SerializeField]
        public int m_OwnIndex = 0;

        [SerializeField]
        public int m_RotationIndex = 0;

        [System.NonSerialized]
        public Transform m_OriginalParent;
        [System.NonSerialized]
        public Quaternion m_OriginalQuaternion;
        [System.NonSerialized]
        public Vector3 m_OriginalPosition;
        [System.NonSerialized]
        public StagingData m_ConnectedTo;
    }

    [SerializeField]
    public int m_CurrentPoint = 0;



    [SerializeField]
    public List<StagingData> m_StagingData = new List<StagingData>();

    [SerializeField]
    public ConectionType m_ConectionType = ConectionType.Triangle;

    public override void CollectAllData(GameObject root)
    {
        if (root != null)
        {
            for (int i = 0; i < m_StagingData.Count; i++)
            {
                StagingData item = m_StagingData[i];
                item.m_ModelGameObject = StringToGameObject(root, item.m_ModelGameObjectName);
                if (item.m_ModelGameObject != null)
                {
                    item.m_OriginalParent = item.m_ModelGameObject.transform.parent;

                    item.m_OriginalQuaternion = item.m_ModelGameObject.transform.localRotation;
                    item.m_OriginalPosition = item.m_ModelGameObject.transform.localPosition;
                }
                item.m_OwnIndex = i;
            }
        }
    }

}