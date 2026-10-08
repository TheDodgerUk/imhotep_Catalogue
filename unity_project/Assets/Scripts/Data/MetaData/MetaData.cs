using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SerializeField]
public class GameObjectData
{
    [System.NonSerialized]
    public GameObject GameObjectRef;
    [SerializeField]
    public string GameObjectName;

    public virtual void CollectAllData(GameObject root)
    {
        if (string.IsNullOrEmpty(GameObjectName) == false)
        {
            GameObjectRef = MetaData.StringToGameObject(root, GameObjectName, false);
        }
    }
}

[SerializeField]
public class AnimationGameObjectData : GameObjectData
{
    [SerializeField]
    public string AnimationName;
}
[SerializeField]
public class RendererGameObjectData : GameObjectData
{
    [System.NonSerialized]
    public Renderer RendererObj;

    public override void CollectAllData(GameObject root)
    {
        base.CollectAllData(root);
        if (GameObjectRef != null)
        {
            RendererObj = GameObjectRef.GetComponent<Renderer>();
        }
    }
}

[SerializeField]
public class ColliderGameObjectData : GameObjectData
{
    [System.NonSerialized]
    public Collider ColliderObj;

    public override void CollectAllData(GameObject root)
    {
        base.CollectAllData(root);
        if (GameObjectRef != null)
        {
            ColliderObj = GameObjectRef.GetComponent<Collider>();
        }
    }
}

public enum MetaDataType
{
    NOTHING,
    Catalogue,
    Render,
    ContentLight,
    ContentPlacement,
    ContentPivot,
    ContentAudio,
    ContentAnimation,
    ContentMaterialChange,
    ContentScale,
    ContentAdaptive,
    ContentGameObjectCycle,
    ContentScript,
    ContentSteamAudio,
    ContentModular,
    ContentLighting,
    ContentSlider,
    ContentButton,
    ContentDial,
    ContentClick,
    ArFeatures,
    ContentStaging,
    ContentVideoPlayer,
    ContentPickUp,
    ContentParticleSystem,
    ContentDrink,
    ContentFood,
    ContentPickUpSocket,
    ContentPickUpCable,
    ContentHinge,
    ContentHasRigidBody,
    ContentCollision,
}

public enum ExternalMetaDataType
{
    MlfLoudspeaker,
    MlfMechanical,
}

public enum MetaAttribute
{
    Moveable,
}

public abstract class MetaData
{
    public virtual void OnPreSave(CatalogueEntry entry) { }
    public virtual void OnPostLoad(CatalogueEntry entry) { }


    public virtual void CollectAllData(UnityEngine.GameObject root) { }
    public static  UnityEngine.GameObject StringToGameObject(UnityEngine.GameObject root, string name, bool showError = true)
    {
        if (string.IsNullOrEmpty(name) == true)
        {
            if (showError == true)
            {
                UnityEngine.Debug.LogError($"StringToGameObject name was null, on  {root.name}");
            }
            return null;
        }
        if(string.Equals(root.name, name, StringComparison.CurrentCultureIgnoreCase) == true)
        {
            return root;
        }
        var found = root?.transform?.Search(name);
        if (found != null)
        {
            return found.gameObject;
        }
        else
        {
            UnityEngine.Debug.LogError($"Cannot find name: {name}, on GameObject: {root.name}", root);
            return null;
        }
    }

}

public class MetaDataAttribute : Attribute
{
    public MetaDataType MetaType { get; }
    public MetaDataAttribute(MetaDataType enumType)
    {
        MetaType = enumType;
    }
}

public static class MetaDataExtensions
{
    private static readonly Dictionary<MetaDataType, Type> EnumToType = GetEnumToTypeMap();
    private static readonly Dictionary<Type, MetaDataType> TypeToEnum = EnumToType.Inverse();

    public static Type GetDataType(this MetaDataType enumType) => EnumToType.Get(enumType);
    public static MetaDataType? GetEnumType(this Type type) => TypeToEnum.TryGet(type);

    private static MetaDataType? GetMetaDataType(this Type type) => type.GetAttribute<MetaDataAttribute>()?.MetaType;

    private static Dictionary<MetaDataType, Type> GetEnumToTypeMap()
    {
        var allTypes = typeof(MetaDataType).Assembly.GetTypes().ToList();
        var pairs = allTypes.Select(t => new { Enum = t.GetMetaDataType(), Type = t });
        return pairs.Where(p => p.Enum != null).ToDictionary(p => p.Enum.Value, p => p.Type);
    }
}
