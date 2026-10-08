using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnityEngine.Object), true)]
public class InspectorMethodEditor : Editor
{
    private List<AttributeMethod<InspectorButtonAttribute>> m_Methods = null;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawMethodButtons();
    }

    protected void DrawMethodButtons()
    {
        if (m_Methods == null)
        {
            m_Methods = GetMethods<InspectorButtonAttribute>(target.GetType());
        }

        foreach (var m in m_Methods)
        {
            using (new GuiEnabledScope(EditorApplication.isPlaying || !m.Attribute.PlayModeOnly))
            {
                if (GUILayout.Button($"{m.Method.Name}()") == true)
                {
                    m.Method.Invoke(target, new object[0]);
                }
            }
        }
    }

    private List<AttributeMethod<T>> GetMethods<T>(Type type) where T : Attribute
    {
        var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

        var allMethods = type.GetMethods(flags).ToList();
        var attributes = allMethods.Extract(m => new AttributeMethod<T> { Method = m });
        attributes.ForEach(a => a.Attribute = a.Method.GetCustomAttribute<T>());
        attributes.RemoveAll(a => a.Attribute == null);
        attributes.RemoveAll(a => a.Method.GetParameters().Length != 0);

        if (type.BaseType != null)
        {
            attributes.InsertRange(0, GetMethods<T>(type.BaseType));
        }
        return attributes;
    }

    private class AttributeMethod<T> where T : Attribute
    {
        public MethodInfo Method;
        public T Attribute;
    }
}
