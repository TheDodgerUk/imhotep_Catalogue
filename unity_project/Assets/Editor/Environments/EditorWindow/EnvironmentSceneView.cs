using System;
using UnityEditor;
using UnityEngine;

namespace EditorTools.Environments
{
    public class EnvironmentSceneDataView
    {
        public event Action<EnvironmentSceneData> Remove = null;

        private readonly EnvironmentVariantDataView m_VariantsView;

        private readonly EnvironmentSceneData m_SceneData;

        public EnvironmentSceneDataView(EnvironmentSceneData sceneData)
        {
            m_VariantsView = new EnvironmentVariantDataView(sceneData);
            m_VariantsView.ToggleFoldout(true);
            m_SceneData = sceneData;
        }

        public bool OnGUI()
        {
            bool remove = false;
            var style = GUI.skin.box.Background(0.15f);
            using (new EditorGUILayout.VerticalScope(style))
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.TextField(m_SceneData.Guid, EditorStyles.boldLabel);

                    if (Utils.Gui.BtnRemove() == true)
                    {
                        Remove?.Invoke(m_SceneData);
                        remove = true;
                    }
                }

                m_SceneData.Name.Text = EditorGUILayout.TextField("Environment Name", m_SceneData.Name.Text);

                m_VariantsView.OnGUI();
            }

            return remove;
        }

        public void OnSceneGUI(SceneView sceneView)
        {
            m_VariantsView.OnSceneGUI(sceneView);
        }
    }
}
