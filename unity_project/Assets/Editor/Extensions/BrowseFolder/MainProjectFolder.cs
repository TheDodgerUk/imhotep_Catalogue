using FilePathHelper;
using UnityEngine;

namespace EditorTools
{
    public class MainProjectFolder : BrowseFolder
    {
        private static readonly string Key = (nameof(MainProjectFolder) + "_assets").ShaHash();
        private const string StreamingPathFromRepo = ProjectPathFromRepo + "/StreamingAssets";
        private const string ProjectPathFromRepo = "Assets";
        private const string DefaultProjectName = "imhotep";

        public MainProjectFolder() : base("Main Project", Key, GetDefaultPath(), StreamingPathFromRepo, DefaultProjectName, new[] { GlobalConsts.DataFolder, GlobalConsts.BundleFolder }) { }

        public static string GetDefaultPath()
        {
            var repoPath = FilePath.FromAbsolute(Application.dataPath.Strip($"/{ProjectPathFromRepo}"));
            return repoPath.GetFolder().Append(DefaultProjectName).Absolute;
        }
    }
}
