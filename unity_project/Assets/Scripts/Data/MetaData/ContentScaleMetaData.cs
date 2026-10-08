using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[MetaData(MetaDataType.ContentScale)]
public class ContentScaleMetaData : MetaData
{
    private Vector3 m_ObjScale = Vector3.one;
    public Vector3 GetObjectScale() { return m_ObjScale; }
    public void SetScale(Vector3 scale) { m_ObjScale = scale; }
}