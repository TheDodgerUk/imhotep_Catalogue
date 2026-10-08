using EditorTools.Views;
using System.Collections.Generic;
using UnityEditor;

namespace EditorTools.CatalogueViewer
{
    public class CatalogueFilterView : IListItemViewable
    {
        private CatalogueFilterValueView m_ValueView;
        private CatalogueFilter m_Selected;

        public CatalogueFilterView(CatalogueFilter filter)
        {
            SetSelected(filter);
        }

        public void SetSelected(CatalogueFilter filter)
        {
            m_ValueView = new CatalogueFilterValueView(filter.Values);
            m_Selected = filter;
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                Utils.View.DrawGuid(m_Selected.Guid);

                m_Selected.Name.Text = EditorGUILayout.TextField("Name", m_Selected.Name.Text);

                m_ValueView.FilterType = EditorHelper.EnumPopup("Filter Type", m_Selected.GetFilterType());
                m_ValueView.OnGUI();
            }
        }

        private class CatalogueFilterValueView : DefaultListView<CatalogueFilterValue>
        {
            public FilterType FilterType;

            public CatalogueFilterValueView(List<CatalogueFilterValue> values) : base("Values", values) { }

            protected override CatalogueFilterValue DrawElement(CatalogueFilterValue element)
            {
                if (FilterType == FilterType.Value)
                {
                    DrawValue(element);
                }
                if (FilterType == FilterType.Range)
                {
                    DrawRange(element);
                }
                return element;
            }
        }

        private static void DrawValue(CatalogueFilterValue v)
        {
            v.Min = null;
            v.Max = null;

            v.Value = EditorGUILayout.TextField("Value", v.Value);
        }

        private static void DrawRange(CatalogueFilterValue v)
        {
            v.Value = null;

            float min = 0;
            float max = 0;
            using (new EditorGUILayout.HorizontalScope())
            {
                min = EditorGUILayout.DelayedFloatField("Min", v.Min ?? 0);
                using (new LabelWidthScope(52))
                {
                    max = EditorGUILayout.DelayedFloatField("Max", v.Max ?? 0);
                }
            }

            const float eps = 0.1f;
            if (max <= min && max != v.Max)
            {
                min = max - eps;
            }
            if (min >= max && min != v.Min)
            {
                max = min + eps;
            }
            if (min == max)
            {
                max = min + eps;
            }
            v.Min = min;
            v.Max = max;
        }
    }
}