using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectObjectInScene : EditorWindow
{
    [MenuItem("Window/Utility/Select Object in Scene")]
    public static void ShowWindow() { GetWindow<SelectObjectInScene>("Selection"); }

    private List<GameObject> m_SelectedObjects;
    private string m_SearchPattern;
    private Scene m_SelectedScene;

    private Vector2 m_ScrollPos = Vector2.zero;
    private bool m_SceneFoldout = false;

    private void Awake()
    {
        m_SelectedObjects = new List<GameObject>();
    }

    private void OnGUI()
    {
        // get loaded scenes
        var loadedScenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; ++i)
        {
            loadedScenes.Add(SceneManager.GetSceneAt(i));
        }

        using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
        {
            string sceneName = m_SelectedScene.IsValid() ? m_SelectedScene.name : "(none)";
            m_SceneFoldout = EditorGUILayout.Foldout(m_SceneFoldout, $"Selected scene: {sceneName}", true);

            // draw scenes
            if (m_SceneFoldout == true)
            {
                using (new GuiSkinFontAlignmentScope(GUI.skin.button, TextAnchor.MiddleLeft))
                {
                    using (new GuiIndentScope())
                    {
                        foreach (var scene in loadedScenes)
                        {
                            bool selected = m_SelectedScene == scene;
                            using (new GuiColourScope(selected ? Color.cyan : Color.white))
                            {
                                if (EditorHelper.Button(scene.name) == true)
                                {
                                    if (selected == true)
                                    {
                                        m_SelectedScene = new Scene();
                                    }
                                    else
                                    {
                                        m_SelectedScene = scene;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            m_SearchPattern = EditorHelper.SearchBar(m_SearchPattern);

            // draw objects
            foreach (var go in m_SelectedObjects)
            {
                EditorHelper.ObjectField(go);
            }
            m_ScrollPos = s.scrollPosition;
        }
    }
}
