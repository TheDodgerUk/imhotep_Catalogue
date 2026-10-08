using EditorTools;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public static class Gui
    {
        public static readonly GUILayoutOption BtnUtilityWidth = GUILayout.Width(20);

        public static bool Btn(string text, bool misAlign, params GUILayoutOption[] options)
        {
            if (misAlign == true)
            {
                return GUI.Button(EditorGUILayout.GetControlRect(options), text);
            }
            else
            {
                return GUILayout.Button(text, options);
            }
        }

        public static bool BtnUtility(string text, bool misAlign = false)
        {
            return Btn(text, misAlign, BtnUtilityWidth);
        }

        public static bool BtnRemove(bool misAlign = false)
        {
            using (new GuiSkinTextColourScope(GUI.skin.button.normal, Color.white))
            {
                using (new GuiColourScope(bg: Color.red))
                {
                    return (BtnUtility("X", misAlign) == true);
                }
            }
        }

        public static Rect GetButtonRect(string label, int width)
        {
            return GUILayoutUtility.GetRect(label.ToGUI(), GUI.skin.button, GUILayout.Width(width));
        }
    }
}
