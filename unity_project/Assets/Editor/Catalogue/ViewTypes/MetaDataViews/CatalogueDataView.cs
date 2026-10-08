using EditorTools.Views;
using System.Collections.Generic;
using UnityEditor;

namespace EditorTools.CatalogueViewer
{
    public class CatalogueDataView : MetaDataView<CatalogueMetaData>
    {
        private CategoryView m_CategoryView;
        //private FilterDictionaryView m_FilterView;

        public CatalogueDataView(Catalogue catalogue, CatalogueEntry entry, List<IMetaDataView> oldViews) : base(catalogue, entry, oldViews)
        {
            m_CategoryView = new CategoryView("Categories", m_MetaData.Categories, catalogue);
            //var allKeys = m_Catalogue.Description.Filters.Extract(f => f.Guid);
            //m_FilterView = new FilterDictionaryView("Filters", m_MetaData.Filters, allKeys, m_Catalogue);
        }

        protected override void OnMetaDataGUI()
        {
            //m_MetaData.Category = EditorHelper.Popup("Category", m_MetaData.Category, m_Catalogue.Description.Categories);
            m_MetaData.Brand = EditorHelper.Popup("Brand", m_MetaData.Brand, m_Catalogue.Description.Brands);

            m_CategoryView.OnGUI();

            EditorGUILayout.Space();

            //m_FilterView.OnGUI();
        }

        private class FilterDictionaryView : DictionaryView<string, string>
        {
            //private Catalogue m_Catalogue;

            public FilterDictionaryView(string name, Dictionary<string, string> dictionary, List<string> allKeys, Catalogue catalogue) : base(name, dictionary, allKeys)
            {
                //m_Catalogue = catalogue;
            }

            protected override string DrawKey(string key, List<string> availableKeys)
            {
                string displayKey = key.Replace("_", "/");
                var keys = availableKeys.Extract(k => k.Replace("_", "/"));
                return EditorHelper.Popup(displayKey, keys).Replace("/", "_");
            }

            protected override string DrawValue(string key, string value)
            {
                //var filter = m_Catalogue.Description.Filters.Find(f => f.Guid == key);
                //var type = filter.GetFilterType();

                //if (type == FilterType.Value)
                //{
                //    // if value type, show as popup
                //    var values = filter.Values.Extract(v => v.Value);
                //    return EditorHelper.Popup(value, values);
                //}
                //else if (type == FilterType.Range)
                //{
                //    // if range, show as float
                //    float num;
                //    if (float.TryParse(value, out num) == false)
                //    {
                //        num = 0;
                //    }
                //    return EditorGUILayout.DelayedFloatField(num).ToString();
                //}
                return EditorGUILayout.DelayedTextField(value);
            }
        }

        private class CategoryView : ListView<CatalogueCategoryMetaData>
        {
            private Catalogue m_Catalogue;

            public CategoryView(string name, List<CatalogueCategoryMetaData> categories, Catalogue catalogue) : base(name, categories)
            {
                m_Catalogue = catalogue;
            }

            protected override CatalogueCategoryMetaData DrawElement(CatalogueCategoryMetaData element)
            {
                var categories = m_Catalogue.Description.GetAllSubCategories();
                var category = categories.Find(c => c.Category.Guid == element.CategoryId);
                var selected = EditorHelper.Popup(categories, category, c => c.Path);
                element.CategoryId = selected.Category.Guid;
                return element;
            }

            protected override void OnAdd()
            {
                var categories = m_Catalogue.Description.GetAllSubCategories();
                Add(new CatalogueCategoryMetaData
                {
                    CategoryId = categories.First().Category.Guid
                });
            }
        }
    }
}