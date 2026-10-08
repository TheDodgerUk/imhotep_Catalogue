using EditorTools;
using FilePathHelper;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class PbrMaterialMenu
{
    private static readonly List<string> Albedo = new[] { "Albedo", "Alb" }.ToList();
    private static readonly List<string> Normal = new[] { "Normal", "Norm", "Nrm" }.ToList();
    private static readonly List<string> Emission = new[] { "Emission", "Emis" }.ToList();
    private static readonly List<string> Pbr = new[] { "PBR", "pbr" }.ToList();

    private const string MenuRoot = "Assets/PBR Material/";

    [MenuItem(MenuRoot + "Populate Material", true)]
    public static bool PopulateMaterialValidate() => Selection.activeObject is Material;

    [MenuItem(MenuRoot + "Populate Material")]
    public static void PopulateMaterial() => PopulateMaterial(Selection.activeObject as Material);

    public static void PopulateMaterial(Material material)
    {
        using (var progressBar = new ProgressBar($"Populating {material.name}", 5))
        {
            progressBar.UpdateIncrement("Searching for Textures");

            var folderPath = material.GetAssetPath().GetFolder();
            string baseName = material.name.Strip("_Mat");
            var albedo = GetTexture(folderPath, baseName, Albedo);
            var normal = GetTexture(folderPath, baseName, Normal);
            var emission = GetTexture(folderPath, baseName, Emission);
            var pbr = GetTexture(folderPath, baseName, Pbr);

            if (albedo == null)
            {
                Debug.LogError("Could not find Albedo texture.\n");
                return;
            }

            // ignore emission if not validated
            progressBar.UpdateIncrement("Validing Emission Map");
            if (emission == null || ValidateEmission(emission) == false)
            {
                emission = null;
            }
            else
            {
                var importer = emission.GetImporter<TextureImporter>();
                if (importer.isReadable == true)
                {
                    importer.isReadable = false;
                    importer.SaveAndReimport();
                }
            }

            progressBar.UpdateIncrement("Configuring Normal Map");
            if (normal != null)
            {
                var normalImporter = normal.GetImporter<TextureImporter>();
                if (normalImporter.textureType != TextureImporterType.NormalMap)
                {
                    normalImporter.textureType = TextureImporterType.NormalMap;
                    normalImporter.SaveAndReimport();
                }
            }
            progressBar.UpdateIncrement("Applying textures");

            material.SetTexture("_MainTex", albedo);
            material.SetTexture("_BumpMap", normal);
            material.SetTexture("_MetallicGlossMap", pbr);
            material.SetTexture("_OcclusionMap", pbr);
            material.SetTexture("_EmissionMap", emission);

            material.EnableKeyword("_METALLICGLOSSMAP");
            material.EnableKeyword("_NORMALMAP");

            const string kEmissionKeyword = "_EMISSION";
            if (emission != null)
            {
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
                material.EnableKeyword(kEmissionKeyword);
            }
            else
            {
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                material.DisableKeyword(kEmissionKeyword);
            }
            progressBar.UpdateIncrement("Reimporting");
            material.GetImporter().SaveAndReimport();
        }
    }

    private static bool ValidateEmission(Texture2D emission)
    {
        var importer = emission.GetImporter<TextureImporter>();
        if (importer.isReadable == false)
        {
            importer.isReadable = true;
            importer.SaveAndReimport();
        }
        var pixels = emission.GetPixels();
        for (int i = 0; i < pixels.Length; ++i)
        {
            if (pixels[i].maxColorComponent != 0)
            {
                return true;
            }
        }
        return false;
    }

    private static Texture2D GetTexture(FilePath folderPath, string baseName, List<string> possibleNames)
    {
        for (int i = 0; i < possibleNames.Count; ++i)
        {
            var path = folderPath.Append($"{baseName}_{possibleNames[i]}.png");
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path.Relative);
            if (texture != null)
            {
                return texture;
            }
        }
        return null;
    }
}
