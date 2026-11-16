#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Quest2BatchModelOptimizer : EditorWindow
{
    private bool selectedFoldersOnly = false;
    private bool disableReadWrite = false;
    private bool generateLightmapUVs = false;
    private bool disableTangents = false;
    private bool disableVisibility = false;
    private bool disableDeformPercent = false;
    private ModelImporterMeshCompression compression = ModelImporterMeshCompression.Low;
    private Vector2 scroll;

    [MenuItem("Tools/Quest 2/Batch Optimise Models")]
    private static void Open() => GetWindow<Quest2BatchModelOptimizer>("Quest 2 Models");

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Quest 2 - Batch Model Import Settings", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Changes imported FBX/OBJ/etc. model importers, not .asset meshes. Back up or commit your project first. Reimporting can take time.", MessageType.Info);
        selectedFoldersOnly = EditorGUILayout.ToggleLeft("Only selected Project folders", selectedFoldersOnly);
        compression = (ModelImporterMeshCompression)EditorGUILayout.EnumPopup("Mesh Compression", compression);
        disableReadWrite = EditorGUILayout.ToggleLeft("Disable Read/Write (CAUTION: runtime mesh editing)", disableReadWrite);
        generateLightmapUVs = EditorGUILayout.ToggleLeft("Generate Lightmap UVs (only if needed)", generateLightmapUVs);
        disableTangents = EditorGUILayout.ToggleLeft("Disable Tangents (only if shaders do not use them)", disableTangents);
        disableVisibility = EditorGUILayout.ToggleLeft("Disable Import Visibility", disableVisibility);
        disableDeformPercent = EditorGUILayout.ToggleLeft("Disable Import Deform Percent", disableDeformPercent);
        EditorGUILayout.Space();
        var paths = GetModelPaths();
        EditorGUILayout.LabelField("Imported models found: " + paths.Count);
        if (GUILayout.Button("Apply Settings to Models"))
        {
            if (!EditorUtility.DisplayDialog("Batch Reimport Models", "Update import settings and reimport " + paths.Count + " models? This may take a while.", "Apply", "Cancel")) return;
            Apply(paths);
        }
    }

    private List<string> GetModelPaths()
    {
        string[] folders = null;
        if (selectedFoldersOnly)
        {
            var list = new List<string>();
            foreach (var obj in Selection.GetFiltered<UnityEngine.Object>(SelectionMode.Assets))
            {
                string path = AssetDatabase.GetAssetPath(obj);
                if (AssetDatabase.IsValidFolder(path)) list.Add(path);
            }
            if (list.Count == 0) return new List<string>();
            folders = list.ToArray();
        }
        string[] guids = folders == null ? AssetDatabase.FindAssets("t:Model") : AssetDatabase.FindAssets("t:Model", folders);
        var result = new List<string>();
        var seen = new HashSet<string>();
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (seen.Add(path) && AssetImporter.GetAtPath(path) is ModelImporter) result.Add(path);
        }
        return result;
    }

    private void Apply(List<string> paths)
    {
        int changed = 0, failed = 0;
        try
        {
            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];
                if (EditorUtility.DisplayCancelableProgressBar("Optimising Model Imports", path, (float)i / Math.Max(1, paths.Count))) break;
                try
                {
                    var importer = AssetImporter.GetAtPath(path) as ModelImporter;
                    if (importer == null) continue;
                    importer.importBlendShapes = false;
                    importer.importCameras = false;
                    importer.importLights = false;
                    importer.meshCompression = compression;
                    importer.optimizeMeshPolygons = true;
                    importer.optimizeMeshVertices = true;
                    importer.addCollider = false;
                    importer.keepQuads = false;
                    importer.weldVertices = true;
                    if (disableReadWrite) importer.isReadable = false;
                    if (generateLightmapUVs) importer.generateSecondaryUV = true;
                    if (disableTangents) importer.importTangents = ModelImporterTangents.None;
                    if (disableVisibility) importer.importVisibility = false;
                    importer.SaveAndReimport();
                    changed++;
                }
                catch (Exception ex) { failed++; Debug.LogError("Failed to optimise " + path + ": " + ex); }
            }
        }
        finally { EditorUtility.ClearProgressBar(); }
        Debug.Log($"Quest 2 batch model optimisation complete: {changed} updated, {failed} failed.");
        EditorUtility.DisplayDialog("Finished", $"Updated: {changed}\nFailed: {failed}\nCheck Console for details.", "OK");
    }
}
#endif
