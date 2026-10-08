using System.Collections.Generic;

[MetaData(MetaDataType.Catalogue)]
public class CatalogueMetaData : MetaData
{
    public List<CatalogueCategoryMetaData> Categories;
    public string Brand;

    public CatalogueMetaData()
    {
        Categories = new List<CatalogueCategoryMetaData>();
    }
}

public class CatalogueCategoryMetaData
{
    public string CategoryId;

    public Dictionary<string, string> Filters;

    public CatalogueCategoryMetaData()
    {
        Filters = new Dictionary<string, string>();
    }
}

