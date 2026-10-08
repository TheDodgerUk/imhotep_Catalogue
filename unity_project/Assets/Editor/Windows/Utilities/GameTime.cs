using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameTime : EditorWindow
{
    private static readonly List<float> Times = new List<float> { 0.1f, 0.5f, 1, 2, 5, 10 };

    [MenuItem("Window/Utility/Game Time")]
    public static void ShowWindow() { GetWindow<GameTime>("Game Time"); }

    private void OnGUI()
    {
        float screenWidth = Screen.width - 28f;
        int widthPx = (int)(screenWidth / Times.Count).Clamp(40, 80);
        int heightPx = (int)(widthPx * 0.8f);

        var height = GUILayout.Height(heightPx);
        var width = GUILayout.Width(widthPx);

        GUILayout.BeginHorizontal();
        {
            foreach (float t in Times)
            {
                string text = "x" + t.ToString("N1");
                if (GUILayout.Button(text, width, height) == true)
                {
                    Time.timeScale = t;
                }
            }
        }
        GUILayout.EndHorizontal();

        Time.timeScale = EditorGUILayout.FloatField("Current time scale", Time.timeScale);
    }
}
