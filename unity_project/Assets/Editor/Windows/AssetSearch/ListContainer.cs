using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ListContainer<T>
{
    public int Count { get { return List.Count; } }
    public Vector2 ScrollPos { get; set; }
    public List<T> List { get; set; }
    public bool Foldout { get; set; }

    protected GUIStyle m_TitleStyle;

    public ListContainer()
    {
        m_TitleStyle = EditorStyles.boldLabel;
        List = new List<T>();
    }

    public virtual void Draw(string title)
    {
        // title
        GUILayout.Label(title, m_TitleStyle);
    }
}
