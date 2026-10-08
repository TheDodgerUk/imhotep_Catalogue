using System.Collections.Generic;

public enum FilterType { Value, Range };

public class CatalogueFilter : ISearchableData
{
    public LocalisedValue Name { get; private set; }


    public string ReferenceNumber;
    public string Guid;
    public List<CatalogueFilterValue> Values;

    public CatalogueFilter()
    {
        Guid = Utils.Code.GenerateGuid();
        Values = new List<CatalogueFilterValue>();
        OnPostLoad();
    }

    public void OnPreSave()
    {
        Name.Flush();
    }

    public void OnPostLoad()
    {
        Name = new LocalisedValue($"{Catalogue.LocalisationKey}_filter", Guid);
    }

    public FilterType GetFilterType()
    {
        bool isValue = Values.Exists(v => v.Max == null || v.Min == null);
        return (isValue ? FilterType.Value : FilterType.Range);
    }

    public bool Matches(string searchTerm)
    {
        if (Guid.ToLower().Contains(searchTerm) == true)
        {
            return true;
        }
        if (Name.Text.ToLower().Contains(searchTerm) == true)
        {
            return true;
        }
        return false;
    }

    public string GetContentDisplay(SearchDisplayMode mode, string filter,  MetaDataType metaDataType)
    {
        switch (mode)
        {
            case SearchDisplayMode.Id: return Guid;
            case SearchDisplayMode.FullName: return Name.Text;
            case SearchDisplayMode.ShortName: return Name.Text;
            case SearchDisplayMode.Both: return $"{Guid} - {Name}";
            case SearchDisplayMode.Reference: return ReferenceNumber;
            default: return string.Empty;
        }
    }
}

public class CatalogueFilterValue
{
    public string Value;
    public float? Min;
    public float? Max;
}
