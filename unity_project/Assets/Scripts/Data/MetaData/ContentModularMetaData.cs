using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentModular)]
public class ContentModularMetaData : MetaData
{

    public class ModularData
    {
        [System.NonSerialized]
        public GameObject m_ConnectionPointGameObject;

        [SerializeField]
        public string m_ConnectionPointGameObjectName = "";

        [System.NonSerialized]
        public GameObject m_ItemsRootGameObject;

        [SerializeField]
        public string m_ItemsRootGameObjectName = "";

        [System.NonSerialized]
        public List<Transform> m_ItemsChildrenGameObject = new List<Transform>();

        [SerializeField]
        public int m_SelectedChildIndex = 0;
    }


    public List<ModularData> m_Modular = new List<ModularData>();
}