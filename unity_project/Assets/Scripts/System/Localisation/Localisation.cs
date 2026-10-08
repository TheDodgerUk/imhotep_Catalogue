using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

public class Localisation : Singleton<Localisation>
{
    public class Ordered
    {
        public string Key;
        public string Value;
        public Ordered(string inputKey, string inputValue)
        {
            Key = inputKey;
            Value = inputValue;
        }
    }


    private Dictionary<string, string> m_Dictionary;

    public string this[string key]
    {
        get { return Get(key); }
        set { Set(key, value); }
    }

    public void LoadLanguage(Language language)
    {
        string path = GetLanguagePath(language);
        m_Dictionary = Json.JsonNet.ReadFromFile<Dictionary<string, string>>(path);

        List<Ordered> newOrder = new List<Ordered>();

        foreach (var entry in m_Dictionary)
        {
            newOrder.Add(new Ordered(entry.Key, entry.Value));
        }

    }

    public void SaveLanguage(Language language)
    {
        List<Ordered> newOrder = new List<Ordered>();

        OrderedDictionary orderedDictionary = new OrderedDictionary();
        foreach (var entry in m_Dictionary)
        {
            newOrder.Add(new Ordered(entry.Key, entry.Value));
        }
        newOrder = newOrder.OrderBy(e => e.Key).ToList();

        foreach (var entry in newOrder)
        {
            orderedDictionary.Add(entry.Key, entry.Value);
        }   

        // this will save it in alphabetical order, so we wont get the weird order changing when commit
        Json.Library.WriteToFile(orderedDictionary, GetLanguagePath(language), true);
    }

    public void PurgeKeys(string startsWith)
    {
        var keys = new List<string>(m_Dictionary.Keys);
        for (int i = 0; i < keys.Count; ++i)
        {
            if (keys[i].ToLower().StartsWith(startsWith.ToLower()) == true)
            {
                m_Dictionary.Remove(keys[i]);
                keys.RemoveAt(i);
                --i;
            }
        }
    }

    public void VerifyData(List<CatalogueEntry> entries)
    {
        UnityEngine.Debug.LogError("Fill in ");
    }

    public string Get(string key) => m_Dictionary.Get(key) ?? string.Empty;

    public void Set(string key, string value)
    {
        m_Dictionary[key] = value;
    }

    private static string GetLanguagePath(Language language)
    {
        return $"{GlobalConsts.BUNDLE_STREAMING_LANGUAGES}/{language.ToString().ToLower()}";
    }

    public enum Language { English };
}
