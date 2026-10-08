using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ReflectionExtensions
{
    public static bool IsInUserAssembly(this Type type) => type.Assembly.IsUserAssembly();

    public static bool IsUserAssembly(this Assembly assembly)
    {
        return assembly.FullName.StartsWith("Assembly-CSharp");
    }

    public static bool ImplementsInterface<TInterface>(this Type type)
    {
        return typeof(TInterface).IsAssignableFrom(type);
        //return type.GetInterfaces().ToList().Contains(typeof(TInterface));
    }

    public static Type GetGenericBaseType(this Type type)
    {
        if (type.IsGenericType == false)
        {
            if (type.BaseType != null)
            {
                return GetGenericBaseType(type.BaseType);
            }
            return null;
        }
        return type;
    }

    public static List<Type> GetInterfaceImplementors(this Type type)
    {
        return type.Assembly.GetTypes().Where(t => t.GetInterfaces().Contains(type)).ToList();
    }

    public static List<Type> GetConcreteChildren(this Type type)
    {
        return Utils.Reflection.CollectUserTypes(t => !t.IsAbstract && t.IsOfType(type));
    }

    public static List<Type> GetConcreteChildren(this Type type, Type genericArgument)
    {
        if (type.IsGenericType == true)
        {
            return Utils.Reflection.CollectUserTypes(t =>
            {
                if (t.IsAbstract == false && t.IsOfType(type) == true)
                {
                    var genericBaseType = t.GetGenericBaseType();
                    var args = genericBaseType.GetGenericArguments();
                    return args.Contains(genericArgument);
                }
                return false;
            });
        }
        return null;
    }

    public static bool IsOfType<T>(this Type type) => type.IsOfType(typeof(T));

    public static bool IsOfType(this Type type, Type targetType)
    {
        if (type == targetType)
        {
            return true;
        }
        else if (type.BaseType != null)
        {
            if (type.BaseType.IsGenericType == true)
            {
                var genericType = type.BaseType.GetGenericTypeDefinition();
                return genericType.IsOfType(targetType);
            }
            else
            {
                return type.BaseType.IsOfType(targetType);
            }
        }
        return false;
    }

    public static List<Type> GetBaseTypes(this Type type, Type stopAt = null)
    {
        var types = new List<Type> { type };
        if (type.BaseType != null && type.BaseType != stopAt)
        {
            types.InsertRange(0, type.BaseType.GetBaseTypes(stopAt));
        }
        return types;
    }

    public static T GetAttribute<T>(this Type type) where T : Attribute
    {
        return type.GetCustomAttributes(false).OfType<T>().SingleOrDefault();
    }

    public static T AddComponentCopy<T>(this GameObject game, T duplicate) where T : Component
    {
        T target = game.AddComponent<T>();
        foreach (PropertyInfo x in typeof(T).GetProperties())
            if (x.CanWrite)
                x.SetValue(target, x.GetValue(duplicate));
        return target;
    }

}
