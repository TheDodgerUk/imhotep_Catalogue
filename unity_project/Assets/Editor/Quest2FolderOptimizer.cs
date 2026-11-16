
#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

public class Quest2FolderOptimizer : EditorWindow
{
    [SerializeField]
    private DefaultAsset targetFolder;

    [SerializeField]
    private ModelImporterMeshCompression meshCompression =
        ModelImporterMeshCompression.Low;

    [SerializeField]
    private bool disableReadWrite = true;

    [SerializeField]
    private bool generateLightmapUVs = false;

    [SerializeField]
    private bool disableTangents = false;

    [MenuItem("Tools/Quest 2/Folder Model Optimizer")]
    public static void Open()
    {
        GetWindow<Quest2FolderOptimizer>(
            "Quest 2 Folder Optimizer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Quest 2 - Folder Model Optimizer",
            EditorStyles.boldLabel);

        EditorGUILayout.Space(10);

        targetFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Models Folder",
            targetFolder,
            typeof(DefaultAsset),
            false);

        EditorGUILayout.Space(10);

        meshCompression =
            (ModelImporterMeshCompression)EditorGUILayout.EnumPopup(
                "Mesh Compression",
                meshCompression);

        disableReadWrite = EditorGUILayout.Toggle(
            "Disable Read/Write",
            disableReadWrite);

        generateLightmapUVs = EditorGUILayout.Toggle(
            "Generate Lightmap UVs",
            generateLightmapUVs);

        disableTangents = EditorGUILayout.Toggle(
            "Disable Tangents",
            disableTangents);

        EditorGUILayout.Space(10);

        string folderPath = GetFolderPath();

        if (string.IsNullOrEmpty(folderPath))
        {
            EditorGUILayout.HelpBox(
                "Drag a folder from the Project window.",
                MessageType.Info);

            return;
        }

        string[] modelGuids = AssetDatabase.FindAssets(
            "t:Model",
            new[] { folderPath });

        EditorGUILayout.LabelField(
            "Models Found",
            modelGuids.Length.ToString("N0"));

        EditorGUILayout.Space(10);

        if (GUILayout.Button(
            "Apply Settings to All Models",
            GUILayout.Height(35)))
        {
            ApplySettings(modelGuids);
        }
    }

    private string GetFolderPath()
    {
        if (targetFolder == null)
            return null;

        string path =
            AssetDatabase.GetAssetPath(targetFolder);

        if (!AssetDatabase.IsValidFolder(path))
            return null;

        return path;
    }

    private void ApplySettings(string[] guids)
    {
        int updated = 0;
        int skipped = 0;

        bool confirmed = EditorUtility.DisplayDialog(
            "Optimise Quest 2 Models",
            "Apply import settings to " +
            guids.Length.ToString("N0") +
            " models?\n\n" +
            "This will reimport the models.",
            "Apply",
            "Cancel");

        if (!confirmed)
            return;

        try
        {
            for (int i = 0; i < guids.Length; i++)
            {
                string path =
                    AssetDatabase.GUIDToAssetPath(guids[i]);

                // Only process imported FBX models.
                if (!path.EndsWith(
                    ".fbx",
                    StringComparison.OrdinalIgnoreCase))
                {
                    skipped++;
                    continue;
                }

                ModelImporter importer =
                    AssetImporter.GetAtPath(path)
                    as ModelImporter;

                if (importer == null)
                {
                    skipped++;
                    continue;
                }

                bool cancelled =
                    EditorUtility.DisplayCancelableProgressBar(
                        "Quest 2 Model Optimizer",
                        path,
                        (float)i / Math.Max(1, guids.Length));

                if (cancelled)
                    break;

                // =====================================
                // QUEST 2 IMPORT SETTINGS
                // =====================================

                importer.importBlendShapes = false;
                importer.importCameras = false;
                importer.importLights = false;
                importer.importVisibility = false;

                importer.meshCompression =
                    meshCompression;

                importer.optimizeMeshVertices = true;
                importer.optimizeMeshPolygons = true;

                importer.isReadable =
                    !disableReadWrite;

                importer.addCollider = false;
                importer.keepQuads = false;
                importer.weldVertices = true;

                importer.importNormals =
                    ModelImporterNormals.Import;

                importer.importTangents =
                    disableTangents
                        ? ModelImporterTangents.None
                        : ModelImporterTangents.CalculateMikk;

                importer.generateSecondaryUV =
                    generateLightmapUVs;

                // Save and reimport this model.
                importer.SaveAndReimport();

                updated++;
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        Debug.Log(
            "Quest 2 optimisation finished. " +
            "Updated: " + updated +
            ", Skipped: " + skipped);

        EditorUtility.DisplayDialog(
            "Finished",
            "Updated: " + updated +
            "\nSkipped: " + skipped,
            "OK");
    }
}

#endif
