using System.Collections.Generic;

public class CatalogueDescription
{
    public CatalogueCategory RootCategory;
    public List<string> Brands;

    public void OnPreSave()
    {
        RootCategory.OnPreSave();
    }
    public void OnPostLoad()
    {
        RootCategory.OnPostLoad();
    }

    public List<CatalogueCategoryPath> GetAllSubCategories()
    {
        return RootCategory.GetAllSubCategories(new List<CatalogueCategory>());
    }
}
