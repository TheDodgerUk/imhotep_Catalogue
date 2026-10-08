using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class ToolsMenu
{
    [MenuItem("Tools/Find All Non One Scaled")]
    public static void NonOneScaled()
    {
        var transforms = GameObject.FindObjectsOfType<Transform>().ToList();
        var nonOne = transforms.FindAll(t =>
        {
            var s = t.localScale;
            return s.sqrMagnitude != 3 || s.x < 0 || s.y < 0 || s.z < 0;
        });
        Debug.Log(nonOne.Stringify(t => t.GetScenePath(), "\n"));
    }

    [MenuItem("Tools/Clean Empty Folders", false, 200)]
    public static void CleanEmptyFolders()
    {
        FolderCleaner.CleanEmptyFolderInProject();
    }

    [MenuItem("Tools/Clear Progress Bar", false, 202)]
    public static void ClearProgressBar()
    {
        ProgressBar.Clear();
    }

    [MenuItem("Tools/Clear Player Prefs", false, 204)]
    public static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    [MenuItem("Tools/Bamboo Builder/Build Lighting", false, 107)]
    public static void BuildLighting()
    {
        BuildSystem.BambooBuilder.BuildLighting();
    }

    [MenuItem("Tools/Bamboo Builder/Build Asset Bundles", false, 108)]
    public static void BuildAssetBundles()
    {
        BuildSystem.BambooBuilder.BuildAssetBundles();
    }

    [MenuItem("Tools/Apply Selected Prefabs", false, 206)]
    public static void ApplySelectedPrefabs()
    {
        using (var progressBar = new ProgressBar("Applying Prefabs", Selection.gameObjects.Length))
        {
            foreach (var selection in Selection.gameObjects)
            {
                progressBar.UpdateIncrement(selection.name);

                var root = PrefabUtility.FindPrefabRoot(selection);

                var originalHideFlags = root.hideFlags;
                if (originalHideFlags != HideFlags.None)
                {
                    Debug.LogWarning($"Had to reset hideFlags on \"{root.name}\". Was to set to [{originalHideFlags}]\n", root);
                    root.hideFlags = HideFlags.None;
                }
                var prefab = PrefabUtility.GetCorrespondingObjectFromSource(root);
                if (prefab != null)
                {
                    var options = ReplacePrefabOptions.ConnectToPrefab;
                    PrefabUtility.ReplacePrefab(root, prefab, options);
                }
                root.hideFlags = originalHideFlags;
            }
        }
    }

    [MenuItem("Tools/Switch Child with Parent", false, 208)]
    public static void MakeChildParent()
    {
        var transforms = Selection.transforms;
        using (var progressBar = new ProgressBar("Switching Children", transforms.Length))
        {
            foreach (var child in transforms)
            {
                string undoName = child.name;
                var parent = child.parent;
                if (parent != null)
                {
                    int siblingIndex = parent.GetSiblingIndex();
                    var grandChildren = child.GetDirectChildren();
                    var siblings = parent.GetDirectChildren();
                    siblings.Remove(child);

                    var grandParent = parent.parent;
                    Undo.SetTransformParent(child, grandParent, undoName);
                    Undo.SetTransformParent(parent, child, undoName);
                    child.SetSiblingIndex(siblingIndex);

                    foreach (var sibling in siblings)
                    {
                        Undo.SetTransformParent(sibling, child, undoName);
                    }
                    foreach (var grandChild in grandChildren)
                    {
                        Undo.SetTransformParent(grandChild, parent, undoName);
                    }

                    string parentName = parent.name;
                    Undo.RegisterCompleteObjectUndo(parent, undoName);
                    parent.name = child.name;
                    Undo.RegisterCompleteObjectUndo(child, undoName);
                    child.name = parentName;
                }
                progressBar.UpdateIncrement(child.name);
            }
        }
    }

    [MenuItem("CONTEXT/Component/Move to GameObject", false, 0)]
    public static void MoveComponentToGameObject(MenuCommand command)
    {
        var component = command.context as Component;
        if (component != null)
        {
            if (ComponentUtility.CopyComponent(component) == true)
            {
                var go = new GameObject($"{component.name}_{component.GetType().Name}");
                Undo.RegisterCreatedObjectUndo(go, $"Undo Create {go.name}");
                go.transform.SnapToParent(component.transform);
                ComponentUtility.PasteComponentAsNew(go);
                Undo.DestroyObjectImmediate(component);
            }
            else
            {
                Debug.LogError($"Failed to copy component {component.name}\n", component);
            }
        }
        else
        {
            Debug.LogError($"This is not a Component!\n", command.context);
        }
    }

    [MenuItem("CONTEXT/Transform/Reset Parent Position", false, 50)]
    public static void Transform_ResetParentPosition(MenuCommand command)
    {
        ResetOrientation(command, "Reset Parent Position", t => t.localPosition = Vector3.zero);
    }

    [MenuItem("CONTEXT/Transform/Reset Parent Rotation", false, 51)]
    public static void Transform_ResetParentRotation(MenuCommand command)
    {
        ResetOrientation(command, "Reset Parent Rotation", t => t.localRotation = Quaternion.identity);
    }

    [MenuItem("CONTEXT/Transform/Reset Parent Scale", false, 52)]
    public static void Transform_ResetParentScale(MenuCommand command)
    {
        ResetOrientation(command, "Reset Parent Scale", t => t.localScale = Vector3.zero);
    }

    [MenuItem("CONTEXT/Transform/Reset Parent Orientation", false, 53)]
    public static void Transform_ResetParentOrientation(MenuCommand command)
    {
        ResetOrientation(command, "Reset Parent Orientation", t => t.Reset(true));
    }

    private static void ResetOrientation(MenuCommand command, string undoName, System.Action<Transform> resetHandler)
    {
        var transform = command.context as Transform;
        var children = transform.GetDirectChildren();
        foreach (var child in children)
        {
            Undo.SetTransformParent(child, null, undoName);
        }

        Undo.RecordObject(transform, undoName);
        resetHandler(transform);

        foreach (var child in children)
        {
            Undo.SetTransformParent(child, transform, undoName);
        }
    }

    [MenuItem("CONTEXT/BoxCollider/Centre Transform to Collider", false, 0)]
    public static void BoxCollider_CentreTransformToCollider(MenuCommand command)
    {
        RectUtility.CentreTransformToCollider(command.context as BoxCollider);
    }

    [MenuItem("CONTEXT/BoxCollider/Resize To Rect", false, 0)]
    public static void BoxCollider_ResizeToRect(MenuCommand command)
    {
        RectUtility.AlignColliderToRect(command.context as BoxCollider);
    }

    [MenuItem("CONTEXT/BoxCollider/Resize To Rect", true, 0)]
    public static bool BoxCollider_ResizeToRect_Validation(MenuCommand command)
    {
        var boxCollider = command.context as BoxCollider;
        return boxCollider.GetRect() != null;
    }

    [MenuItem("CONTEXT/Transform/Resize To Rect", false, 0)]
    public static void Transform_ResizeToRect(MenuCommand command)
    {
        RectUtility.AlignTransformToRect(command.context as Transform);
    }

    [MenuItem("CONTEXT/Transform/Resize To Rect", true, 0)]
    public static bool Transform_ResizeToRect_Validation(MenuCommand command)
    {
        var transform = command.context as Transform;
        return transform.parent is RectTransform;
    }


    [MenuItem("Tools/Add lightType to Light")]
    public static void RenameLights()
    {
#if UNITY_EDITOR
        var transforms = GameObject.FindObjectsOfType<Light>().ToList();

        int count = 0;
        for(int i = 0; i < transforms.Count; i++)
        {
            if (transforms[i].lightmapBakeType == LightmapBakeType.Baked)
            {
                
                if (transforms[i].name.Contains("Baked") == false)
                {
                    if (transforms[i].name.Contains("Realtime") == true)
                    {
                        transforms[i].name = transforms[i].name.Replace("Realtime", "Baked");
                        Debug.Log($"Changed Realtime to Baked", transforms[i].gameObject);
                    }
                    else
                    {
                        transforms[i].name += "_Baked";
                        Debug.Log($"Changed Added _Baked", transforms[i].gameObject);
                    }
                    count++;
                }
            }


            if (transforms[i].lightmapBakeType == LightmapBakeType.Realtime)
            {
                if (transforms[i].name.Contains("Realtime") == false)
                {
                    if (transforms[i].name.Contains("Baked") == true)
                    {
                        transforms[i].name = transforms[i].name.Replace("Baked", "Realtime");
                        Debug.Log($"Changed Baked to Realtime", transforms[i].gameObject);
                    }
                    else
                    {
                        transforms[i].name += "_Realtime";
                        Debug.Log($"Changed Added _Realtime", transforms[i].gameObject);
                    }
                    count++;
                }
            }
        }
        Debug.LogError($"Changed number : {count}");

#endif
    }
}
