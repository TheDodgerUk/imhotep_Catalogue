using UnityEditor;
using UnityEngine;

public class GridPlace : EditorWindow
{
    [MenuItem("Window/Utility/Grid Place Objects")]
    private static void ShowWindow() => GetWindow<GridPlace>(true, "Grid Place Object");

    GameObject objectToPlace;
    int Width;
    int Height;
    float ObjectWidth;
    float ObjectHeight;

    private void OnGUI()
    {
        Width = EditorGUILayout.IntField("Number X", Width);
        Height = EditorGUILayout.IntField("Number Y", Height);
        ObjectWidth = EditorGUILayout.FloatField("Object Width", ObjectWidth);
        ObjectHeight = EditorGUILayout.FloatField("Object Height", ObjectHeight);
        objectToPlace = EditorHelper.ObjectField("Object to Place", objectToPlace);

        if (null != objectToPlace && null != Selection.activeTransform)
        {
            if (true == GUILayout.Button("Process"))
            {
                ProcessObject(Selection.activeTransform);
            }
        }
    }

    private void ProcessObject(Transform selection)
    {
        //Assume centre it current transform
        Vector3 startingPostion = selection.position;
        float halfObjectWidth = ObjectWidth * 0.5f;
        float halfObjectHeight = ObjectHeight * 0.5f;
        startingPostion.x -= ((float)Width * 0.5f) * ObjectWidth - halfObjectWidth;
        startingPostion.y -= ((float)Height * 0.5f) * ObjectHeight - halfObjectHeight;

        int iCount = 0;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                //Spawn object and parent
                GameObject screen = GameObject.Instantiate(objectToPlace, selection);
                screen.name = iCount.ToString();
                Vector3 offset = new Vector3(x * ObjectWidth, y * ObjectHeight, 0f);
                screen.transform.position = startingPostion + offset;
                screen.transform.localRotation = Quaternion.identity;
                iCount++;
            }
        }
    }
}
