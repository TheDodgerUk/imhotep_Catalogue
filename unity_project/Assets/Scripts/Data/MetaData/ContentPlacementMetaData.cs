using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ContentPlacementMetaData;

public static class EnumPlacementExtensions
{
    public static bool IsWall(this EnumPlacement enumValue)
    {
        switch (enumValue)
        {
            case EnumPlacement.WallMounted:
            case EnumPlacement.WallMountedAndStackable:
            case EnumPlacement.WallCutout:
            case EnumPlacement.WallMountedAndGrounded:
            case EnumPlacement.WallCutoutAndGrounded:
            case EnumPlacement.WallHanging:
                return true;
        }
        return false;
    }

    public static bool IsWallStackable(this EnumPlacement enumValue)
    {
        switch (enumValue)
        {
            case EnumPlacement.WallMountedAndStackable:
                return true;
        }
        return false;
    }

    public static bool IsStackable(this EnumPlacement enumValue)
    {
        switch (enumValue)
        {
            case EnumPlacement.Stackable:
            case EnumPlacement.WallMountedAndStackable:
                return true;
        }
        return false;
    }

    public static bool IsSpecial(this EnumPlacement enumValue)
    {
        switch (enumValue)
        {
            case EnumPlacement.WallHanging:
            case EnumPlacement.Ceiling:
            case EnumPlacement.MountToAll:
                return true;
        }
        return false;
    }

    public static bool IsCutout(this EnumPlacement enumValue)
    {
        switch (enumValue)
        {
            case EnumPlacement.WallCutout:
            case EnumPlacement.WallCutoutAndGrounded:
                return true;
        }
        return false;
    }


    public static bool IsWallAndGrounded(this EnumPlacement enumValue)
    {
        switch (enumValue)
        {
            case EnumPlacement.WallMountedAndGrounded:
            case EnumPlacement.WallCutoutAndGrounded:
                return true;
        }
        return false;
    }

    public static bool IsGrounded(this EnumPlacement enumValue)
    {
        switch (enumValue)
        {
            case EnumPlacement.Stackable:
            case EnumPlacement.NotStackable:
            case EnumPlacement.WallMountedAndGrounded:
            case EnumPlacement.WallCutoutAndGrounded:
                return true;
        }
        return false;
    }
}



[System.Serializable, MetaData(MetaDataType.ContentPlacement)]
public class ContentPlacementMetaData : MetaData
{
    public enum EnumPlacement
    {
        Stackable,
        NotStackable,                   // sofa
        WallMounted,                    // lights
        WallMountedAndStackable,
        WallMountedAndGrounded,         // curtains 
        WallCutout,                     // windows
        WallCutoutAndGrounded,          // doors
        WallHanging,
        Ceiling,
        MountToAll, 
        SetToZero,
    }

    public enum EnumMethod
    {
        Gravity, 
        Widget,
    }


    [SerializeField]
    public EnumPlacement Placement = EnumPlacement.Stackable;

    [SerializeField]
    public EnumMethod Method = EnumMethod.Gravity;

    [SerializeField]
    public bool m_MainColliderEnabledInVR = true;

    [SerializeField]
    public bool m_UseAllMainColliders = true;

    [SerializeField]
    public string m_OutlineOverrideGameObjectName;

    [NonSerialized]
    public GameObject m_OutlineOverrideGameObject;

    public enum SnappingPointEnum
    {
        MiddleX,
        MiddleY,
        MiddleZ,
        Right,
        Left,
        Forward,
        Back,
        Top,
        Bottom,
    }

    public enum SnappingPointEnumFloor
    {
        MiddleX,
        MiddleZ,
        Right,
        Left,
        Forward,
        Back,

    }

    public enum SnappingPointEnumWall
    {
        MiddleX,
        MiddleY,
        Right,
        Left,
        Top,
        Bottom,
    }

    public enum SnappingPositionsEnum
    {
        Snapping_Position_Top,
        Snapping_Position_Bottom,
        Snapping_Position_Left,
        Snapping_Position_Right,
        Snapping_Position_Forward,
        Snapping_Position_Backward,
    }


    [SerializeField]
    public bool m_SnapToOthers = true;

    [SerializeField]
    public bool m_SnapToOnlyTheseOthers = false;
    public class SnappingItemData
    {
        [SerializeField]
        public string m_GuidsToSnap = "";
    }

