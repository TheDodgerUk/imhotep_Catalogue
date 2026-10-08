using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentMaterialChange)]
public class ContentMaterialChangeMetaData : MetaData
{
    public class MaterialItemData
    {
        public enum OrientationType
        {
            Portrait,
            Landscape,
        }

        public enum MaterialType
        {
            Colour,
            Material,
            Texture
        }

        public enum TextureMap
        {
            Main,
            Secondary,
        }

        [SerializeField]
        public MaterialType m_MaterialType = MaterialType.Colour;


        [System.NonSerialized]
        public GameObject m_MaterialGameObject;

        [SerializeField]
        public string m_MaterialGameObjectName;

        [SerializeField]
        public int m_MaterialIndex;

        [SerializeField]
        public OrientationType m_OrientationType = OrientationType.Portrait;

        [SerializeField]
        public TextureMap m_TextureMap = TextureMap.Main;
    }

    public List<MaterialItemData> m_MaterialItemData = new List<MaterialItemData>();
}
