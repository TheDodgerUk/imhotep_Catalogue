using EditorTools.Bundles;
using FilePathHelper;
using UnityEditor;

namespace EditorTools
{
    public class StagingBundlesFolder : BrowseFolder
    {
        public StagingBundlesFolder(BuildTarget platform) 
            : base("Built Bundles", null, 
                  AssetBundleBuilder.StagingFolder(GlobalConsts.CurrentPlatformName()).Absolute) { }

        protected override void OnShowFolder(FilePath path)
        {
            EditorHelper.SelectAndPingObject(path.Relative);
        }

        protected override void OnCleanFolder(FilePath path)
        {
            base.OnCleanFolder(path);

            AssetDatabase.Refresh();
        }
    }
}
