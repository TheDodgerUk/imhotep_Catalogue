using System.Collections.Generic;

public class CatalogueCategory : ISearchableData
{
    public LocalisedValue Name { get; private set; }


    public string ReferenceNumber;
    public string Guid;
    public List<CatalogueCategory> SubCategories;
    public List<CatalogueFilter> Filters;

    public CatalogueCategory()
    {
        SubCategories = new List<CatalogueCategory>();
        Filters = new List<CatalogueFilter>();

        Guid = Utils.Code.GenerateGuid();
        OnPostLoad();
    }

    public void OnPreSave()
    {
        Name.Flush();

        foreach (var subCategory in SubCategories)
        {
            subCategory.OnPreSave();
        }
        foreach (var filter in Filters)
        {
            filter.OnPreSave();
        }
    }

    public void OnPostLoad()
    {
        Name = new LocalisedValue($"{Catalogue.LocalisationKey}_category", Guid);

        foreach (var subCategory in SubCategories)
        {
            subCategory.OnPostLoad();
        }
        foreach (var filter in Filters)
        {
            filter.OnPostLoad();
        }
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
        if (SubCategories.Exists(c => c.Matches(searchTerm) == true))
        {
            return true;
        }
        if (Filters.Exists(f => f.Matches(searchTerm) == true))
        {
            return true;
        }
        return false;
    }

    public string GetContentDisplay(SearchDisplayMode mode, string filter, MetaDataType metaDataType)
    {
        if (filter == "ALL" || Filters.Find(e => e.Name.Text == filter) != null)
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
        return string.Empty;
    }

    public List<CatalogueCategoryPath> GetAllSubCategories(List<CatalogueCategory> parents)
    {
        parents.Add(this);

        var tree = new List<CatalogueCategoryPath>();
        foreach (var sub in SubCategories)
        {
            if (sub.SubCategories.Count == 0)
            {
                tree.Add(new CatalogueCategoryPath(sub, parents.Clone()));
            }
            else
            {
                tree.AddRange(sub.GetAllSubCategories(parents.Clone()));
            }
        }
        return tree;
    }
}

public class CatalogueCategoryPath
{
    public CatalogueCategory Category;
    public string Path;

    public CatalogueCategoryPath(CatalogueCategory category, List<CatalogueCategory> parents)
    {
        Category = category;
        Path = ConstructPath(category, parents);
    }

    private static string ConstructPath(CatalogueCategory category, List<CatalogueCategory> parents)
    {
        if (parents.Count == 0)
        {
            return category.Name.Text;
        }
        else
        {
            var last = parents.Pop();
            return $"{last.Name}/{ConstructPath(category, parents)}";
        }
    }
}
