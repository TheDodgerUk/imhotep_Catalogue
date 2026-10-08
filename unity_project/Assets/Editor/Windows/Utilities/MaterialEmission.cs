using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MaterialEmission : EditorWindow
{
    [MenuItem("Window/Utility/Material Emission")]
    public static void ShowWindow() => GetWindow<MaterialEmission>("Material Emission");

    private const string EmissionColour = "_EmissionColor";
    private const string EmissionMap = "_EmissionMap";
    private const string EmissionFlag = "_EMISSION";
    private const string ShaderName = "Standard";
    private const string Filter = "*.mat";

    private List<Material> m_Materials;
    private Vector2 m_ScrollPos;

    private void OnGUI()
    {
        if (GUILayout.Button("Grab Selected") == true)
        {
            m_Materials = new List<Material>();
            var paths = GetFilesSelected();

            using (var progress = new ProgressBar("Loading Materials", paths))
            {
                foreach (var path in paths)
                {
                    progress.UpdateIncrement($"Loading material at path: {path}");

                    var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                    if (mat.shader.name == ShaderName)
                    {
                        if (mat.HasProperty(EmissionMap) == true)
                        {
                            mat.GetTexture(EmissionMap);
                        }
                        m_Materials.Add(mat);
                    }
                }
            }
        }

        if (m_Materials != null)
        {
            EditorGUILayout.FloatField("Count", m_Materials.Count);
            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            {
                foreach (var material in m_Materials)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorHelper.ObjectField(material);

                        GUILayout.Space(8);
                        EditorGUILayout.Toggle(material.IsKeywordEnabled(EmissionFlag), GUILayout.Width(26));

                        if (material.HasProperty(EmissionColour) == true)
                        {
                            EditorGUILayout.ColorField(material.GetColor(EmissionColour), GUILayout.Width(168));
                        }
                        if (material.HasProperty(EmissionMap) == true)
                        {
                            EditorHelper.ObjectField(material.GetTexture(EmissionMap), GUILayout.Width(168), GUILayout.Height(EditorGUIUtility.singleLineHeight));
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Fix Emission") == true)
            {
                foreach (var material in m_Materials)
                {
                    if (ShouldDisableEmission(material) == true)
                    {
                        material.DisableKeyword(EmissionFlag);
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                    }
                    else
                    {
                        material.EnableKeyword(EmissionFlag);
                        material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                    }
                }
            }
        }

    }

    private static bool ShouldDisableEmission(Material mat)
    {
        if (mat.HasProperty(EmissionMap) && mat.GetTexture(EmissionMap) != null)
        {
            return false;
        }

        if (mat.HasProperty(EmissionColour) && mat.GetColor(EmissionColour).grayscale > 0)
        {
            return false;
        }

        return true;
    }

    private static List<string> GetFilesSelected()
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

    private static List<string> GetFilesAtPath(string path)
    {
        return GetFiles(Directory.GetFiles(path, Filter, SearchOption.AllDirectories).ToList());
    }

    private static List<string> GetFiles(List<string> paths)
    {
        string filter = Filter.Strip("*");
        paths = paths.DistinctList();
        paths.RemoveAll(p => p == null);
        paths.RemoveAll(p => p.Contains(filter) == false);
        return paths.Extract(p => p.Replace("\\", "/"));
    }
}
