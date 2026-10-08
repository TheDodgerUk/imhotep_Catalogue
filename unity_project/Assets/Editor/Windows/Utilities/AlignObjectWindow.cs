using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AlignmentAxis
{
    public static readonly List<AlignmentAxis> Axes = new List<AlignmentAxis>
        {
            new AlignmentAxis("+x", (t, d) => t.right = d),
            new AlignmentAxis("+y", (t, d) => t.up = d),
            new AlignmentAxis("+z", (t, d) => t.forward = d),
            new AlignmentAxis("-x", (t, d) => t.right = -d),
            new AlignmentAxis("-y", (t, d) => t.up = -d),
            new AlignmentAxis("-z", (t, d) => t.forward = -d),
        };

    public delegate void DirectionHandler(Transform t, Vector3 direction);

    public readonly string Display;
    public readonly DirectionHandler Handler;
    public AlignmentAxis(string display, DirectionHandler handler)
    {
        Display = display;
        Handler = handler;
    }
}

public class AlignObjectWindow : EditorWindow
{
    [MenuItem("Window/Utility/Align Objects")]
    public static void ShowWindow() { GetWindow<AlignObjectWindow>("Align"); }

    private AlignmentAxis m_Axis = AlignmentAxis.Axes.First();

    private void OnEnable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGui;
        SceneView.onSceneGUIDelegate += OnSceneGui;
    }

    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGui;
    }

    private void OnGUI()
    {
        DrawAxisAlignmentSelection();

        var selection = Selection.gameObjects;
        if (selection.Length != 2)
        {
            EditorGUILayout.HelpBox("Must have 2 objects to align", MessageType.Error, true);
            return;
        }

        // rotation objects to align to axis
        var a = selection[0].transform;
        var b = selection[1].transform;

        var delta = b.position - a.position;
        var direction = delta.normalized;

        m_Axis.Handler(a, direction);
        m_Axis.Handler(b, direction);

        // draw buttons to spawn clone at mid point
        foreach (var go in selection)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorHelper.ObjectField(go);

                if (GUILayout.Button("Clone at Centre Point") == true)
                {
                    var clone = Utils.Unity.Clone(go, go.transform.parent);
                    Undo.RegisterCreatedObjectUndo(clone, clone.name);
                    clone.transform.position = (b.position + a.position) * 0.5f;
                }
            }
        }

        EditorGUILayout.LabelField("Distance", delta.magnitude.ToString("N3"));

        SceneView.RepaintAll();
    }

    private void DrawAxisAlignmentSelection()
    {
        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope(GUILayout.Width(EditorGUIUtility.labelWidth)))
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                GUILayout.Label("Axis");
            }

            m_Axis = EditorHelper.SelectionGrid(3, m_Axis, AlignmentAxis.Axes, axis => axis.Display);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    private GameObject[] GetSelection()
    {
        var selection = Selection.gameObjects;
        if (selection.Length != 2)
        {
            foreach (var go in selection)
            {
                EditorHelper.ObjectField(go);
            }
            ShowNotification(new GUIContent("Must have 2 objects to align"));
            return null;
        }
        return selection;
    }

    private void OnSceneGui(SceneView sceneView)
    {
        var selection = Selection.gameObjects;
        if (selection.Length != 2)
        {
            return;
        }

        var a = selection[0].transform;
        var b = selection[1].transform;

        Handles.color = Color.red;
        Handles.DrawLine(a.position, b.position);
    }
}
