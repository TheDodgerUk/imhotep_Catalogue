
public enum SearchDisplayMode { Id, FullName, ShortName, Both , Reference }

public interface ISearchableData
{
    string GetContentDisplay(SearchDisplayMode mode, string filter, MetaDataType metaDataType);
    bool Matches(string search);
}
