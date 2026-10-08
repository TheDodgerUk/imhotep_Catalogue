using System;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    public class InputTextWindow : EditorWindow
    {
        public static InputTextWindow Create(string label, string defaultText, Rect anchorRect, Action<string> ok, Action cancel = null)
        {
            var window = CreateInstance<InputTextWindow>();

            float height = (EditorGUIUtility.singleLineHeight + 4) * 3;
            var guiPos = new Vector2(anchorRect.xMin, anchorRect.yMax);
            var pos = GUIUtility.GUIToScreenPoint(guiPos);

            window.position = new Rect(pos.x, pos.y, 350, height);

            window.ShowPopup();

            window.m_Input = defaultText;
            window.m_Label = label;
            window.m_Cancel = cancel;
            window.m_Ok = ok;

            return window;
        }

        private Action m_Cancel;
        private Action<string> m_Ok;

        private string m_Input;
        private string m_Label;

        private void OnLostFocus()
        {
            m_Cancel?.Invoke();
            Close();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField(m_Label);

            GUI.SetNextControlName("input");
            EditorGUI.FocusTextInControl("input");

            m_Input = EditorGUILayout.TextField(m_Input);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Ok") == true)
                {
                    m_Ok(m_Input);
                    Close();
                }
                if (GUILayout.Button("Cancel") == true)
                {
                    m_Cancel?.Invoke();
                    Close();
                }
            }
        }
    }
}