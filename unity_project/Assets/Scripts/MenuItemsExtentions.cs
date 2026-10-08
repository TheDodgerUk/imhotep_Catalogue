#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class MenuItemsExtentions
{
    [MenuItem("GameObject/Helpers/StagingPoint/FromRenderer", false, 0)]
    private static void FromRenderer()
    {
        var selected = Selection.activeGameObject;
        Bounds totalBounds = new Bounds();
        foreach (var renderer in selected.GetComponentsInChildren<Renderer>())
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

        GameObject temp = new GameObject("BOUNDING BOX TEST");
        temp.transform.SetParent(selected.transform);
        temp.transform.localPosition = Vector3.zero;
        BoxCollider b = temp.AddComponent<BoxCollider>();
        b.center = totalBounds.center;
        b.size = totalBounds.size;

        GeneratePoints(selected, b);
        UnityEngine.Object.DestroyImmediate(temp);
    }


    [MenuItem("GameObject/Helpers/StagingPoint/FromBoxCollider", false, 0)]
    private static void FromBoxCollider()
    {
        var selected = Selection.activeGameObject;
        BoxCollider b = selected.GetComponent<BoxCollider>();
        GeneratePoints(selected, b);
    }



    [MenuItem("GameObject/Helpers/CenterOfMass", false, 0)]
    private static void CenterOfMass()
    {
        var selected = Selection.activeGameObject;
        if (selected.name == GlobalConsts.RootModel)
        {
            GameObject center = new GameObject(GlobalConsts.CenterOfMass);
            center.transform.SetParent(selected.transform);
        }
        else
        {
            EditorUtility.DisplayDialog("CenterOfMass Error", $"CenterOfMass can only be done as a Child of {GlobalConsts.RootModel}", "OK");
        }
    }


    private static void GeneratePoints(GameObject selected, BoxCollider b)
    {
        Vector3[] vertices = new Vector3[6];
        vertices[0] = selected.transform.TransformPoint(b.center + new Vector3(0, b.size.y, 0) * 0.5f);
        vertices[1] = selected.transform.TransformPoint(b.center + new Vector3(0, -b.size.y, 0) * 0.5f);
        vertices[2] = selected.transform.TransformPoint(b.center + new Vector3(b.size.x, 0, 0) * 0.5f);
        vertices[3] = selected.transform.TransformPoint(b.center + new Vector3(-b.size.x, 0, 0) * 0.5f);
        vertices[4] = selected.transform.TransformPoint(b.center + new Vector3(0, 0, b.size.z) * 0.5f);
        vertices[5] = selected.transform.TransformPoint(b.center + new Vector3(0, 0, -b.size.z) * 0.5f);


        GameObject[] gameObjects = new GameObject[6];
        for (int i = 0; i < vertices.Length; i++)
        {
            GameObject newObj = new GameObject($"Staging Generated {i}");
            newObj.transform.SetParent(selected.transform);
            newObj.transform.position = vertices[i];
            gameObjects[i] = newObj;
        }

        gameObjects[0].transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
        gameObjects[1].transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        gameObjects[2].transform.localRotation = Quaternion.Euler(0f, 90f, 0);
        gameObjects[3].transform.localRotation = Quaternion.Euler(0f, -90f, 0);

        gameObjects[4].transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
        gameObjects[5].transform.localRotation = Quaternion.Euler(0f, 180f, 0f);


        gameObjects[0].name = "Staging Point Generated UP";
        gameObjects[1].name = "Staging Point Generated DOWN";

        gameObjects[2].name = "Staging Point Generated FORWARD";
        gameObjects[3].name = "Staging Point Generated BACKWARD";

        gameObjects[4].name = "Staging Point Generated LEFT";
        gameObjects[5].name = "Staging Point Generated RIGHT";
    }

    [MenuItem("GameObject/ConvertURPToStandard", false, 0)]
    private static void ConvertAllToStandard()
    {
        Debug.Log("*** FINDING ASSETS BY TYPE ***");

        string STANDARD = "Standard";
        string URP = "Universal Render Pipeline/Lit";
        string URP_UNLIT = "Universal Render Pipeline/Unlit";

        var standard = Shader.Find(STANDARD);
        var lit = Shader.Find(URP);
        var unLit = Shader.Find(URP_UNLIT);

        if (standard == null)
        {
            Debug.LogError("standard is null");
            return;
        }

        if (lit == null)
        {
            Debug.LogError("lit is null");
            return;
        }

        if (unLit == null)
        {
            Debug.LogError("unLit is null");
            return;
        }

        string[] guids;

        guids = AssetDatabase.FindAssets("t:material");
        int count = 0;
        int index = 0;

#if CATALOG_PROGRAM
        string FileName = "D:/FilesChecked_CATALOG";
#else
        string FileName = "D:/FilesChecked_MAIN";
#endif

        List<string> list = Json.FullSerialiser.ReadFromFile<List<string>>(FileName);
        if (list == null)
        {
            list = new List<string>();

        }
        Debug.LogError($"guids {guids.Length}");
        ///return;
        foreach (string guid in guids)
        {
            if (list.Contains(guid) == false)
            {
                Debug.Log("Material: " + AssetDatabase.GUIDToAssetPath(guid));
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Material asset = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (asset.shader.name == URP || asset.shader.name == URP_UNLIT)
                {
                    asset.shader = standard;
                    Debug.LogError("asset: " + asset.name);
                    EditorUtility.SetDirty(asset);
                    count++;
                }
                list.Add(guid);
                index++;
                if (count > 100)
                {
                    AssetDatabase.SaveAssets();
                    break;
                }
            }
        }


        Json.FullSerialiser.WriteToFile(list, FileName, true);
        Debug.LogError($"number of URP changed :{count}  , length {guids.Length}   got to {guids.Length - index}");
    }


}
#endif