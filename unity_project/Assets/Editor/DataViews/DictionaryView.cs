using System.Collections.Generic;
using System.Linq;

namespace EditorTools.Views
{
    public abstract class DictionaryView<TKey, TValue> : ListView<Tuple<TKey, TValue>>
    {
        public GuiLayoutDirection Direction = GuiLayoutDirection.Horizontal;

        private Dictionary<TKey, TValue> m_Dictionary;
        private UniqueValueGatherer<TKey> m_Unique;
        private List<TKey> m_AllKeys;

        public DictionaryView(string name, Dictionary<TKey, TValue> dictionary, List<TKey> allKeys) : base(name, dictionary?.ExtractAsTuples())
        {
            m_Dictionary = dictionary ?? new Dictionary<TKey, TValue>();
            m_AllKeys = allKeys;
        }

        public override void OnGUI()
        {
            m_Unique = new UniqueValueGatherer<TKey>(m_Dictionary.Keys.ToList(), m_AllKeys);
            m_IsFull = m_Unique.HasAvailableValue();
            m_Dictionary.Clear();

            base.OnGUI();

            foreach (var tuple in GetList())
            {
                m_Dictionary.Add(tuple.A, tuple.B);
            }
        }

        protected override Tuple<TKey, TValue> DrawElement(Tuple<TKey, TValue> tuple)
        {
            using (new GuiLayoutDirectionScope(Direction))
            {
                var indent = new GuiIndentScope();
                tuple.A = DrawKey(tuple.A, m_Unique.GetAvailableFor(tuple.A));

                if (Direction == GuiLayoutDirection.Horizontal) { indent.Dispose(); }
                tuple.B = DrawValue(tuple.A, tuple.B);

                if (Direction == GuiLayoutDirection.Vertical) { indent.Dispose(); }
            }

            return tuple;
        }

        protected abstract TKey DrawKey(TKey key, List<TKey> availableKeys);
        protected abstract TValue DrawValue(TKey key, TValue value);

        protected override void OnAdd()
        {
            if (m_Unique.HasAvailableValue() == true)
            {
                var key = m_Unique.GetAvailableValue();
                Add(Tuple.New(key, default(TValue)));
            }
        }
    }
}