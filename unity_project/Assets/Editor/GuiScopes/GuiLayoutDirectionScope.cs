using UnityEditor;
using UnityEngine;

public enum GuiLayoutDirection { Horizontal, Vertical };

public class GuiLayoutDirectionScope : GUI.Scope
{
    private readonly GuiLayoutDirection m_Direction;

    public GuiLayoutDirectionScope(GuiLayoutDirection direction)
    {
        m_Direction = direction;
        Begin();
    }

    protected override void CloseScope()
    {
        End();
    }

    public void Begin()
    {
        if (m_Direction == GuiLayoutDirection.Horizontal)
        {
            EditorGUILayout.BeginHorizontal();
        }
        else if (m_Direction == GuiLayoutDirection.Vertical)
        {
            EditorGUILayout.BeginVertical();
        }
    }

    public void End()
    {
        if (m_Direction == GuiLayoutDirection.Horizontal)
        {
            EditorGUILayout.EndHorizontal();
        }
        else if (m_Direction == GuiLayoutDirection.Vertical)
        {
            EditorGUILayout.EndVertical();
        }
    }
}