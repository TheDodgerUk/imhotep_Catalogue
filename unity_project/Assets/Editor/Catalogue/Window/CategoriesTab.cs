using EditorTools.Views;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class CategoriesTab : IEditorView
    {
        public string Name => "Categories";

        private CatalogueCategoryView m_CategoryView;

        public CategoriesTab(Catalogue catalogue)
        {
            m_CategoryView = new CatalogueCategoryView(catalogue.Description.RootCategory);
        }

        public void OnShow() { }

        public void OnGUI()
        {
            m_CategoryView.OnGUI();

            GUILayout.FlexibleSpace();
        }

        private class CatalogueCategoryView : IListItemViewable
        {
            private CatalogueCategoryListView m_SubCategoryListView;
            private CatalogueFilterListView m_FilterView;

            private CatalogueCategory m_Category;

            public CatalogueCategoryView(CatalogueCategory category)
            {
                var style = BackgroundStyle.Darken;
                m_SubCategoryListView = new CatalogueCategoryListView("Sub Categories", category.SubCategories) { Style = style };
                m_FilterView = new CatalogueFilterListView("Filters", category.Filters) { Style = style };
                m_SubCategoryListView.ToggleFoldout(false);
                m_FilterView.ToggleFoldout(false);

                m_Category = category;
            }

            public void OnGUI()
            {
                Utils.View.DrawGuid(m_Category.Guid);

                m_Category.Name.Text = EditorGUILayout.TextField("Name", m_Category.Name.Text);

                m_SubCategoryListView.OnGUI();
                m_FilterView.OnGUI();
            }
        }

        private class CatalogueCategoryListView : DefaultListItemView<CatalogueCategory, CatalogueCategoryView>
        {
            public CatalogueCategoryListView(string name, List<CatalogueCategory> categories) : base(name, categories, c => new CatalogueCategoryView(c)) { }
        }

        private class CatalogueFilterListView : DefaultListItemView<CatalogueFilter, CatalogueFilterView>
        {
            public CatalogueFilterListView(string name, List<CatalogueFilter> filters) : base(name, filters, f => new CatalogueFilterView(f)) { }
        }
    }
}
