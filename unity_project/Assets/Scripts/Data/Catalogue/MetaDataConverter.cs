using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

public class MetaDataConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(Dictionary<MetaDataType, MetaData>).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var properties = jsonObject.Properties().ToList();
        int count = properties.Count;

        var dict = new Dictionary<MetaDataType, MetaData>(count);
        for (int i = 0; i < count; ++i)
        {
            var name = properties[i].Name;
            if (name.IsEnum<MetaDataType>() == true)
            {
                var metaType = name.ParseEnum<MetaDataType>();
                var dataType = metaType.GetDataType();

                if (dataType != null)
                {
                    string str = jsonObject[name].ToString(Formatting.None);
                    var data = Json.JsonNet.ReadFromText(str, dataType);
                    var metaData = data as MetaData;
                    if (metaData != null)
                    {
                        dict.Add(metaType, metaData);
                    }
                    else
                    {
                        UnityEngine.Debug.LogError($"Failed to cast from [{dataType}] to [MetaData].\n");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogError($"Failed to parse MetaData. Type (C#) for [{metaType}] does not exist.\n");
                }
            }
            else
            {
                UnityEngine.Debug.LogError($"Failed to parse MetaData. MetaDataType (Enum) [{name}] does not exist.\n");
            }
        }
        return dict;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value);
    }
}
