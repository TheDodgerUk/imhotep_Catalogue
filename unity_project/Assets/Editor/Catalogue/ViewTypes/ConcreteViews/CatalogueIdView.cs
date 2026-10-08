using EditorTools.Views;
using System;
using System.Collections.Generic;

namespace EditorTools.CatalogueViewer
{
    public class CatalogueIdView : UniqueListView<string>
    {
        private readonly Dictionary<string, CatalogueEntry> m_EntriesByName;
        private readonly Dictionary<string, CatalogueEntry> m_EntriesById;

        public CatalogueIdView(string name, Catalogue catalogue, List<string> ids, Predicate<CatalogueEntry> predicate) : base(name, ids, GetIds(catalogue, predicate))
        {
            m_EntriesByName = catalogue.Entries.ExtractAsValues(e => e.FullName.Text);
            m_EntriesById = catalogue.Entries.ExtractAsValues(e => e.Guid);
        }

        protected override string DrawElement(string id)
        {
            string current = IdToName(id);
            var available = Unique.GetAvailableFor(id).Extract(IdToName);

            return NameToId(EditorHelper.Popup(current, available));
        }

        private string IdToName(string id) => m_EntriesById[id].FullName.Text;
        private string NameToId(string name) => m_EntriesByName[name].Guid;

        private static List<string> GetIds(Catalogue catalogue, Predicate<CatalogueEntry> predicate)
        {
            return catalogue.Entries.FindAll(predicate).Extract(c => c.Guid);
        }
    }
}