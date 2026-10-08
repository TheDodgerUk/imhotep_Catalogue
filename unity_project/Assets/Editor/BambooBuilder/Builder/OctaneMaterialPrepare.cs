using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class OctaneMaterialPrepare : EditorWindow
{
    [MenuItem("Window/Octane/Prepare Environment", false, 1)]
    private static void ShowEnvironmentEditor() => GetWindow<OctaneMaterialPrepare>("OctanePrepare");

    private void OnGUI()
    {
        GUILayout.Label("Environment Editor", EditorStyles.boldLabel);


        if(GUILayout.Button("Change To Specular"))
        {
            GameObject[] objs = EditorSceneManager.GetActiveScene().GetRootGameObjects();

            for (int i = 0; i < objs.Length; i++)
            {
                Renderer[] renderers = objs[i].GetComponentsInChildren<Renderer>();
                for(int x =0; x < renderers.Length; x++)
                {

                }                        
            }
        }
    }

    private void ProcessAllShaders(Shader shader)
    {

    }
}
