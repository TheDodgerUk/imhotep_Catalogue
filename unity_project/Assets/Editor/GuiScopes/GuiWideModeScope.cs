using UnityEditor;
using UnityEngine;

public class GuiWideModeScope : GUI.Scope
{
    private readonly bool m_WasWide;

    public GuiWideModeScope(bool wideMode)
    {
        m_WasWide = EditorGUIUtility.wideMode;
        EditorGUIUtility.wideMode = wideMode;
    }

    protected override void CloseScope()
    {
        EditorGUIUtility.wideMode = m_WasWide;
    }
}