using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentFood)]
public class ContentFoodMetaData : ContentPickUpMetaData
{
    public class FoodParts
    {
        [SerializeField]
        public float m_Weight = 1;

        [SerializeField]
        public GameObjectData m_FoodPart = new GameObjectData();

        [System.NonSerialized]
        public AudioSource m_AudioSource;

        [System.NonSerialized]
        public List<Renderer> m_Renderer = new List<Renderer>();

        [System.NonSerialized]
        public List<Collider> m_Collider = new List<Collider>();
    }

    public List<FoodParts> m_FoodParts = new List<FoodParts>();

    public override void CollectAllData(GameObject root) 
    {
        if (root == null)
        {
            Debug.LogError("Root is null");
            return;
        }
        base.CollectAllData(root);
        if (root != null)
        {
            foreach (var item in m_FoodParts)
            {
                item.m_FoodPart.CollectAllData(root);
                if (item.m_FoodPart.GameObjectRef != null)
                {
                    item.m_AudioSource = item.m_FoodPart.GameObjectRef.GetComponent<AudioSource>();
                    item.m_Renderer = item.m_FoodPart.GameObjectRef.GetComponentsInChildren<Renderer>().ToList();
                    item.m_Collider = item.m_FoodPart.GameObjectRef.GetComponentsInChildren<Collider>().ToList();
                }
                else
                {
                    Debug.LogError($"Cannot find item {root.GetGameObjectPath()}", root.gameObject);
                }
            }
        }
    }

}