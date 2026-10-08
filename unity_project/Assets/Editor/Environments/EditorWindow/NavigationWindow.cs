using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AI;
using UnityEngine;

namespace EditorTools.Environments
{
    public class NavigationWindow : EditorWindow
    {
        [MenuItem("Window/Custom/Navigation Editor", false, 1)]
        private static void ShowWindow() => GetWindow<NavigationWindow>("Navigation");

        private List<Renderer> m_LayeredRenderers;
        private List<Renderer> m_StaticRenderers;

        private bool m_LayeredFoldout;
        private bool m_StaticFoldout;
        private bool m_EnableLayered;

        private Vector2 m_ScrollPos;

        private void OnEnable()
        {
            autoRepaintOnSceneChange = true;
            EditorHelper.UnfocusControls();

            RefreshRenderers();
        }

        private void RefreshRenderers()
        {
            var scenes = Utils.Editor.GetLoadedScenes();
        }

        private void OnGUI()
        {
            GUILayout.Label("Navigation Editor", EditorStyles.boldLabel);

            if (m_LayeredRenderers.Contains(null) || m_StaticRenderers.Contains(null))
            {
                RefreshRenderers();
            }

            using (new GuiEnabledScope(!EnvironmentSceneManager.IsBaking()))
            {
                using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
                {
                    m_LayeredFoldout = DrawNavigationRenderers(m_LayeredRenderers, "Navigation Specific Renderers", m_LayeredFoldout);
                    m_StaticFoldout = DrawNavigationRenderers(m_StaticRenderers, "Navigation Static Renderers", m_StaticFoldout);
                    m_ScrollPos = s.scrollPosition;
                }
                GUILayout.FlexibleSpace();

                EditorGUI.BeginChangeCheck();
                const string EnableLayered = "Enable Navigation Specific Renderers";
                m_EnableLayered = EditorGUILayout.ToggleLeft(EnableLayered, m_EnableLayered);
                if (EditorGUI.EndChangeCheck() == true)
                {
                    EnvironmentSceneManager.ToggleNavigationRenderers(m_LayeredRenderers, m_EnableLayered);
                }

                DrawVisibleLayer("Environment", Layers.DefaultLayer, Layers.TeleportLayer);
                DrawVisibleLayer("Navigation", Layers.TeleportLayer);

                using (new GUILayout.HorizontalScope())
                {
                    if (NavMeshBuilder.isRunning == false)
                    {
                        if (GUILayout.Button("Bake NavMesh") == true)
                        {
                            NavMeshBuilder.BuildNavMeshAsync();
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("Cancel Bake") == true)
                        {
                            NavMeshBuilder.Cancel();
                        }
                    }
                    if (GUILayout.Button("Clear NavMesh") == true)
                    {
                        NavMeshBuilder.ClearAllNavMeshes();
                    }
                }
                if (GUILayout.Button("Refresh Renderers") == true)
                {
                    RefreshRenderers();
                }
            }
        }

        private void DrawVisibleLayer(string label, params int[] layers)
        {
            bool state = layers.ToList().TrueForAll(Utils.Editor.IsLayerVisible);
            bool newState = EditorGUILayout.ToggleLeft($"Show {label} Layer", state);
            if (state != newState)
            {
                foreach (var layer in layers)
                {
                    Utils.Editor.ToggleVisibleLayer(layer, newState);
                }
                SceneView.RepaintAll();
            }
        }

        private static bool DrawNavigationRenderers(List<Renderer> renderers, string label, bool foldout)
        {
            foldout = EditorGUILayout.Foldout(foldout, label, true);
            if (foldout == true)
            {
                using (new GuiIndentScope())
                {
                    var toggleWidth = GUILayout.Width(26);
                    foreach (var renderer in renderers)
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            if (renderer == null)
                            {
                                EditorGUILayout.LabelField("(null)");
                            }
                            else
                            {
                                var go = renderer.gameObject;
                                EditorHelper.ObjectField(renderer);
                                EditorGUILayout.Toggle(GUIContent.none, go.activeInHierarchy, toggleWidth);
                            }
                        }
                    }
                }
            }

            return foldout;
        }
    }
}