    [SerializeField]
    public List<SnappingItemData> m_SnappingItemData = new List<SnappingItemData>();
    [SerializeField]
    public float m_SnappingDistance = 0.1f;
    [SerializeField]
    public int m_SnappingPointEnumField = 1;


    [SerializeField]
    public string m_ExtraPivotPoint = "";
    [NonSerialized]
    public GameObject m_ExtraPivotPointGameObject = null;
    [SerializeField]
    public Axis m_ExtraPivotPointAxis = Axis.X;

    public List<SnappingPointEnum> UnPackSnappingPointField()
    {
        //https://answers.unity.com/questions/319940/maskfield-selected-values.html
        List<SnappingPointEnum> finalList = new List<SnappingPointEnum>();
        List<string> newList = new List<string>();
        var mask = m_SnappingPointEnumField;
        if (Placement.IsWall())
        {
            for (int i = 0; i < Enum.GetNames(typeof(SnappingPointEnumWall)).Length; i++)
            {
                int layer = 1 << i;
                if ((mask & layer) != 0)
                {
                    newList.Add(((SnappingPointEnumWall)i).ToString());
                }
            }
        }
        else
        {
            for (int i = 0; i < Enum.GetNames(typeof(SnappingPointEnumFloor)).Length; i++)
            {
                int layer = 1 << i;
                if ((mask & layer) != 0)
                {
                    newList.Add(((SnappingPointEnumFloor)i).ToString());
                }
            }
        }

        foreach (var item in newList)
        {
            var newItem = (SnappingPointEnum)Enum.Parse(typeof(SnappingPointEnum), item.ToString(), true);
            finalList.Add(newItem);
        }
        return finalList;
    }

    public static bool Match(GameObject thisGameobject, string thisName, GameObject otherGameObject, string otherName)
    {
        SnappingPositionsEnum m1 = (SnappingPositionsEnum)System.Enum.Parse(typeof(SnappingPositionsEnum), thisName);
        SnappingPositionsEnum m2 = (SnappingPositionsEnum)System.Enum.Parse(typeof(SnappingPositionsEnum), otherName);

        bool xAxis = (m1 == SnappingPositionsEnum.Snapping_Position_Left || m1 == SnappingPositionsEnum.Snapping_Position_Right);
        if (Vector3.Dot(thisGameobject.transform.forward, otherGameObject.transform.forward) < 0 && xAxis == true)
        {
            if (m1 == m2) return true;
            return false;
        }
        else
        {
            if (m1 == m2) return false;
            if (m1 == SnappingPositionsEnum.Snapping_Position_Bottom && m2 == SnappingPositionsEnum.Snapping_Position_Top) return true;
            if (m2 == SnappingPositionsEnum.Snapping_Position_Bottom && m1 == SnappingPositionsEnum.Snapping_Position_Top) return true;

            if (m1 == SnappingPositionsEnum.Snapping_Position_Left && m2 == SnappingPositionsEnum.Snapping_Position_Right) return true;
            if (m2 == SnappingPositionsEnum.Snapping_Position_Left && m1 == SnappingPositionsEnum.Snapping_Position_Right) return true;

            if (m1 == SnappingPositionsEnum.Snapping_Position_Forward && m2 == SnappingPositionsEnum.Snapping_Position_Backward) return true;
            if (m2 == SnappingPositionsEnum.Snapping_Position_Forward && m1 == SnappingPositionsEnum.Snapping_Position_Backward) return true;
        }
        return true;
    }




    public static Vector3 FaceDirection(GameObject root, SnappingPositionsEnum snapEnum)
    {
        Vector3 direction = Vector3.zero;
        switch (snapEnum)
        {
            case SnappingPositionsEnum.Snapping_Position_Top:
                direction = root.transform.up;
                break;
            case SnappingPositionsEnum.Snapping_Position_Bottom:
                direction = -root.transform.up;
                break;
            case SnappingPositionsEnum.Snapping_Position_Left:
                direction = -root.transform.right;
                break;
            case SnappingPositionsEnum.Snapping_Position_Right:
                direction = root.transform.right;
                break;
            case SnappingPositionsEnum.Snapping_Position_Forward:
                direction = root.transform.forward;
                break;
            case SnappingPositionsEnum.Snapping_Position_Backward:
                direction = -root.transform.forward;
                break;
            default:
                break;
        }
        return direction;

    }
}