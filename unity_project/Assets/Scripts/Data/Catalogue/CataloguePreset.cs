
public class CataloguePreset : ISearchableData
{
    private const string LocalisationKey = Catalogue.LocalisationKey + "_preset";

    public LocalisedValue ShortName { get; private set; }
    public LocalisedValue FullName { get; private set; }

    public string ReferenceNumber;
    public string Guid;
    public SaveScene Contents;

    public CataloguePreset()
    {
        Guid = Utils.Code.GenerateGuid();
        Contents = new SaveScene();
        OnPostLoad();
    }

    public void OnPostLoad()
    {
        ShortName = new LocalisedValue(LocalisationKey, Guid, "short");
        FullName = new LocalisedValue(LocalisationKey, Guid, "full");
    }

    public void OnPreSave()
    {
        FullName.Flush();
        ShortName.Flush();
    }

    public bool Matches(string searchTerm)
    {
        if (FullName.Text.ToLower().Contains(searchTerm) == true)
        {
            return true;
        }
        else if (ShortName.Text.ToLower().Contains(searchTerm) == true)
        {
            return true;
        }
        else if (Guid.Contains(searchTerm) == true)
        {
            return true;
        }
        return false;
    }

    public string GetContentDisplay(SearchDisplayMode mode, string filter, MetaDataType metaDataType)
    {
        switch (mode)
        {
            case SearchDisplayMode.Id: return Guid;
            case SearchDisplayMode.FullName: return FullName.Text;
            case SearchDisplayMode.ShortName: return ShortName.Text;
            case SearchDisplayMode.Both: return $"{Guid} - {FullName}";
            case SearchDisplayMode.Reference: return ReferenceNumber;
            default: return string.Empty;
        }
    }
}

