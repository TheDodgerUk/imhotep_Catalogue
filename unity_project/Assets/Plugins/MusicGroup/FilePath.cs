using System.IO;
using System.Linq;
using UnityEngine;

namespace FilePathHelper
{
    public class FilePath
    {
        public readonly string Extension;
        public readonly string FileName;
        public readonly string Absolute;
        public readonly string Relative;

        private FilePath(string absolute, string relative)
        {
            Absolute = absolute.SanitiseSlashes();
            Relative = relative.SanitiseSlashes();

            var i = Path.GetInvalidPathChars().ToList();
            if (i.Exists(c => Relative.Contains(c.ToString())) == false)
            {
                Extension = Path.GetExtension(Relative);
                FileName = Path.GetFileName(Relative);
            }
        }

        public override string ToString() => Absolute;

        public FilePath InFolder(FilePath folderPath) => folderPath.Append(FileName);

        public FilePath GetFolder() => FromAbsolute(Absolute.Strip($"/{FileName}"));

        public FilePath Append(string suffix) => AppendExt($"/{suffix}");
        public FilePath AppendExt(string suffix) => new FilePath($"{Absolute}{suffix}", $"{Relative}{suffix}");
        public FilePath RemoveExt() => new FilePath(Absolute.Strip(Extension), Relative.Strip(Extension));

        public FilePath Strip(params string[] toRemove) => new FilePath(Absolute.Strip(toRemove), Relative.Strip(toRemove));

        private FilePath ChangeRoot(string absolute, string relative, string to) => new FilePath(Absolute.Replace(absolute, to), Relative.Replace(relative, to));
        public FilePath ChangeRoot(FilePath from, string to) => ChangeRoot(from.Absolute + "/", from.Relative + "/", to);
        public FilePath ChangeRoot(string from, string to) => ChangeRoot(from, from, to);

        public static FilePath FromAbsolute(string absolute) => new FilePath(absolute, ToRelativePath(absolute, Application.dataPath));
        public static FilePath FromRelative(string relative) => new FilePath(ToAbsolutePath(relative, Application.dataPath), relative);
        public static FilePath FromStreaming(string streaming) => FromAbsolute($"{Application.streamingAssetsPath}/{streaming}");

        private static string ToAbsolutePath(string relative, string basePath) => basePath.Replace("Assets", relative.SanitiseSlashes());
        private static string ToRelativePath(string absolute, string basePath) => absolute.SanitiseSlashes().Replace(basePath, "Assets");
    }

    static class FilePathExtensions
    {
        public static string SanitiseSlashes(this string str) => str?.Replace('\\', '/');
        public static string DesanitiseSlashes(this string str) => str.DesanitiseSlashes(Path.DirectorySeparatorChar);
        public static string DesanitiseSlashes(this string str, char pathChar) => str?.Replace('/', pathChar);

        public static string Strip(this string str, params string[] toRemove)
        {
            for (int i = 0; i < toRemove.Length; ++i)
            {
                str = str?.Replace(toRemove[i], "");
            }
            return str;
        }
    }
}
