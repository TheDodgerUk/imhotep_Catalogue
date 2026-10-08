using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityObject = UnityEngine.Object;

public class AssetSearchWindow : EditorWindow
{
    [MenuItem("Window/Custom/Asset Search")]
    public static void ShowWindow() { GetWindow<AssetSearchWindow>("Asset Search"); }

    private enum Tab { Build, Search, Results };
    private Tab m_CurrentTab = Tab.Build;

    private AssetContainer<UnityObject> m_Targets;
    private AssetContainer<UnityObject> m_Scenes;
    private AssetContainer<GameObject> m_Prefabs;

    private ReferenceContainer m_References;

    private GUIStyle m_StyleTitle;
    private EditorJob m_Job;

    private void Awake()
    {
        m_Targets = new AssetContainer<UnityObject>("*.*");
        m_Scenes = new AssetContainer<UnityObject>("*.unity");
        m_Prefabs = new AssetContainer<GameObject>("*.prefab");

        m_Prefabs.SetDefaultFolders("Artwork/Models", "Artwork/Prefabs");
        m_Scenes.SetDefaultFolders("Scenes");
    }

    private void OnSelectionChange()
    {
        Repaint();
    }

    private void Update()
    {
        if (m_Job != null && m_Job.ReadyToContinue() == true)
        {
            m_Job.DoWork();
        }
    }

    private void OnGUI()
    {
        if (m_Prefabs == null || m_Scenes == null || m_Targets == null)
        {
            Awake();
        }

        m_StyleTitle = EditorStyles.boldLabel;

        if (m_Job != null)
        {
            ShowNotification(m_Job.Title);
            return;
        }

        var tabs = Utils.Code.GetEnumValues<Tab>().Extract(t => t.ToString()).ToArray();
        m_CurrentTab = (Tab)GUILayout.SelectionGrid((int)m_CurrentTab, tabs, tabs.Length);
        if (m_CurrentTab == Tab.Results && (m_References == null || m_References.Count == 0))
        {
            m_CurrentTab = Tab.Search;
            ShowNotification(new GUIContent("No references found!"));
        }

        if (m_CurrentTab == Tab.Build)
        {
            DrawBuildTab();
        }
        else if (m_CurrentTab == Tab.Search)
        {
            DrawSearchTab();
        }
        else if (m_CurrentTab == Tab.Results)
        {
            DrawResultsTab();
        }
    }

    private void DrawBuildTab()
    {
        GUILayout.Label("Search Locations", m_StyleTitle);

        if (GUILayout.Button("Grab All Scenes and Prefabs") == true)
        {
            m_Prefabs.GrabDefault();
            m_Scenes.GrabDefault();
        }

        GUILayout.BeginHorizontal();
        {
            var width = GUILayout.Width(Screen.width * 0.5f - 8);
            GUILayout.BeginVertical(width);
            {
                m_Prefabs.Draw("Prefabs");
            }
            GUILayout.EndVertical();
            EditorGUILayout.Separator();
            GUILayout.BeginVertical(width);
            {
                m_Scenes.Draw("Scenes");
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Build Search Scene", GUILayout.Height(64)) == true)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == true)
            {
                BuildSearchScene();
            }
        }
    }

    private void DrawSearchTab()
    {
        m_Targets.Draw("Search For");

        if (GUILayout.Button("Search in Scene", GUILayout.Height(64)) == true)
        {
            if (m_Targets == null || m_Targets.Count == 0)
            {
                ShowNotification(new GUIContent("No search targets set!"));
            }
            else
            {
                SearchInScene();
                m_CurrentTab = Tab.Results;
            }
        }
    }

    private void DrawResultsTab()
    {
        if (m_References != null)
        {
            m_References.Draw("References");
        }
    }

    private void BuildSearchScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var searchRoot = new GameObject("Search");

        var prefabRoot = new GameObject("Prefabs");
        var canvasRect = prefabRoot.AddComponent<RectTransform>();
        canvasRect.SetParent(searchRoot.transform, false);

        var canvas = prefabRoot.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;

        for (int i = 0; i < m_Prefabs.Count; ++i)
        {
            string msg = "Loading " + m_Prefabs.List[i].name;
            float progress = (i / (float)(m_Prefabs.Count + m_Scenes.Count));
            if (Progress("Building Search Scene", msg, progress) == true)
            {
                m_Job.Cancel();
                return;
            }

            var clone = PrefabUtility.InstantiatePrefab(m_Prefabs.List[i]) as GameObject;
            clone.transform.SetParent(canvasRect, false);
        }

        var sceneRoot = new GameObject("Scenes");
        sceneRoot.transform.SetParent(searchRoot.transform, false);

        m_Job = new EditorJob("Building Search Scene", AddSceneToSearchScene, OnSearchSceneBuilt, m_Scenes.Count);
    }

    private void AddSceneToSearchScene(int i)
    {
        if (i > 0)
        {
            OnSceneLoaded(i - 1);
        }

        string msg = "Loading " + m_Scenes.List[i].name;
        float progress = ((i + m_Prefabs.Count) / (float)(m_Prefabs.Count + m_Scenes.Count));
        if (Progress("Building Search Scene", msg, progress) == true)
        {
            m_Job.Cancel();
            return;
        }
        string path = AssetDatabase.GetAssetOrScenePath(m_Scenes.List[i]);
        EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
        m_Job.WaitBetweenJobs = 0.001f;
    }

