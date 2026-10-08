using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InstructionData
{
    [System.NonSerialized]
    public GameObject m_ModelGameObject;
    [SerializeField]
    public string m_ModelGameObjectName = "";

    [SerializeField]
    public string m_sTitle;
    [SerializeField]
    public string m_sDescription;
}
