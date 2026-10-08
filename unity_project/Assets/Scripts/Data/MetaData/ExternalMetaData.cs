using Newtonsoft.Json.Linq;

public class ExternalMetaData
{
    public string ExternalPath { get; private set; }
    public JObject JsonObject { get; set; }

    public ExternalMetaDataType Type;

    public ExternalMetaData(ExternalMetaDataType externalMetaDataType)
    {
        Type = externalMetaDataType;
    }

    public void LoadJson(CatalogueEntry entry)
    {
        ExternalPath = Catalogue.GetExternalMetaDataPath(entry.Guid, Type);
        LoadJson();
    }

    public void LoadJson()
    {
        if (Utils.IO.FileExists($"{ExternalPath}.json") == true)
        {
            JsonObject = Json.JsonNet.ReadFromFile<JObject>(ExternalPath);
        }
        else
        {
            JsonObject = null;
        }
    }

    public void SaveJson()
    {
        Json.JsonNet.WriteToFile(JsonObject, ExternalPath, true);
    }
}

