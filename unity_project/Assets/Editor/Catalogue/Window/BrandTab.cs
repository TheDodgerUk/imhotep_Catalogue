using EditorTools.Views;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class BrandTab : IEditorView
    {
        public string Name => "Brands";

        private StringView m_StringView;

        public BrandTab(Catalogue catalogue)
        {
            m_StringView = new StringView(Name, catalogue.Description.Brands)
            {
                UseScrollView = true
            };
        }

        public void OnShow() { }

        public void OnGUI()
        {
            m_StringView.OnGUI();
            GUILayout.FlexibleSpace();
        }
    }
}
