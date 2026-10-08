using System;
using System.Collections.Generic;

namespace EditorTools.Views
{
    public abstract class ListItemView<TItem, TView> : ListView<TItem> where TView : IListItemViewable
    {
        private Dictionary<TItem, TView> m_Views;

        private Func<TItem, TView> m_ViewConstructor;

        public ListItemView(string name, List<TItem> list, Func<TItem, TView> viewConstructor) : base(name, list)
        {
            m_ViewConstructor = viewConstructor;

            m_Views = list.ExtractAsKeys(viewConstructor);
        }

        protected override TItem DrawElement(TItem element)
        {
            m_Views[element].OnGUI();
            return element;
        }

        protected override void Add(TItem item)
        {
            base.Add(item);

            m_Views.Add(item, m_ViewConstructor(item));
        }

        protected override void Remove(TItem item)
        {
            base.Remove(item);

            m_Views.Remove(item);
        }
    }

    public abstract class DefaultListItemView<TItem, TView> : ListItemView<TItem, TView> where TItem : new() where TView : IListItemViewable
    {
        public DefaultListItemView(string name, List<TItem> list, Func<TItem, TView> viewConstructor) : base(name, list, viewConstructor) { }

        protected override void OnAdd()
        {
            Add(new TItem());
        }
    }

    public interface IListItemViewable
    {
        void OnGUI();
    }
}