using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SaveScene
{
    public readonly List<SceneSaveObject> Objects;

    public SaveScene()
    {
        Objects = new List<SceneSaveObject>();
    }
}

public class SceneSaveObject
{
    public readonly string RuntimeId;
    public readonly Vector3 Position;
    public readonly Quaternion Rotation;

    public readonly List<JObject> Children;

    public readonly string CatalogueId;
    public readonly List<string> Modules;
    [Newtonsoft.Json.JsonProperty(PropertyName = "$type")]
    public readonly string type;
}
