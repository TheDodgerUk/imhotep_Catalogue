using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentClick)]
public class ContentClickMetaData : MetaData
{
    [SerializeField]
    public List<InstructionData> m_ClickData = new List<InstructionData>();

    public override void CollectAllData(GameObject root)
    {
        if (root != null)
        {
            foreach (var item in m_ClickData)
            {
                item.m_ModelGameObject = StringToGameObject(root, item.m_ModelGameObjectName);
            }
        }
    }


}