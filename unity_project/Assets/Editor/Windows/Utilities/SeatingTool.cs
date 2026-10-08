using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SeatingTool : EditorWindow
{
    [MenuItem("Window/Utility/Seating Tool")]
    private static void ShowWindow() => GetWindow<SeatingTool>().ShowUtility();

    private Transform m_MainSeat;
    private string m_PrefixName;
    private string m_Number;

    private void OnGUI()
    {
        m_PrefixName = EditorGUILayout.TextField("Block Name", m_PrefixName);
        m_Number = EditorGUILayout.TextField("Block Number", m_Number);

        m_MainSeat = EditorHelper.ObjectField("Main Seat", m_MainSeat);

        if (GUILayout.Button("Create Block from Selected") == true)
        {
            var block = new GameObject(m_PrefixName + m_Number);
            var seats = Selection.transforms.ToList();
            seats.Sort(Comparisons.AlphaNumericByName);

            var first = seats.First();
            if (m_MainSeat != null)
            {
                first = m_MainSeat;
            }

            block.transform.position = first.position;

            var euler = first.eulerAngles;
            euler.x = 0;
            euler.z = 0;
            block.transform.eulerAngles = euler;

            seats.ForEach(s => s.SetParent(block.transform, true));

            m_MainSeat = null;
        }

        if (GUILayout.Button("Select Parents of Selection") == true)
        {
            var selection = Selection.gameObjects;
            for (int i = 0; i < selection.Length; ++i)
            {
                selection[i] = selection[i].transform.parent?.gameObject;
            }
            Selection.objects = selection;
        }

        if (GUILayout.Button("Find Empty Parents") == true)
        {
            var empty = new List<GameObject>();
            foreach (Transform child in Selection.activeTransform)
            {
                if (child.childCount == 0)
                {
                    empty.Add(child.gameObject);
                }
            }
            Selection.objects = empty.ToArray();
        }

        if (GUILayout.Button("Reset Scale") == true)
        {
            foreach (Transform child in Selection.activeTransform)
            {
                Undo.RecordObject(child, "Reset Scale");
                child.localScale = Vector3.one;
                child.Rotate(180, 0, 0, Space.Self);
            }
        }

        if (GUILayout.Button("Split Root Objects into Scenes") == true)
        {
            var active = SceneManager.GetActiveScene();
            string folder = active.path.Strip($"{active.name}.unity");


            foreach (var go in active.GetRootGameObjects())
            {
                string name = $"Seating_{go.name}";
                var scene = SceneManager.GetSceneByName(name);
                if (scene.IsValid() == false)
                {
                    scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                }

                Undo.RecordObject(go, "Moved Scene");
                SceneManager.MoveGameObjectToScene(go, scene);

                EditorSceneManager.SaveScene(scene, $"{folder}{name}.unity");
            }
        }
    }
}