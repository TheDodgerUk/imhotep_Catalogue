#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class CreateColliders
{
    public enum ColliderType
    {
        Box,
        Mesh,
    }

    [MenuItem("GameObject/Helpers/MakeGPUInstanceing", false, 0)]
    public static void MakeGPUInstanceing()
    {
        GameObject root = Selection.activeGameObject;
        var renderers = root.GetComponentsInChildren<Renderer>(true);
        foreach (var render in renderers)
        {
            foreach (var mat in render.materials)
            {
                if (mat.enableInstancing == false)
                {
                    mat.enableInstancing = true;

                }
            }
        }
        EditorUtility.SetDirty(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }



    [MenuItem("GameObject/Helpers/ChangeMeshColliderToBoxCollider", false, 0)]
    public static void ChangeMeshColliderToBoxCollider()
    {
        GameObject root = Selection.activeGameObject;
        InternalChangeMeshColliderToBoxCollider(root);
    }

    private static void InternalChangeMeshColliderToBoxCollider(GameObject root)
    {

        var meshCollider = root.GetComponentsInChildren<MeshCollider>();
        for (int i = 0; i < meshCollider.Length; i++)
        {
            var mesh = meshCollider[i].sharedMesh;

            var box = meshCollider[i].GetComponent<BoxCollider>();
            if (box == null)
            {
                box = meshCollider[i].AddComponent<BoxCollider>();
            }
            box.size = mesh.bounds.size;
            box.center = mesh.bounds.center;

            Object.DestroyImmediate(meshCollider[i], true);
        }
        EditorUtility.SetDirty(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }







    [MenuItem("GameObject/Helpers/ChangeChildrenMeshsToColliders", false, 0)]
    public static void ChangeChildrenMeshsToColliders()
    {
        GameObject root = Selection.activeGameObject;
        InternalChangeMeshToCollider(root);
    }


    public static void ChangeChildrenMeshsToColliders(GameObject root)
    {
        InternalChangeMeshToCollider(root);
    }

    private static void InternalChangeMeshToCollider(GameObject root)
    {
        var allChildren = root.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < allChildren.Length; i++)
        {
            var child = allChildren[i];
            if (child.name.CaseInsensitiveContains("Collision_") == true || child.name.CaseInsensitiveContains("Collider"))
            {
                var mesh = child.GetComponent<MeshFilter>();

                if ((child.GetComponent<MeshCollider>() == null) && (mesh != null))
                {

                    var col = child.AddComponent<MeshCollider>();
                    col.sharedMesh = mesh.sharedMesh;
                    col.convex = true;
                    child.SetActive(true);

                    var meshRender = child.GetComponent<MeshRenderer>();
                    Object.DestroyImmediate(meshRender, true);
                    Object.DestroyImmediate(mesh, true);


                }
            }
        }

        EditorUtility.SetDirty(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


    [MenuItem("GameObject/Helpers/NonConvexMeshCollider_5", false, 0)]
    private static void NonConvexMeshCollider_5()
    {
        var obj = Selection.activeGameObject;
        var nonConvex = obj.AddComponent<NonConvexMeshCollider>();
        nonConvex.Calculate(5);
        Object.DestroyImmediate(nonConvex);
    }




    private static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }



    public static Bounds GetEncapsulatingBounds(GameObject obj)
    {
        Bounds totalBounds = new Bounds();
        foreach (var renderer in obj.GetComponentsInChildren<Renderer>())
        {
            var path =  renderer.gameObject.GetGameObjectPath();
            if (path.CaseInsensitiveContains("Hologram/") == false)
            {
                if (totalBounds.size.magnitude == 0f)
                {
                    totalBounds = renderer.bounds;
                }
                else
                {
                    totalBounds.Encapsulate(renderer.bounds);
                }
            }
        }
        return totalBounds;
    }


    [MenuItem("GameObject/Helpers/CreateDummyParent", false, 0)]
    public static void CreateDummyParent()
    {
        var baseItems = Selection.gameObjects;
        for (int i = 0; i < baseItems.Length; i++)
        {
            var baseItem = baseItems[i];
            int baseSibling = baseItem.transform.GetSiblingIndex();
            string newName = baseItem.name + "_DUMMY";
            if (newName != baseItem.name && newName != baseItem.transform.parent.name)
            {
                GameObject createObject = new GameObject();
                createObject.name = newName;
                var baseParent = baseItem.transform.parent;
                createObject.transform.SetParent(baseItem.transform);
                createObject.ClearLocals();
                createObject.transform.SetParent(null);
                createObject.transform.SetParent(baseParent);
                baseItem.transform.SetParent(createObject.transform);
                createObject.transform.SetSiblingIndex(baseSibling);
            }

        }
    }
}
#endif