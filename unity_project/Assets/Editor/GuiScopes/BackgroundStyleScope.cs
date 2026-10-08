using EditorTools;
using UnityEditor;
using UnityEngine;

public abstract class BackgroundStyleScope : GUI.Scope
{
    protected BackgroundStyleScope(BackgroundStyle style, params GUILayoutOption[] options) { }

    protected static GUIStyle GetStyle(BackgroundStyle style)
    {
        switch (style)
        {
            case BackgroundStyle.Darken: return GUI.skin.box.Background(0.1f);
            case BackgroundStyle.Box: return GUI.skin.box;
            default: return GUIStyle.none;
        }
    }
}

public class VerticalStyleScope : BackgroundStyleScope
{
    public VerticalStyleScope(BackgroundStyle style, params GUILayoutOption[] options) : base(style, options)
    {
        EditorGUILayout.BeginVertical(GetStyle(style), options);
    }

    protected override void CloseScope()
    {
        EditorGUILayout.EndVertical();
    }
}

public class HorizontalStyleScope : BackgroundStyleScope
{
    public HorizontalStyleScope(BackgroundStyle style, params GUILayoutOption[] options) : base(style, options)
    {
        EditorGUILayout.BeginHorizontal(GetStyle(style), options);
    }

    protected override void CloseScope()
    {
        EditorGUILayout.EndHorizontal();
    }
}

public enum BackgroundStyle
{
    None,
    Darken,
    Box,
}