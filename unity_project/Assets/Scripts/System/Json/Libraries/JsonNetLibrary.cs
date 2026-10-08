using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace JsonPlugin
{
    public class JsonNetLibrary : JsonLibrary
    {
        protected override T DeserialiseData<T>(string jsonText)
        {
            return JsonConvert.DeserializeObject<T>(jsonText, GetDeserialiseSettings());
        }

        protected override object DeserialiseData(string jsonText, Type type)
        {
            return JsonConvert.DeserializeObject(jsonText, type, GetDeserialiseSettings());
        }

        protected override bool DeserialiseData(string jsonText, object obj)
        {
            JsonConvert.PopulateObject(jsonText, obj, GetDeserialiseSettings());
            return true;
        }

        protected override string SerialiseData(object data, bool prettyPrint)
        {
            return JsonConvert.SerializeObject(data, GetSerialiseSettings(prettyPrint));
        }

        private static JsonSerializerSettings GetDeserialiseSettings()
        {
            return new JsonSerializerSettings
            {
                Converters = { new StringEnumConverter(), new MetaDataConverter() },
                ContractResolver = new FieldsOnlyContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        private static JsonSerializerSettings GetSerialiseSettings(bool prettyPrint)
        {
            return new JsonSerializerSettings
            {
                Formatting = prettyPrint ? Formatting.Indented : Formatting.None,
                Converters = { new StringEnumConverter() },
                ContractResolver = new FieldsOnlyContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        private class FieldsOnlyContractResolver : DefaultContractResolver
        {
            public FieldsOnlyContractResolver() { }

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                property.Writable = CanSetMemberValue(member, false);
                return property;
            }

            public static bool CanSetMemberValue(MemberInfo member, bool nonPublic)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Field:
                        FieldInfo fieldInfo = (FieldInfo)member;

                        if (fieldInfo.IsLiteral)
                        {
                            return false;
                        }
                        if (nonPublic)
                        {
                            return true;
                        }
                        if (fieldInfo.IsPublic)
                        {
                            return true;
                        }
                        return false;
                    case MemberTypes.Property:
                        PropertyInfo propertyInfo = (PropertyInfo)member;

                        if (!propertyInfo.CanWrite)
                        {
                            return false;
                        }
                        if (nonPublic)
                        {
                            return true;
                        }
                        return (propertyInfo.GetSetMethod(nonPublic) != null);
                    default:
                        return false;
                }
            }

            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var members = base.GetSerializableMembers(objectType);
                members.RemoveAll(m => m is PropertyInfo);
                return members;
            }
        }
    }
}
