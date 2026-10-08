using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class CustomHierarchyGui
{
    static CustomHierarchyGui()
    {
        EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyGui;
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGui;
    }

    private static void OnHierarchyGui(int instanceID, Rect selectionRect)
    {
        var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go != null)
        {
            var rect = new Rect(selectionRect)
            {
                x = selectionRect.width + 16,
                width = 16,
            };
            bool state = GUI.Toggle(rect, go.activeSelf, string.Empty);

            if (go.activeSelf != state)
            {
                Undo.RecordObject(go, $"Toggle {go.name} {(state ? "Off" : "On")}");
                go.SetActive(state);
            }
        }
    }
}
