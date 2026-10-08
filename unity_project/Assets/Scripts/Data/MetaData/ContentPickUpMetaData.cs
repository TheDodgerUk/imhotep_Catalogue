using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContentButtonMetaData;

[System.Serializable, MetaData(MetaDataType.ContentPickUp)]
public class ContentPickUpMetaData : MetaData
{

    public float m_WeightKG = 1f;
    public float m_ThrowPower = 1f;


    [SerializeField]
    public List<ColliderGameObjectData> m_IgnoreColliders = new List<ColliderGameObjectData>();
    public override void CollectAllData(GameObject root) 
    {
        foreach (var data in m_IgnoreColliders)
        {
            data.CollectAllData(root);
        }
    }

}