    private void OnSearchSceneBuilt()
    {
        if (m_Scenes.Count > 0)
        {
            OnSceneLoaded(m_Scenes.Count - 1);
        }

        EditorUtility.ClearProgressBar();
        m_Job = null;
    }

    private void OnSceneLoaded(int index)
    {
        var parent = GameObject.Find("Scenes").transform;
        string name = m_Scenes.List[index].name;
        var root = new GameObject(name).transform;

        var transforms = FindObjectsOfType<Transform>().ToList();
        transforms = transforms.FindAll(t => t == t.root);
        transforms.RemoveAll(t => t.name == "Search");
        transforms.ForEach(t => t.SetParent(root));
        root.SetParent(parent);
    }

    private void SearchInScene()
    {
        m_References = new ReferenceContainer();
        var root = GameObject.Find("Search");
        CheckForReferencesOnGameObjects(root);
        CheckForReferencesOnComponents(root);
        m_References.BuildGroups(m_Targets.List);
        EditorUtility.ClearProgressBar();
    }

    private void CheckForReferencesOnGameObjects(GameObject root)
    {
        var targets = m_Targets.List;
        int update = GetUpdateIndex(targets);
        for (int i = 0; i < targets.Count; ++i)
        {
            var go = targets[i] as GameObject;
            if (go != null)
            {
                string path = DoProgressBar(go.transform, targets, i, update);
                if (path == null)
                {
                    return; // cancelled
                }

                var found = root.transform.SearchAll(go.name);
                if (found.Count > 0)
                {
                    m_References.List.Add(new AssetReference(go, path));
                }
            }
        }
    }

    private void CheckForReferencesOnComponents(GameObject root)
    {
        var components = root.GetComponentsInChildren<Component>(true).ToList();
        int update = GetUpdateIndex(components);
        for (int i = 0; i < components.Count; ++i)
        {
            if (components[i] == null)
            {
                Debug.Log("There is a missing component at: " + i + "\n");
            }
            else
            {
                string scenePath = DoProgressBar(components[i].transform, components, i, update);
                if (scenePath == null)
                {
                    return; // cancelled
                }

                var obj = new SerializedObject(components[i]);
                var iterator = obj.GetIterator();

                while (iterator.NextVisible(true))
                {
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        foreach (var target in m_Targets.List)
                        {
                            if (CheckObjectReference(iterator, target) == true)
                            {
                                if (string.IsNullOrEmpty(scenePath) == true)
                                {
                                    scenePath = ConstructScenePath(components[i].transform);
                                }
                                m_References.List.Add(new AssetReference(target, scenePath));
                            }
                        }
                    }
                }
            }
        }
    }

    private static bool CheckObjectReference(SerializedProperty iterator, UnityObject target)
    {
        if (iterator.objectReferenceValue == target)
        {
            return true;
        }
        else if (iterator.objectReferenceValue is Material && target is Texture)
        {
            var material = iterator.objectReferenceValue as Material;
            var tex = target as Texture;

            // check if is main texture
            if (material.mainTexture == tex)
            {
                return true;
            }
            var shader = material.shader;
            if (shader != null)
            {
                // run through shader properties and try and find texture
                int propertyCount = ShaderUtil.GetPropertyCount(shader);
                for (int i = 0; i < propertyCount; ++i)
                {
                    if (ShaderUtil.GetPropertyType(shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string propertyName = ShaderUtil.GetPropertyName(shader, i);
                        if (material.GetTexture(propertyName) == tex)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private static bool CheckMaterialReference(Material material, string name, Texture tex)
    {
        return (material.HasProperty(name) && material.GetTexture(name) == tex);
    }

    private static string DoProgressBar(Transform transform, IList list, int index, int updateIndex)
    {
        string path = string.Empty;
        if ((index % updateIndex) == 0)
        {
            path = ConstructScenePath(transform);
            string msg = string.Concat("Loading for references in ", path);
            if (Progress("Searching Scene...", msg, index, list) == true)
            {
                return null;
            }
        }
        return path;
    }

    private static string ConstructScenePath(Transform transform)
    {
        if (transform.parent != null)
        {
            return string.Concat(ConstructScenePath(transform.parent), "/", transform.name);
        }
        else
        {
            return transform.name;
        }
    }

    public static int GetUpdateIndex(System.Collections.ICollection list)
    {
        return (int)(list.Count / 100f).Max(1);
    }

    public static bool Progress(string title, string msg, int index, System.Collections.ICollection list)
    {
        int update = GetUpdateIndex(list);
        if ((index % update) == 0)
        {
            return Progress(title, msg, index / (float)list.Count);
        }
        return false;
    }

    public static bool Progress(string title, string msg, float progress)
    {
        string percent = (progress * 100).ToString("N0");
        title = string.Concat(title, " (", percent, "%)");
        bool cancel = EditorUtility.DisplayCancelableProgressBar(title, msg, progress);
        if (cancel == true)
        {
            EditorUtility.ClearProgressBar();
        }
        return cancel;
    }
}
