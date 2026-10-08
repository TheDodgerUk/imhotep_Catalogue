using UnityEditor;
using UnityEngine;

public class LabelWidthScope : GUI.Scope
{
    private readonly float m_LabelWidth;

    public LabelWidthScope(float newWidth)
    {
        m_LabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = newWidth;
    }

    protected override void CloseScope()
    {
        EditorGUIUtility.labelWidth = m_LabelWidth;
    }
}