
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentPickUpSocket)]
public class ContentPickUpSocketMetaData : ContentPickUpMetaData
{
    public enum SocketType
    {
        OneOnly,
        FindByName,
    }

    public enum SocketObjectType
    {
        SubObject,
        Root,
    }

    public class SocketData
    {
#if !CATALOG_PROGRAM
        [System.NonSerialized]
        public TransformData m_OriginalData;
#endif

        [System.NonSerialized]
        public Rigidbody m_GameObjectPickUpRigidbody;

        [SerializeField]
        public float m_Weight = 1f;

        [SerializeField]
        public bool m_UseRoot = false;

        [System.NonSerialized]
        public GameObject m_GameObjectPickup;
        [SerializeField]
        public string m_GameObjectPickUpName;

        [System.NonSerialized]
        public GameObject m_GameObjectPickUpSocket;
        [SerializeField]
        public string m_GameObjectPickUpSocketName;

        [System.NonSerialized]
        public GameObject m_GameObjectSocket;
        [SerializeField]
        public string m_GameObjectSocketName;

        [SerializeField]
        public SocketType m_SocketType = SocketType.OneOnly;

        [SerializeField]
        public SocketObjectType m_SocketObjectType = SocketObjectType.SubObject;

        [SerializeField]
        public bool IsSocketed = false;
    }

    public List<SocketData> m_Data = new List<SocketData>();


    public override void CollectAllData(GameObject root)
    {
        if (root == null)
        {
            Debug.LogError("Root is null");
            return;
        }
        base.CollectAllData(root);
        for (int i = 0; i < m_Data.Count; i++)
        {
            if (m_Data[i].m_SocketType == ContentPickUpSocketMetaData.SocketType.OneOnly)
            {
                m_Data[i].m_GameObjectSocket = StringToGameObject(root, m_Data[i].m_GameObjectSocketName, false);
            }


            if (m_Data[i].m_UseRoot == true)
            {
                m_Data[i].m_GameObjectPickup = root;
            }
            else
            {
                m_Data[i].m_GameObjectPickup = StringToGameObject(root, m_Data[i].m_GameObjectPickUpName);
            }

            m_Data[i].m_GameObjectPickUpSocket = StringToGameObject(root, m_Data[i].m_GameObjectPickUpSocketName);
            if (m_Data[i].m_SocketType == ContentPickUpSocketMetaData.SocketType.OneOnly)
            {
                m_Data[i].m_GameObjectSocket = StringToGameObject(root, m_Data[i].m_GameObjectSocketName, false);
            }

#if !CATALOG_PROGRAM
            m_Data[i].m_OriginalData = m_Data[i].m_GameObjectPickup.GetTransformData();
            int index = i;
            Core.Mono.WaitUntil(3, () => m_Data[index].m_GameObjectPickup.GetComponent<Rigidbody>() != null, () =>
            {
                m_Data[index].m_GameObjectPickUpRigidbody = m_Data[index].m_GameObjectPickup.GetComponent<Rigidbody>();
            });
#endif
        }
    }
}

