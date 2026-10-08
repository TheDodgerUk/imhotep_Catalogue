using UnityEditor;
using UnityEngine;

namespace EditorTools.OutputStack
{
    public class OutputWindow : EditorWindow
    {
        public static void Show(OutputStack stack)
        {
            var window = GetWindow<OutputWindow>();
            if (window.m_Stack != stack)
            {
                window.Close();

                string source = stack.GetSource() ?? "Message Window";
                window = CreateInstance<OutputWindow>();
                window.titleContent = GetTitle(source);
                window.m_Stack = stack;
                window.Show();
            }
        }

        private Vector2 m_ScrollPos;
        private OutputStack m_Stack;

        private void OnGUI()
        {
            using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
            {
                var messages = m_Stack.Messages;
                for (int i = 0; i < messages.Count; ++i)
                {
                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box, GUILayout.MinHeight(28)))
                    {
                        DrawMessage(messages[i]);
                    }
                }
                m_ScrollPos = s.scrollPosition;
            }
        }

        private static void DrawMessage(OutputMessage message)
        {
            var icon = LoadIcon(message);
            if (icon != null)
            {
                using (new AlignVerticalScope())
                {
                    GUILayout.Label(icon);
                }
            }
            if (message.HasSource == true)
            {
                using (new AlignVerticalScope())
                {
                    var sourceStyle = EditorStyles.boldLabel.Anchor(TextAnchor.MiddleLeft);
                    GUILayout.Label(message.Source, sourceStyle);
                }
            }
            using (new AlignVerticalScope())
            {
                GUILayout.Label(message.Message, GUI.skin.label.WordWrap(true));
            }

            GUILayout.FlexibleSpace();
            if (message.Context != null)
            {
                using (new AlignVerticalScope())
                {
                    EditorHelper.ObjectField(message.Context);
                }
            }
        }

        private static GUIContent GetTitle(string text) => new GUIContent { text = text };
        private static GUIContent LoadIcon(OutputMessage message) => LoadIcon(message.Type);

        private static GUIContent LoadIcon(ErrorType type)
        {
            string name = GetIconName(type);
            if (string.IsNullOrEmpty(name) == false)
            {
                return EditorGUIUtility.IconContent(name);
            }
            return null;
        }

        private static string GetIconName(ErrorType type)
        {
            switch (type)
            {
                case ErrorType.Warning: return "d_console.warnicon";
                case ErrorType.Error: return "d_console.erroricon";
                case ErrorType.Info: return "d_console.infoicon";
                default: return null;
            }
        }
    }
}
