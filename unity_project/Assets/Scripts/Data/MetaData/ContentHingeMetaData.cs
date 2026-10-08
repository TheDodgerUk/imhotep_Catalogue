using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentHinge)]
public class ContentHingeMetaData : MetaData
{

    public class Spring
    {
        [SerializeField]
        public float m_Spring;
        [SerializeField]
        public float m_Damper;
        [SerializeField]
        public float m_TargetPosition;
    }

    public class HingeData
    {
        public float m_AngleMin = 0;
        public float m_AngleMax = 0;

        [System.NonSerialized]
        public GameObject m_HingeGameObject;
        [SerializeField]
        public string m_HingeGameObjectName = "";


        [SerializeField]
        public bool m_IsConnectedToRoot = true;
        [System.NonSerialized]
        public GameObject m_ConnectedGameObject;
        [SerializeField]
        public string m_ConnectedGameObjectName = "";

        [SerializeField]
        public Vector3 m_Axis = Vector3.forward;

        [SerializeField]
        public bool m_UseSpring;

        public Spring m_SpringData = new Spring();


    }

    public List<HingeData> m_HingeData = new List<HingeData>();


    public override void CollectAllData(GameObject root) 
    {
        if (root == null)
        {
            Debug.LogError("Root is null");
            return;
        }
        base.CollectAllData(root);
        foreach (var item in m_HingeData)
        {
            item.m_HingeGameObject = StringToGameObject(root, item.m_HingeGameObjectName);
            item.m_ConnectedGameObject = StringToGameObject(root, item.m_ConnectedGameObjectName);
        }
    }

}