using FullSerializer;
using System;

namespace JsonPlugin
{
    public class FullSerializerLibrary : JsonLibrary
    {
        protected override T DeserialiseData<T>(string jsonText)
        {
            return (T)Deserialise(jsonText, default(T), typeof(T));
        }

        protected override object DeserialiseData(string jsonText, Type type)
        {
            return Deserialise(jsonText, null, type);
        }

        protected override bool DeserialiseData(string jsonText, object obj)
        {
            Deserialise(jsonText, obj, obj.GetType());
            return true;
        }

        protected override string SerialiseData(object data, bool prettyPrint)
        {
            fsData fsData;
            var serialiser = new fsSerializer();
            var result = serialiser.TrySerialize(data, out fsData);
            result.AssertSuccessWithoutWarnings();

            if (prettyPrint == true)
            {
                return fsJsonPrinter.PrettyJson(fsData);
            }
            else
            {
                return fsJsonPrinter.CompressedJson(fsData);
            }
        }

        private static object Deserialise(string jsonText, object obj, Type type)
        {
            if (obj == null)
            {
                obj = null;
            }
            var serialiser = new fsSerializer();
            var jsonData = fsJsonParser.Parse(jsonText);
            var result = serialiser.TryDeserialize(jsonData, obj.GetType(), ref obj);
            result.AssertSuccessWithoutWarnings();
            return obj;
        }
    }
}
