using System;
using UnityEditor;

namespace EditorTools.CatalogueViewer
{
    public class CataloguePresetTab : IEditorView
    {
        public event Action<string> PresetSelected = null;
        public event Action Repaint = null;

        public string Name => "Presets";

        private SearchView<CataloguePreset> m_SearchView;
        private CataloguePresetView m_PresetView;

        private Action m_NextFrame = null;

        public CataloguePresetTab(Catalogue catalogue, string selectedId)
        {
            var selected = catalogue.Presets.Find(e => e.Guid == selectedId);
            if (selected == null)
            {
                selected = catalogue.Presets.First();
            }

            m_SearchView = new SearchView<CataloguePreset>(catalogue.Presets, selected);
            m_PresetView = new CataloguePresetView(catalogue, selected);
            m_SearchView.Repaint += () => Repaint?.Invoke();
            m_SearchView.DataSelected += OnDataSelected;
        }

        public void OnShow() { }

        public void OnGUI()
        {
            m_NextFrame?.Invoke();
            m_NextFrame = null;

            using (new EditorGUILayout.HorizontalScope())
            {
                m_SearchView.OnGUI();

                using (new EditorGUILayout.VerticalScope())
                {
                    m_PresetView.OnGUI();
                }
            }
        }

        private void OnDataSelected(CataloguePreset selected)
        {
            m_NextFrame = () =>
            {
                EditorHelper.UnfocusControls();
                m_SearchView.SetSelected(selected);
                m_PresetView.SetSelected(selected);
                PresetSelected?.Invoke(selected.Guid);
            };
        }
    }
}
