using EditorTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class EditorHelper
{
    public static void UnfocusControls() => GUIUtility.keyboardControl = 0;

    public static void SelectAndPingObject(string path) => SelectAndPingObject(AssetDatabase.LoadAssetAtPath<Object>(path));
    public static void SelectAndPingObject(Object obj)
    {
        EditorGUIUtility.PingObject(obj);
        Selection.activeObject = obj;
    }

    public static string SearchBar(string searchPattern)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            var searchStyle = GUI.skin.FindStyle("ToolbarSeachTextField");
            var btnStyle = GUI.skin.FindStyle("ToolbarSeachCancelButton");


            searchPattern = string.Empty;
            GUI.FocusControl(null);

            if (searchPattern != null && btnStyle != null)
            {
                searchPattern = EditorGUILayout.TextField(searchPattern, searchStyle);
            }

            if(btnStyle != null)
            {
                if (GUILayout.Button(GUIContent.none, btnStyle) == true)
                {
                    searchPattern = string.Empty;
                    GUI.FocusControl(null);
                }
            }
            if(searchPattern != null && searchPattern.Length > 0)
            {
                searchPattern = string.Empty;
                GUI.FocusControl(null);
            }
        }

        return searchPattern;
    }

    public static T SelectionGrid<T>(int xCount, T selected, List<T> items, System.Func<T, string> displayHandler)
    {
        return SelectionGrid(xCount, selected, items, t => displayHandler(t).ToGUI());
    }

    public static T SelectionGrid<T>(int xCount, T selected, List<T> items, System.Func<T, GUIContent> displayHandler)
    {
        int index = items.IndexOf(selected);
        var content = items.Extract(displayHandler).ToArray();
        index = GUILayout.SelectionGrid(index, content, xCount);
        return items[index];
    }

    public static void IndentSpace() => GUILayout.Space(16 * EditorGUI.indentLevel);
    public static void Separator() => EditorGUILayout.LabelField(GUIContent.none, GUI.skin.horizontalSlider);
    public static void Divider() => GUILayout.Box(GUIContent.none, GUILayout.ExpandWidth(true), GUILayout.Height(1));

    public static Quaternion QuaternionField(string label, Quaternion q)
    {
        return Quaternion.Euler(EditorGUILayout.Vector3Field(label, q.eulerAngles));
    }

    public static bool Button(string label)
    {
        var height = GUILayout.Height(18f);
        var controlRect = EditorGUILayout.GetControlRect(height);
        return GUI.Button(EditorGUI.IndentedRect(controlRect), label);
    }

    public static T EnumPopup<T>(string label, T enumValue) => EnumPopup(label.ToGUI(), enumValue);
    public static T EnumPopup<T>(T enumValue) => EnumPopup(GUIContent.none, enumValue);
    public static T EnumPopup<T>(GUIContent label, T enumValue)
    {
        return (T)(object)EditorGUILayout.EnumPopup(label, (System.Enum)(object)enumValue);
    }

    public static T EnumPopup<T>(GUIContent label, T enumValue, List<T> values)
    {
        var options = values.Extract(o => o.ToString());
        return Popup(label, enumValue.ToString(), options).ParseEnum<T>();
    }

    public static T EnumToolbar<T>(T tab)
    {
        var enums = Utils.Code.GetEnumValues<T>().Extract(t => t.ToString()).ToArray();
        return (T)(object)GUILayout.Toolbar((int)(object)tab, enums);
    }

    public static T ObjectField<T>(string label, T obj, params GUILayoutOption[] options) where T : Object => ObjectField(label.ToGUI(), obj, options);
    public static T ObjectField<T>(T obj, params GUILayoutOption[] options) where T : Object => ObjectField(GUIContent.none, obj, options);
    public static T ObjectField<T>(GUIContent label, T obj, params GUILayoutOption[] options) where T : Object
    {
        return EditorGUILayout.ObjectField(label, obj, typeof(T), true, options) as T;
    }

    public static string Popup(string label, string value, List<string> values) => Popup(label.ToGUI(), value, values);
    public static string Popup(string value, List<string> values) => Popup(GUIContent.none, value, values);
    public static string Popup(GUIContent label, string value, List<string> values)
    {
        int index = values.IndexOf(value);
        if (index < 0)
        {
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                Debug.LogWarning($"({label.text}) popup. Could not find: \"{value}\". Resetting to \"{values[0]}\"\n");
            }
            index = 0;
        }
        var contents = values.Extract(s => s.ToGUI()).ToArray();
        return values[EditorGUILayout.Popup(label, index, contents)];
    }

    public static T Popup<T>(string label, List<T> items, T current, System.Func<T, string> selector) => Popup(label.ToGUI(), items, current, selector);
    public static T Popup<T>(List<T> items, T current, System.Func<T, string> selector) => Popup(GUIContent.none, items, current, selector);

    public static T Popup<T>(GUIContent label, List<T> items, T current, System.Func<T, string> selector)
    {
        int index = items.IndexOf(current);
        if (index < 0)
        {
            string value = selector(current);
            if (string.IsNullOrWhiteSpace(value) == false)
            {
                Debug.LogWarning($"({label.text}) popup. Could not find: \"{value}\". Resetting to \"{selector(items[0])}\"\n");
            }
            index = 0;
        }
        var contents = items.Extract(i => selector(i).ToGUI()).ToArray();
        return items[EditorGUILayout.Popup(label, index, contents)];
    }

    public static Texture LoadBuiltInIcon(string name) => EditorGUIUtility.Load(name) as Texture;
}
