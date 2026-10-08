using System;
using UnityEngine;

namespace JsonPlugin
{
    public class UnityLibrary : JsonLibrary
    {
        protected override T DeserialiseData<T>(string jsonText)
        {
            return JsonUtility.FromJson<T>(jsonText);
        }

        protected override object DeserialiseData(string jsonText, Type type)
        {
            return JsonUtility.FromJson(jsonText, type);
        }

        protected override bool DeserialiseData(string jsonText, object obj)
        {
            JsonUtility.FromJsonOverwrite(jsonText, obj);
            return true;
        }

        protected override string SerialiseData(object data, bool prettyPrint)
        {
            return JsonUtility.ToJson(data, prettyPrint);
        }
    }
}
