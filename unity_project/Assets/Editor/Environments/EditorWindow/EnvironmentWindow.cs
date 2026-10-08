using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Environments
{
    public class EnvironmentWindow : EditorWindow
    {
        [MenuItem("Window/Custom/Environment Editor", false, 1)]
        private static void ShowEnvironmentEditor() => GetWindow<EnvironmentWindow>("Environments");

        private List<EnvironmentSceneDataView> m_Views;
        private EnvironmentDataManager m_Environments;
        private Vector2 m_ScrollPos;

        private void OnEnable()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            autoRepaintOnSceneChange = true;

            m_Environments = new EnvironmentDataManager();
            m_Environments.LoadFromFile();

            RefreshViews();

            EditorHelper.UnfocusControls();
        }

        private void OnDestroy()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
        }

        private void OnGUI()
        {
            GUILayout.Label("Environment Editor", EditorStyles.boldLabel);

            using (new GuiEnabledScope(!EnvironmentSceneManager.IsBaking()))
            {
                var environments = m_Environments.Environments;
                using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
                {
                    for (int i = 0; i < m_Views.Count; ++i)
                    {
                        if (m_Views[i].OnGUI() == true)
                        {
                            m_Views.RemoveAt(i);
                            --i;
                        }
                    }
                    m_ScrollPos = s.scrollPosition;
                }
                GUILayout.FlexibleSpace();

                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("Add") == true)
                    {
                        var environment = new EnvironmentSceneData();
                        environments.Add(environment);
                        m_Views.Add(CreateView(environment));
                    }
                    if (GUILayout.Button("Load") == true)
                    {
                        m_Environments.LoadFromFile();
                        RefreshViews();
                    }
                    if (GUILayout.Button("Save") == true)
                    {
                        Bundles.AssetBundleCreationValidation.CreateFileStructureForAssignBundles();
                        m_Environments.SaveToFile();
                        AssetDatabase.Refresh();
                    }
                    if (GUILayout.Button("Clean") == true)
                    {
                        EnvironmentSceneManager.CleanUnusedEnvironments(environments);
                    }
                }
            }
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            foreach (var view in m_Views)
            {
                view.OnSceneGUI(sceneView);
            }
        }

        private void RefreshViews()
        {
            m_Views = m_Environments.Environments.Extract(CreateView);
        }

        private EnvironmentSceneDataView CreateView(EnvironmentSceneData environment)
        {
            var view = new EnvironmentSceneDataView(environment);
            view.Remove += OnRemoveEnvironment;
            return view;
        }

        private void OnRemoveEnvironment(EnvironmentSceneData environment)
        {
            m_Environments.Environments.Remove(environment);
        }
    }
}
