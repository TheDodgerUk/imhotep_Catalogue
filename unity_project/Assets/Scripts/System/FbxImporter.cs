
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class FbxImporter : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        ModelImporter model = assetImporter as ModelImporter;

        if (model == null)
            return;

        // Only apply to FBX files.
        if (!assetPath.EndsWith(
            ".fbx",
            System.StringComparison.OrdinalIgnoreCase))
            return;

        // ========================================
        // QUEST 2 - MODEL IMPORT SETTINGS
        // ========================================

        // Disable unnecessary imported data.
        model.importBlendShapes = false;
        model.importCameras = false;
        model.importLights = false;
        model.importVisibility = false;

        // Mesh optimisation.
        model.meshCompression =
            ModelImporterMeshCompression.Low;

        model.optimizeMeshVertices = true;
        model.optimizeMeshPolygons = true;

        // Reduce memory usage.
        model.isReadable = false;

        // Geometry.
        model.addCollider = false;
        model.keepQuads = false;
        model.weldVertices = true;

        // Normals.
        model.importNormals = ModelImporterNormals.Import;

        // Tangents: keep enabled for normal maps.
        model.importTangents =
            ModelImporterTangents.CalculateMikk;

        // Lightmap UVs.
        // Enable only for models that need generated
        // UVs for baked lighting.
        model.generateSecondaryUV = false;
    }
}

#endif
