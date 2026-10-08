using UnityEditor;
using UnityEngine;

public class FieldWidthScope : GUI.Scope
{
    private readonly float m_Width;

    public FieldWidthScope(float newWidth)
    {
        m_Width = EditorGUIUtility.fieldWidth;
        EditorGUIUtility.fieldWidth = newWidth;
    }

    protected override void CloseScope()
    {
        EditorGUIUtility.fieldWidth = m_Width;
    }
}