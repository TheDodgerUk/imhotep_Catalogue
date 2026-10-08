using UnityEditor;
using UnityEngine;

public class GuiIndentScope : GUI.Scope
{
    private readonly int m_IndentLevel;

    public GuiIndentScope() : this(EditorGUI.indentLevel + 1) { }
    public GuiIndentScope(int indentLevel)
    {
        m_IndentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = indentLevel;
    }

    protected override void CloseScope()
    {
        EditorGUI.indentLevel = m_IndentLevel;
    }
}