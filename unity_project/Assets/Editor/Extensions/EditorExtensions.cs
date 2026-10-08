using FilePathHelper;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    public static class EditorExtensions
    {
        public static GUIStyle Clone(this GUIStyle style) => new GUIStyle(style);

        public static GUIContent ToGUI(this string str) => new GUIContent(str);

        public static GUIStyle Anchor(this GUIStyle style, TextAnchor anchor)
        {
            return new GUIStyle(style) { alignment = anchor };
        }
        public static GUIStyle WordWrap(this GUIStyle style, bool wordWrap)
        {
            return new GUIStyle(style) { wordWrap = wordWrap };
        }

        public static GUIStyle Background(this GUIStyle style, float multiplier)
        {
            style = style.Clone();
            var pixels = Texture2D.whiteTexture.GetPixels();
            for (int i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = pixels[i] * multiplier;
            }
            var tex = Texture2D.whiteTexture.Clone();
            style.normal.background = tex;
            tex.SetPixels(pixels);
            tex.Apply();
            return style;
        }

        public static bool HasStaticFlag(this GameObject go, StaticEditorFlags flag)
        {
            return GameObjectUtility.GetStaticEditorFlags(go).HasFlag(flag);
        }

        public static Texture2D Clone(this Texture2D t)
        {
            return new Texture2D(t.width, t.height, t.format, t.mipmapCount > 0);
        }

        public static string GetAssetGuid(this Object asset) => AssetDatabase.AssetPathToGUID(asset.GetAssetPath().Relative);
        public static FilePath GetAssetPath(this Object asset) => FilePath.FromRelative(AssetDatabase.GetAssetPath(asset));

        public static AssetImporter GetImporter(this Object asset) => asset.GetImporter<AssetImporter>();
        public static T GetImporter<T>(this Object asset) where T : AssetImporter
        {
            return AssetImporter.GetAtPath(asset.GetAssetPath().Relative) as T;
        }

        public static string GetAssetBundleName(this Object asset) => GetAssetBundleName(asset.GetAssetPath());
        public static string GetAssetBundleName(FilePath assetPath)
        {
            if (assetPath.Relative == "Assets")
            {
                return string.Empty;
            }

            var importer = AssetImporter.GetAtPath(assetPath.Relative);
            string bundleName = importer?.assetBundleName;
            if (string.IsNullOrEmpty(bundleName) == false)
            {
                return bundleName;
            }
            else
            {
                var folder = assetPath.GetFolder();
                return GetAssetBundleName(folder);
            }
        }
    }
}