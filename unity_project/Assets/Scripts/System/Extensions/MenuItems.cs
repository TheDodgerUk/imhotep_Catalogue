#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class MenuItems
{
    [MenuItem("GameObject/Helpers/Print Full Path", false, 0)]
    private static void PrintFullPath()
    {
        var selected = Selection.activeGameObject;
        string name = selected.scene.path.Split('/').Last();
        Debug.LogError($"{name.Replace(".unity", "")} ->  {selected.GetGameObjectPath()}");
    }


    /********************************************************************************/
    [MenuItem("GameObject/Helpers/TaggedPrefabs/Main/Posters", false, 0)]
    private static void Posters() => CreatePrefab("[Posters]_PosterPrefab");

    [MenuItem("GameObject/Helpers/TaggedPrefabs/Main/360Video", false, 0)]
    private static void _360Video() => CreatePrefab("[360Video]_MidasVideo_360Video");



    /********************************************************************************/
    private static void CreatePrefab(string name)
    {
        GameObject parent = Selection.activeGameObject;
        GameObject taggedPrefab = Resources.Load($"TaggedPrefabs/{name}") as GameObject;
        if (taggedPrefab != null)
        {
            GameObject instantiateRef = GameObject.Instantiate(taggedPrefab, Vector3.zero, Quaternion.identity);
            instantiateRef.name = taggedPrefab.name;
            if (null != parent)
            {
                instantiateRef.transform.SetParent(parent.transform);
                instantiateRef.transform.ClearLocals();
            }
            Selection.activeGameObject = instantiateRef;
        }
        else
        {
            EditorUtility.DisplayDialog("Cannot find prefab", $"{name}, called from {nameof(MenuItems)}.cs", "Ok");
        }
    }

    private static void ChangeLayers(int layer)
    {
        var selected = Selection.gameObjects;
        foreach (var item in selected)
        {
            item.layer = layer;
        }
    }
}
#endif