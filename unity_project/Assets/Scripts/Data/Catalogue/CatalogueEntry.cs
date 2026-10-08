using System;
using System.Collections.Generic;
using System.Linq;

public class CatalogueEntry : ISearchableData
{
    public class SubItems
    {
        [System.NonSerialized]
        public UnityEngine.GameObject m_GameObject;

        [UnityEngine.SerializeField]
        public string m_ObjectName;

        [UnityEngine.SerializeField]
        public string m_GUID;
    }

    private const string LocalisationKey = Catalogue.LocalisationKey + "_entry";

    public LocalisedValue ShortName { get; private set; }
    public LocalisedValue FullName { get; private set; }

    public string ReferenceNumber;
    public readonly string Guid;
    public readonly List<MetaAttribute> Attributes;
    public readonly Dictionary<MetaDataType, MetaData> MetaData;

    public int Version;
    public bool HasVideo = false;
    public bool FlipThumbnail = false;
    public bool IncludeInUnitTests = true;

    public List<string> EnvironmentsAvailable = new List<string>();
    public List<string> CatalogueFilters = new List<string>();

    public List<SubItems> m_SubItems = new List<SubItems>();

    public CatalogueEntry()
    {
        Guid = Utils.Code.GenerateGuid();
        Attributes = new List<MetaAttribute>();
        MetaData = new Dictionary<MetaDataType, MetaData>();

        OnPostLoad();
    }

    public void OnPostLoad()
    {
        ShortName = new LocalisedValue(LocalisationKey, Guid, "short");
        FullName = new LocalisedValue(LocalisationKey, Guid, "full");
        foreach (var metaData in MetaData.Values)
        {
            metaData.OnPostLoad(this);
        }
    }

    public void OnPreSave()
    {
        FullName.Flush();
        ShortName.Flush();

        foreach (var metaData in MetaData.Values)
        {
            metaData.OnPreSave(this);
        }
    }

    public List<MetaDataType> GetMetaDataTypes() => MetaData.Keys.ToList();

    public T GetMetaData<T>() where T : class
    {
        var enumType = typeof(T).GetEnumType();
        if (enumType != null)
        {
            return MetaData.Get(enumType.Value) as T;
        }
        return null;
    }

    public MetaData AddMetaData(MetaDataType enumType)
    {
        var type = enumType.GetDataType();
        if (type != null)
        {
            var data = Activator.CreateInstance(type) as MetaData;
            data.OnPostLoad(this);
            MetaData.Add(enumType, data);

            var keys = MetaData.Keys.ToList();
            keys.Sort();

            var clone = MetaData.Clone();
            MetaData.Clear();

            foreach (var key in keys)
            {
                MetaData.Add(key, clone[key]);
            }
            return data;
        }
        else
        {
            UnityEngine.Debug.LogError($"Could not add MetaData of type \"{enumType}\". There is no matching C# type.\n");
            return null;
        }
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
        if ((filter == "ALL" || EnvironmentsAvailable.IndexOf(filter) != -1 ) && (metaDataType == MetaDataType.NOTHING || MetaData.ContainsKey(metaDataType) == true))
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
        return string.Empty;
    }
}

