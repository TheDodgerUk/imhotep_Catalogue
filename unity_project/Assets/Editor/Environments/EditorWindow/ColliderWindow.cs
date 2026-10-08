using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EditorTools.Environments
{
    public class ColliderWindow : EditorWindow
    {
        [MenuItem("Window/Utility/Collision Window", false, 1)]
        private static void ShowWindow() => GetWindow<ColliderWindow>("Collision");

        private List<Collider> m_Colliders;

        private Vector2 m_ScrollPos;
        private LayerMask m_LayerMask;
        private bool m_Foldout;

        private void OnEnable()
        {
            autoRepaintOnSceneChange = true;
            EditorHelper.UnfocusControls();

            RefreshColliders();
        }

        private void RefreshColliders()
        {
            m_Colliders = FindObjectsOfType<Collider>().ToList();
            m_Colliders.Sort(Comparisons.ByHierarchy);
        }

        private void OnGUI()
        {
            GUILayout.Label("Collision Editor", EditorStyles.boldLabel);

            if (m_Colliders.Exists(c => c == null) == true)
            {
                RefreshColliders();
            }

            var mask = EditorGUILayout.MaskField("Layer", m_LayerMask.value, InternalEditorUtility.layers);
            m_LayerMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(mask);

            var filteredColliders = m_Colliders.FindAll(c => Utils.BitMask.IsSet(m_LayerMask.value, c.gameObject.layer));

            using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
            {
                m_Foldout = EditorGUILayout.Foldout(m_Foldout, "Colliders", true);
                if (m_Foldout == true)
                {
                    using (new GuiIndentScope())
                    {
                        var groups = filteredColliders.GroupBy(c => c.gameObject.scene);
                        groups = groups.OrderBy(g => g.Key.name);

                        var toggleWidth = GUILayout.Width(26);
                        foreach (var group in groups)
                        {
                            EditorGUILayout.LabelField(group.Key.name);
                            using (new GuiIndentScope())
                            {
                                var groupedColliders = group.OrderBy(g => g, Comparisons.GetHierarchy<Collider>());

                                foreach (var collider in groupedColliders)
                                {
                                    using (new EditorGUILayout.HorizontalScope())
                                    {
                                        if (collider == null)
                                        {
                                            EditorGUILayout.LabelField("(null)");
                                        }
                                        else
                                        {
                                            var go = collider.gameObject;
                                            using (new GuiSkinFontAlignmentScope(GUI.skin.button, TextAnchor.MiddleLeft))
                                            {
                                                if (GUILayout.Button($"[{collider.GetType().Name}]_{go.name}") == true)
                                                {
                                                    var sceneView = Utils.Editor.GetSceneView();
                                                    sceneView.LookAt(go.transform.position);
                                                    sceneView.Focus();

                                                    EditorHelper.SelectAndPingObject(go);
                                                }
                                            }
                                            EditorGUILayout.Toggle(GUIContent.none, go.activeInHierarchy, toggleWidth);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                m_ScrollPos = s.scrollPosition;
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Refresh Colliders") == true)
            {
                RefreshColliders();
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
