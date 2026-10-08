using FilePathHelper;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class CataloguePresetView
    {
        private CataloguePreset m_Selected;
        private Catalogue m_Catalogue;

        private SaveSceneView m_SaveSceneView;
        private Vector2 m_ScrollPos;

        public CataloguePresetView(Catalogue catalogue, CataloguePreset selected)
        {
            m_Catalogue = catalogue;
            SetSelected(selected);
        }

        public void SetSelected(CataloguePreset selected)
        {
            m_Selected = selected ?? new CataloguePreset();

            m_SaveSceneView = new SaveSceneView(m_Catalogue, selected.Contents);
        }

        public void OnGUI()
        {
            Utils.View.DrawGuid(m_Selected.Guid, () =>
            {
                if (GUILayout.Button("Select Folder", GUILayout.Width(100)) == true)
                {
                    string presetPath = Catalogue.GetPresetPath(m_Selected.Guid);
                    var path = FilePath.FromAbsolute($"{presetPath}.json");
                    EditorHelper.SelectAndPingObject(path.Relative);
                }
            });

            // name and prefab type
            m_Selected.FullName.Text = EditorGUILayout.TextField("Full Name", m_Selected.FullName.Text);
            m_Selected.ShortName.Text = EditorGUILayout.TextField("Short Name", m_Selected.ShortName.Text);
            m_Selected.ReferenceNumber = EditorGUILayout.TextField("ReferenceNumber", m_Selected.ReferenceNumber);
            EditorGUILayout.Space();

            // catalogue ids
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos, GUIStyle.none, GUI.skin.verticalScrollbar))
                {
                    m_SaveSceneView.OnGUI();
                    m_ScrollPos = s.scrollPosition;
                }
            }
        }
    }
}