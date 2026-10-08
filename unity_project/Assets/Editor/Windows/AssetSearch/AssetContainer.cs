using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AssetContainer<T> : ListContainer<T> where T : Object
{
    private List<string> m_DefaultFolders;
    private string m_Filter;

    public AssetContainer(string filter) : base()
    {
        m_DefaultFolders = new List<string>();
        m_Filter = filter;
    }

    public void SetDefaultFolders(params string[] folders)
    {
        m_DefaultFolders = folders.ToList();
    }

    public override void Draw(string title)
    {
        base.Draw(title);

        DrawControls(title);
        DrawList(title);
    }

    private void DrawControls(string title)
    {
        var maxWidth = GUILayout.MaxWidth(100);
        for (int i = 0; i < m_DefaultFolders.Count; ++i)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(m_DefaultFolders[i] + "/...", m_TitleStyle);
                string path = GetPath(m_DefaultFolders[i]);
                if (GUILayout.Button("Grab", maxWidth) == true)
                {
                    GrabAtPath(path);
                }
                if (List != null)
                {
                    if (GUILayout.Button("Add", maxWidth) == true)
                    {
                        AddAtPath(path);
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button("Grab", maxWidth) == true)
            {
                GrabSelection();
            }
            if (List != null)
            {
                if (GUILayout.Button("Add", maxWidth) == true)
                {
                    AddToList();
                }

                if (GUILayout.Button("Clear", maxWidth) == true)
                {
                    List.Clear();
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    private void DrawList(string title)
    {
        Foldout = EditorGUILayout.Foldout(Foldout, title, true);
        if (Foldout == true && List.Count > 0)
        {
            ScrollPos = GUILayout.BeginScrollView(ScrollPos);
            {
                EditorGUI.indentLevel++;
                {
                    for (int i = 0; i < List.Count; ++i)
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.BeginVertical();
                            {
                                GUILayout.Space(5);
                                EditorHelper.ObjectField(List[i].GetType().Name, List[i]);
                            }
                            EditorGUILayout.EndVertical();

                            if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20)) == true)
                            {
                                List.RemoveAt(i);
                                --i;
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUI.indentLevel--;
            }
            GUILayout.EndScrollView();
        }
    }

    public void GrabDefault()
    {
        List.Clear();
        for (int i = 0; i < m_DefaultFolders.Count; ++i)
        {
            AddAtPath(GetPath(m_DefaultFolders[i]));
        }
    }

    public void GrabAtPath(string path)
    {
        List = Load(GetFilesAtPath(path));
        Foldout = (List.Count > 0);
    }

    public void AddAtPath(string path)
    {
        List.AddRange(Load(GetFilesAtPath(path)));
        List = List.Distinct().ToList();
        Foldout = (List.Count > 0);
    }

    private void GrabSelection()
    {
        List = Load(GetFilesSelected());
        Foldout = (List.Count > 0);
    }

    private void AddToList()
    {
        List.AddRange(Load(GetFilesSelected()));
        List = List.Distinct().ToList();
        Foldout = (List.Count > 0);
    }

    private static List<T> Load(List<string> paths)
    {
        paths.RemoveAll(p => p.Contains(".meta"));
        paths = paths.Extract(p => p.Replace(Application.dataPath, "Assets"));
        string title = "Loading " + typeof(T).Name;

        var objs = new List<T>();
        for (int i = 0; i < paths.Count; ++i)
        {
            if (AssetSearchWindow.Progress(title, paths[i], i, paths) == true)
            {
                break;
            }

            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(paths[i]).ToList();
            if (paths[i].Contains(".prefab") == true)
            {
                assets.Clear();
            }

            var asset = AssetDatabase.LoadAssetAtPath<T>(paths[i]);
            if (assets.Contains(asset) == false)
            {
                assets.Add(asset);
            }
            assets.ForEach(a => objs.Add(a as T));
        }
        EditorUtility.ClearProgressBar();
        return objs;
    }

    private List<string> GetFilesSelected()
    {
        var selection = Selection.objects.ToList();
        var paths = new List<string>();
        foreach (var obj in selection)
        {
            var path = AssetDatabase.GetAssetPath(obj);
            if (Directory.Exists(path) == true)
            {
                paths.AddRange(GetFilesAtPath(path));
            }
            paths.Add(path);
        }
        return GetFiles(paths);
    }

    private List<string> GetFilesAtPath(string path)
    {
        return GetFiles(Directory.GetFiles(path, m_Filter, SearchOption.AllDirectories).ToList());
    }

    private List<string> GetFiles(List<string> paths)
    {
        string filter = m_Filter.Strip("*");
        paths = paths.Distinct().ToList();
        paths.RemoveAll(p => p == null);
        paths.RemoveAll(p => p.Contains(filter) == false);
        return paths.Extract(p => p.Replace("\\", "/"));
    }

    private static string GetPath(string path)
    {
        return string.Concat(Application.dataPath, "/", path);
    }
}
