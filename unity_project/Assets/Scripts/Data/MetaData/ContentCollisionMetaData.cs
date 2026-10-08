using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentCollision)]
public class ContentCollisionMetaData : MetaData
{
    public class CollisionData
    {
        public GameObjectData m_Colliders = new GameObjectData();
        public List<string> m_ValidGuids = new List<string>();
    }

    public List<CollisionData> m_CollisionData = new List<CollisionData>();

    public override void CollectAllData(GameObject root) 
    {
        foreach (var item in m_CollisionData)
        {
            item.m_Colliders.CollectAllData(root);
        }
    }

}