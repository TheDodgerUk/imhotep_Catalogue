using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentDrink)]
public class ContentDrinkMetaData : ContentPickUpMetaData
{

    public float m_LiquidMin = 0;
    public float m_LiquidMax = 0;

    [System.NonSerialized]
    public GameObject m_LiquidWobbleGameObject;
    [SerializeField]
    public string m_LiquidWobbleGameObjectName;

    [System.NonSerialized]
    public GameObject m_LiquidPouringGameObject;
    [SerializeField]
    public string m_LiquidPouringGameObjectName;

    public override void CollectAllData(GameObject root) 
    {
        if (root == null)
        {
            Debug.LogError("Root is null");
            return;
        }
        base.CollectAllData(root);
        m_LiquidWobbleGameObject = StringToGameObject(root,m_LiquidWobbleGameObjectName);
        m_LiquidPouringGameObject = StringToGameObject(root,m_LiquidPouringGameObjectName);

#if !CATALOG_PROGRAM
        Core.AssetsLocalRef.VisualEffectLocalRef.GetItem(Core.Mono, m_LiquidPouringGameObject.name, (item) =>
        {
            var data = m_LiquidPouringGameObject.GetTransformData();
            UnityEngine.Object.Destroy(m_LiquidPouringGameObject);
            if (item != null)
            {
                m_LiquidPouringGameObject = UnityEngine.Object.Instantiate(item);
                m_LiquidPouringGameObject.ApplyTransformData(data, TransformDataEnum.AllLocal);
            }
        });
#endif
    }

}