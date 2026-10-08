using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentSlider)]
public class ContentSliderMetaData : MetaData
{
    [System.Serializable]
    public class ItemData : InstructionData
    {
        [SerializeField]
        public enum MoveType
        {
            GameObject,
            Amount,
        }

        [SerializeField]
        public MoveType m_MoveType = MoveType.GameObject;

        [System.NonSerialized]
        public GameObject m_ModelEndLocalPositionGameObject;

        [SerializeField]
        public string m_ModelEndLocalPositionGameObjectName;

        [SerializeField]
        public float m_ModelEndLocalPositionfloat = 0.0015f;
    }

    [SerializeField]
    public List<ItemData> m_ItemData = new List<ItemData>();

    public override void CollectAllData(GameObject root)
    {
        if (root != null)
        {
            foreach (var item in m_ItemData)
            {
                item.m_ModelGameObject = StringToGameObject(root, item.m_ModelGameObjectName);
                if (string.IsNullOrEmpty(item.m_ModelEndLocalPositionGameObjectName) == false)
                {
                    item.m_ModelEndLocalPositionGameObject = StringToGameObject(root, item.m_ModelEndLocalPositionGameObjectName);
                }
            }
        }
    }


}