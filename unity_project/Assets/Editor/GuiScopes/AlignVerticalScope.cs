using UnityEditor;
using UnityEngine;

public class AlignVerticalScope : GUI.Scope
{
    public AlignVerticalScope()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
    }

    protected override void CloseScope()
    {
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();
    }
}