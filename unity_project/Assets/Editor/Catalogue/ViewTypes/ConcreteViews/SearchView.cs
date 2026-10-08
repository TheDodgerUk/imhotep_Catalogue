using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools.CatalogueViewer
{
    public class SearchView<T> : ExplorerView<T> where T : class, ISearchableData, new()
    {
        private string m_SearchTerm;

        public SearchView(List<T> list, T selected) : base(null, list, selected) { }

        protected override void OnHeaderGUI()
        {
            base.OnHeaderGUI();

            m_SearchTerm = EditorHelper.SearchBar(m_SearchTerm);
        }

        protected override void OnExplorerGUI()
        {
            if (GUILayout.Button("Add New") == true)
            {
                OnAdd();
            }
        }

        protected override List<T> FilterList(List<T> list)
        {
            if (string.IsNullOrEmpty(m_SearchTerm) == false)
            {
                return list.FindAll(e => e.Matches(m_SearchTerm.ToLower()));
            }
            return base.FilterList(list);
        }

        protected override T DrawElement(T element)
        {
            var style = GetButtonStyle(IsSelected(element));

            var width = GUILayout.Width(ExplorerViewSize.Size - (21 + (24 * 3) + 8));
            string content = element.GetContentDisplay(ExplorerViewSize.Display, ExplorerViewSize.CurrentEnvirmentFilter, ExplorerViewSize.CurrentMetaData);
            if (string.IsNullOrEmpty(content) == false)
            {
                if (GUILayout.Button(content, style, width) == true)
                {
                    OnDataSelected(element);
                }
            }
            return element;
        }

        protected override bool IsElementValid(T element)
        {
            string content = element.GetContentDisplay(ExplorerViewSize.Display, ExplorerViewSize.CurrentEnvirmentFilter, ExplorerViewSize.CurrentMetaData);
            if(ExplorerViewSize.ShowAllData == true)
            {
                return true;
            }
            return !string.IsNullOrEmpty(content);
        }

        protected override void Remove(T item)
        {
            var list = GetList();
            int i = list.IndexOf(item) - 1;

            base.Remove(item);

            if (m_Selected == item)
            {
                OnDataSelected(list[i.Max(0)]);
            }
        }

        protected override void OnAdd()
        {
            AddNew(new T());
        }

        protected void AddNew(T item)
        {
            Add(item);
            m_SearchTerm = string.Empty;
            m_ScrollPos.y = Mathf.Infinity;
            OnDataSelected(item);
        }
    }
}