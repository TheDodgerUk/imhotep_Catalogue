using System;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class CatalogueEntryTab : IEditorView
    {
        public event Action<string> EntrySelected = null;
        public event Action<bool, bool, bool> SaveCatalogue = null;
        public event Action Repaint = null;

        public string Name => "Catalogue";

        private CatalogueEntrySearchView m_SearchView;
        private CatalogueEntryView m_EntryView;

        private Action m_NextFrame = null;

        public CatalogueEntryTab(string selectedId)
        {
            var selected = Catalogue.Instance.Entries.Find(e => e.Guid == selectedId);
            if (selected == null)
            {
                selected = Catalogue.Instance.Entries.First();
            }

            m_SearchView = new CatalogueEntrySearchView(Catalogue.Instance.Entries, Catalogue.Instance, selected);
            m_EntryView = new CatalogueEntryView(Catalogue.Instance, selected);
            m_SearchView.SaveCatalogueAll += () => SaveCatalogue?.Invoke(false, false, false);
            m_SearchView.SaveCatalogueLast += () => SaveCatalogue?.Invoke(true, false, false);
            m_SearchView.Repaint += () => Repaint?.Invoke();
            m_SearchView.DataSelected += OnDataSelected;
        }

        public void OnShow()
        {
            m_EntryView.OnShow();
        }

        public void OnGUI()
        {
            m_NextFrame?.Invoke();
            m_NextFrame = null;

            using (new EditorGUILayout.HorizontalScope())
            {

                int keyboardFocus = GUIUtility.keyboardControl;
                bool editingText = EditorGUIUtility.editingTextField;

                m_SearchView.OnGUI();

                // Preserve an entry field's focus if the search view clears it.
                if (editingText && GUIUtility.keyboardControl == 0)
                {
                    GUIUtility.keyboardControl = keyboardFocus;
                    EditorGUIUtility.editingTextField = true;
                }

                using (new EditorGUILayout.VerticalScope())
                {
                    m_EntryView.OnGUI();
                }
            }
        }


        private CatalogueEntry m_LastSelected;

        public void OnDataReSelected()
        {
            OnDataSelected(m_LastSelected);
        }

        private void OnDataSelected(CatalogueEntry selected)
        {
            if (selected != null)
            {
                m_NextFrame = () =>
                {
                    m_LastSelected = selected;
                    EditorHelper.UnfocusControls();
                    m_SearchView.SetSelected(selected);
                    m_EntryView.SetSelected(selected);
                    EntrySelected?.Invoke(selected.Guid);
                };
            }
        }
    }
}
