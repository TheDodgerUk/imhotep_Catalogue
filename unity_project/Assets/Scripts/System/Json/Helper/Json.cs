using JsonPlugin;
using System;
using System.Collections.Generic;

public static class Json
{
    private static readonly Dictionary<JsonLibraryType, JsonLibrary> Libraries = new Dictionary<JsonLibraryType, JsonLibrary>
    {
        { JsonLibraryType.FullSerializer, new FullSerializerLibrary() },
        { JsonLibraryType.JsonNet, new JsonNetLibrary() },
        { JsonLibraryType.Unity, new UnityLibrary() },
    };
    private const JsonLibraryType DefaultLib = JsonLibraryType.Unity;

    public static readonly JsonLibrary FullSerialiser = Libraries.Get(JsonLibraryType.FullSerializer);
    public static readonly JsonLibrary JsonNet = Libraries.Get(JsonLibraryType.JsonNet);
    public static readonly JsonLibrary Unity = Libraries.Get(JsonLibraryType.Unity);

    public static JsonLibraryType LibraryType { get; private set; }
    public static JsonLibrary Library { get; private set; }

    static Json() { SetLibrary(DefaultLib); }

    public static void SetLibrary(JsonLibraryType libraryType)
    {
        Library = Libraries.Get(libraryType);
        LibraryType = libraryType;
    }
    public static void ResetLibrary() => SetLibrary(DefaultLib);
}

public class JsonLibraryScope : IDisposable
{
    private JsonLibraryType m_LastLibraryType;

    public JsonLibraryScope(JsonLibraryType libraryType)
    {
        m_LastLibraryType = Json.LibraryType;
        Json.SetLibrary(libraryType);
    }
    public void Dispose() => Json.SetLibrary(m_LastLibraryType);
}

public enum JsonLibraryType
{
    FullSerializer,
    JsonNet,
    Unity,
}
