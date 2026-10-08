using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PositionSaveLoad))]
public class PositionSaveLoadEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PositionSaveLoad myTarget = (PositionSaveLoad)target;

        if (true == GUILayout.Button("Save Positions"))
        {
            myTarget.SavePositions();
        }

        if (true == GUILayout.Button("Load Positions"))
        {
            myTarget.CreateObjectsFromData();
        }

        EditorGUILayout.LabelField($"Total Saved Objects: {myTarget.m_Positions.Count}");
    }
}
