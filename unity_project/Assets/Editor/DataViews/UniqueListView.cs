using System.Collections.Generic;

namespace EditorTools.Views
{
    public abstract class UniqueListView<T> : ListView<T>
    {
        protected UniqueValueGatherer<T> Unique => m_Unique;
        private UniqueValueGatherer<T> m_Unique;
        private List<T> m_AllValues;

        public UniqueListView(string name, List<T> list, List<T> allValues) : base(name, list)
        {
            m_AllValues = allValues;
        }

        public void RemoveFromList(T item)
        {
            m_AllValues.Remove(item);
        }

        public override void OnGUI()
        {
            m_Unique = new UniqueValueGatherer<T>(GetList().Clone(), m_AllValues);
            m_IsFull = m_Unique.HasAvailableValue();

            base.OnGUI();
        }

        protected override void OnAdd()
        {
            Add(Unique.GetAvailableValue());
        }
    }

    public class UniqueValueGatherer<T>
    {
        private List<T> m_Active;
        private List<T> m_All;

        public UniqueValueGatherer(List<T> active, List<T> all)
        {
            m_Active = active;
            m_All = all;
        }

        public List<T> GetAvailableFor(T current)
        {
            return m_All.FindAll(t => Compare(t, current) || !m_Active.Contains(t));
        }

        public T GetAvailableValue()
        {
            return m_All.Find(t => !m_Active.Contains(t));
        }

        public bool HasAvailableValue()
        {
            return m_All.Exists(t => !m_Active.Contains(t));
        }

        private static bool Compare(T x, T y) => EqualityComparer<T>.Default.Equals(x, y);
    }
}