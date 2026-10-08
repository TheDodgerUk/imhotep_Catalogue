using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StaticFlagEditor : EditorWindow
{
    [MenuItem("Window/Utility/Static Flag Editor")]
    public static void ShowWindow() { GetWindow<StaticFlagEditor>("Static Flags"); }

    private static readonly List<StaticEditorFlags> StaticFlags = new List<StaticEditorFlags>
    {
        StaticEditorFlags.BatchingStatic, StaticEditorFlags.ReflectionProbeStatic,
        StaticEditorFlags.OccludeeStatic, StaticEditorFlags.OccluderStatic,
    };
    private static readonly List<StaticEditorFlags> NavigationFlags = new List<StaticEditorFlags>
    {
        StaticEditorFlags.NavigationStatic, StaticEditorFlags.OffMeshLinkGeneration,
    };
    private static readonly List<StaticEditorFlags> LightingFlags = new List<StaticEditorFlags>
    {
        StaticEditorFlags.ContributeGI
    };

    private Vector2 m_ScrollPos = Vector2.zero;
    private bool m_Foldout = false;

    private void OnEnable()
    {
        autoRepaintOnSceneChange = true;
    }

    private void OnGUI()
    {
        var gameObjects = Selection.gameObjects.ToList();
        if (gameObjects.Count == 0)
        {
            ShowNotification(new GUIContent("No GameObjects Selected!"));
            return;
        }

        EditorGUILayout.Space();

        DrawFlags(gameObjects);

        using (var s = new EditorGUILayout.ScrollViewScope(m_ScrollPos))
        {
            m_Foldout = EditorGUILayout.Foldout(m_Foldout, $"{gameObjects.Count} selected", true);
            if (m_Foldout == true)
            {
                foreach (var go in gameObjects)
                {
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        EditorHelper.ObjectField(go);
                        DrawFlags(go.CreateList());
                    }
                }
            }
            m_ScrollPos = s.scrollPosition;
        }
    }

    private static void DrawFlags(List<GameObject> gameObjects)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            DrawFlagGroup(gameObjects, "Static", StaticFlags);
            DrawFlagGroup(gameObjects, "Navigation", NavigationFlags);
            DrawFlagGroup(gameObjects, "Lighting", LightingFlags);
        }
    }

    private static void DrawFlagGroup(List<GameObject> gameObjects, string name, List<StaticEditorFlags> flags)
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(Screen.width / 3f)))
        {
            // get the flagmask of the first selected gameobject
            var mainGo = gameObjects.First();
            var flagMask = GameObjectUtility.GetStaticEditorFlags(mainGo);

            // does the flagmask have the all the flags for this group set?
            bool groupState = flags.TrueForAll(f => flagMask.HasFlag(f));
            bool newGroupState = GUILayout.Toggle(groupState, name, "Button");
            if (groupState != newGroupState)
            {
                // if the group state has changed, toggle all the flags in this group to the new state
                bool children = ShouldApplyToChildren();
                foreach (var flag in flags)
                {
                    ToggleFlag(gameObjects, flag, newGroupState, children);
                }
            }

            EditorGUILayout.Space();

            // draw each flag state
            foreach (var flag in flags)
            {
                bool state = flagMask.HasFlag(flag);
                bool newState = GUILayout.Toggle(state, flag.ToString(), "Button");
                if (state != newState)
                {
                    // if the flag state has changed toggle it
                    ToggleFlag(gameObjects, flag, newState);
                }
            }
        }
    }

    private static void ToggleFlag(List<GameObject> gameObjects, StaticEditorFlags flag, bool state, bool? children = null)
    {
        if (children == null)
        {
            children = ShouldApplyToChildren();
        }

        foreach (var go in gameObjects)
        {
            ToggleFlag(go, flag, state);

            if (children == true)
            {
                foreach (var child in go.transform.GetAllChildren())
                {
                    ToggleFlag(child.gameObject, flag, state);
                }
            }
        }
    }

    private static bool ShouldApplyToChildren()
    {
        return EditorUtility.DisplayDialog("Apply to Children?", "Would you like to also apply the Flag Mask to children?", "Yes, Change Children", "No");
    }

    public static void ToggleFlag(GameObject go, StaticEditorFlags flag, bool state)
    {
        Undo.RecordObject(go, "Set Editor Flags");
        var mask = GameObjectUtility.GetStaticEditorFlags(go);
        Utils.BitMask.Toggle(ref mask, flag, state);
        GameObjectUtility.SetStaticEditorFlags(go, mask);
    }

    private static StaticEditorFlags ToggleFlag(StaticEditorFlags mask, StaticEditorFlags flag, bool state)
    {
        if (state == true)
        {
            return mask | flag;
        }
        else
        {
            return mask & ~flag;
        }
    }

    public void OnSelectionChange() => Repaint();
}
