using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable, MetaData(MetaDataType.ContentAdaptive)]
public class ContentAdaptiveMetaData : MetaData
{
    public enum AxisChoice
    {
        X,
        Y,
        Z,
        X_Y,
        X_Z
    }

    public class Modular
    {
        [System.NonSerialized]
        public GameObject m_GameObject;

        [SerializeField]
        public string m_GameObjectName = "";

        [SerializeField]
        public Vector3 m_Offset;

        [SerializeField]
        public Vector3 m_ModulePartSize = Vector3.zero;
    }

    public class ModAxisSingle
    {
        public Modular m_NonModuleMinus = new Modular();
        public Modular m_NonModulePlus = new Modular();
        public Modular m_NonModuleCentre = new Modular();
    }

    public class ModAxisDouble
    {
        public Modular m_NonModule_NW = new Modular();
        public Modular m_NonModule_N = new Modular();
        public Modular m_NonModule_NE = new Modular();
        public Modular m_NonModule_E = new Modular();
        public Modular m_NonModule_SE = new Modular();
        public Modular m_NonModule_S = new Modular();
        public Modular m_NonModule_SW = new Modular();
        public Modular m_NonModule_W = new Modular();
        public Modular m_NonModuleCentre = new Modular();
    }

    public ModAxisSingle m_NonModularSingle = new ModAxisSingle();
    public ModAxisDouble m_NonModularDouble = new ModAxisDouble();


    public AxisChoice m_AxisChoice = AxisChoice.X;


    public List<Axis> UnPackAxisField()
    {
        List<Axis> finalList = new List<Axis>();

        switch (m_AxisChoice)
        {
            case AxisChoice.X:
                finalList.Add(Axis.X);
                break;
            case AxisChoice.Y:
                finalList.Add(Axis.Y);
                break;
            case AxisChoice.Z:
                finalList.Add(Axis.Z);
                break;
            case AxisChoice.X_Y:
                finalList.Add(Axis.X);
                finalList.Add(Axis.Y);
                break;
            case AxisChoice.X_Z:
                finalList.Add(Axis.X);
                finalList.Add(Axis.Z);
                break;
            default:
                break;
        }
        return finalList;
    }


}