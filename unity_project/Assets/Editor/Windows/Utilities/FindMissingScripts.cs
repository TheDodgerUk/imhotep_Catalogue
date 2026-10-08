using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Window/Utility/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<FindMissingScripts>("Find Missing Scripts");
    }

    public void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in selected GameObjects"))
        {
            FindInSelected();
        }
        if (GUILayout.Button("Select GameObjects with Missing Scripts"))
        {
            var missing = FindInSelected();
            Selection.objects = missing.ToArray();
        }
        if (GUILayout.Button("Find Missing Scripts in all scenes"))
        {
            FindInScenes();
        }
    }

    private static List<GameObject> FindInSelected()
    {
        int searchedGos = 0, searchedComponents = 0;
        var missing = new List<GameObject>();
        var objs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);

        foreach (GameObject go in objs)
        {
            FindInGameObject(go, missing, ref searchedGos, ref searchedComponents);
        }
        Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", searchedGos, searchedComponents, missing.Count));
        return missing;
    }


    private static void FindInGameObject(GameObject go, List<GameObject> missing, ref int searchedGos, ref int searchedComponents)
    {
        searchedGos++;
        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            searchedComponents++;
            if (components[i] == null)
            {
                if (missing.Contains(go) == false)
                {
                    missing.Add(go);
                }
                Debug.Log(go.name + " has an empty script attached in position: " + i, go);
            }
        }

        // go through each child (if any)
        foreach (Transform child in go.transform)
        {
            FindInGameObject(child.gameObject, missing, ref searchedGos, ref searchedComponents);
        }
    }

    private static List<GameObject> FindInScenes()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
        {
            int searchedGos = 0, searchedComponents = 0;
            var missing = new List<GameObject>();
            string[] scenes = GetScenePaths();

            var bar = new ProgressBar("Searching scene...", scenes.Length);
            for (int i = 0; i < scenes.Length; ++i)
            {
                bar.UpdateIncrement($"Searching {scenes[i]}");

                EditorSceneManager.OpenScene(scenes[i], OpenSceneMode.Single);

                var gameObjects = FindObjectsOfType<GameObject>();
                foreach (var go in gameObjects)
                {
                    FindInGameObject(go, missing, ref searchedGos, ref searchedComponents);
                }
            }
            EditorUtility.ClearProgressBar();
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} missing", searchedGos, searchedComponents, missing.Count));

            return missing;
        }

        return new List<GameObject>();
    }

    private static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        return scenes;
    }
}
