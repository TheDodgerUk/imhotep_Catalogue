using System;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public static class View
    {
        public static void DrawGuid(string guid, Action additionalControls = null)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.TextField("GUID", guid, EditorStyles.boldLabel);

                if (GUILayout.Button("Copy GUID", GUILayout.Width(90)) == true)
                {
                    EditorGUIUtility.systemCopyBuffer = guid;
                }

                additionalControls?.Invoke();
            }
        }
    }
}