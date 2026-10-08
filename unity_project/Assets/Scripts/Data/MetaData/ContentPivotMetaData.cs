using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentPivot)]
public class ContentPivotMetaData : MetaData
{
    public class StringItemData
    {
        public enum PivotState
        {
            ToOpen,
            ToClose,
        }

        [System.NonSerialized]
        public PivotState m_PivotState = PivotState.ToOpen;

        [System.NonSerialized]
        public bool m_PivotInteractionsAllowed = true;

        [System.NonSerialized]
        public GameObject m_ModelPivotGameObject;

        [System.NonSerialized]
        public Collider m_ModelPivotCollider;

        [System.NonSerialized]
        public Vector3 m_ModelPivotRotation;

        [SerializeField]
        public string m_ModelPivotName = "";

        [SerializeField]
        public float m_Min = 0;

        [SerializeField]
        public float m_Max = 90;

        [SerializeField]
        public Axis m_Axis = Axis.Y;

        [SerializeField]
        public float m_Time = 2;
    }


    public List<StringItemData> m_PivotData = new List<StringItemData>();
}