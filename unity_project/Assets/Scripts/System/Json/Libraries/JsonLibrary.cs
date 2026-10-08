using System;
using System.IO;
using UnityEngine;

namespace JsonPlugin
{
    public abstract class JsonLibrary
    {
        public T ReadFromFile<T>(string filePath) where T : class
        {
            return ReadFromText<T>(ReadFile(filePath));
        }
        public object ReadFromFile(string filePath, Type type)
        {
            return ReadFromText(ReadFile(filePath), type);
        }
        public bool FillFromFile(string filePath, object obj)
        {
            return FillFromText(ReadFile(filePath), obj);
        }

        public T ReadFromText<T>(string jsonText)
        {
            return TryDeserialise(typeof(T), jsonText, () => DeserialiseData<T>(jsonText));
        }
        public object ReadFromText(string jsonText, Type type)
        {
            return TryDeserialise(type, jsonText, () => DeserialiseData(jsonText, type));
        }
        public bool FillFromText(string jsonText, object obj)
        {
            return TryDeserialise(obj.GetType(), jsonText, () => DeserialiseData(jsonText, obj));
        }

        public void WriteToFile(object data, string filePath, bool prettyPrint)
        {
            filePath = AppendExtension(filePath);
            string jsonText = WriteToText(data, prettyPrint);

            try
            {
                Utils.IO.ForceFolder(Path.GetDirectoryName(filePath));
                File.WriteAllText(filePath, jsonText);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to write text to path \"{filePath}\"\n\"{e.Message}\" text: {jsonText}");
            }
        }

        public string WriteToText(object data, bool prettyPrint)
        {
            try
            {
                return SerialiseData(data, prettyPrint);
            }
            catch (Exception e)
            {
                Debug.Log($"Failed to serialise JSON to type {{{data.GetType().Name}}}.\n\"{e.Message}\" from data:\n{data.ToString()}");
            }
            return null;
        }

        protected abstract T DeserialiseData<T>(string jsonText);
        protected abstract object DeserialiseData(string jsonText, Type type);
        protected abstract bool DeserialiseData(string jsonText, object obj);

        protected abstract string SerialiseData(object data, bool prettyPrint);

        private static T TryDeserialise<T>(Type type, string text, Func<T> deserialise)
        {
            try
            {
                return deserialise();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON to type: {{{typeof(T).Name}}}.\n\"{e.Message}\" from JSON:\n{text}");
            }
            return default(T);
        }

        private static string ReadFile(string filePath) => Utils.IO.ReadTextFromFile(AppendExtension(filePath));

        private static string AppendExtension(string filePath)
        {
            filePath = filePath.SanitiseSlashes();

            int index = filePath.IndexOf(".");
            if (index >= 0)
            {
                filePath = filePath.Substring(0, index);
            }

            filePath += ".json";
            return filePath;
        }
    }
}
