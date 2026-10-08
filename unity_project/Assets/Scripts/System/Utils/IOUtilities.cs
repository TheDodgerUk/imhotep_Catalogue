using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Utils
{
    public static class IO
    {
        public static void CleanFolder(string absolute)
        {
            DestroyFolder(absolute);
            ForceFolder(absolute);
        }

        public static bool ForceFolder(string folderPath)
        {
            Directory.CreateDirectory(folderPath);
            return FolderExists(folderPath);
        }

        public static void CleanFolderContents(string folderPath)
        {
            if (FolderExists(folderPath) == true)
            {
                foreach (var file in Directory.GetFiles(folderPath))
                {
                    DeleteFile(file);
                }
            }
        }

        public static void DestroyFolder(string folderPath)
        {
            try
            {
                folderPath = folderPath.DesanitiseSlashes();

                if (FolderExists(folderPath) == false)
                {
                    return;
                }

                foreach (string directory in Directory.GetDirectories(folderPath))
                {
                    DestroyFolder(directory);
                }

                CleanFolderContents(folderPath);

                try
                {
                    DeleteFolder(folderPath);
                }
                catch (IOException ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    throw ex;
                }
                catch (UnauthorizedAccessException ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    throw ex;
                }
            }
            catch (IOException ex)
            {
                UnityEngine.Debug.LogException(ex);

            }
            catch (UnauthorizedAccessException ex)
            {
                UnityEngine.Debug.LogException(ex);
            }
        }

        private static void DeleteFolder(string path)
        {
            string meta = path.TrimEnd('\\', '/') + ".meta";
            if (FileExists(meta) == true)
            {
                DeleteFile(meta);
            }
            if (FolderExists(path) == true)
            {
                Directory.Delete(path, true);
            }
        }

        public static void CopyFile(string sourcePath, string destinationPath)
        {
            File.Copy(sourcePath, destinationPath, true);
        }

#if UNITY_STANDALONE_OSX || UNITY_IOS
        public static void DeleteFile(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath) == true)
                {
                    System.IO.File.Delete(filePath);
                }
                else
                {
                    UnityEngine.Debug.LogError($"Could not find file {filePath}");
                }
            }
            catch(Exception e)
            {
                UnityEngine.Debug.LogError($"DeleteFile error file: {filePath}, Exception {e.Message}");
            }
        }
#else
        public static void DeleteFile(string filePath) => Win32.DeleteFile(Win32.ExtendPath(filePath));
#endif

        public static bool DriveExists(string path) => FolderExists(Path.GetPathRoot(path));
        public static bool FolderExists(string folderPath) => Directory.Exists(folderPath);
        public static bool FileExists(string filePath) => File.Exists(filePath);

        public static string GetFullFileName(string filePath) => Path.GetFileName(filePath);
        public static string GetFileName(string filePath) => Path.GetFileNameWithoutExtension(filePath);

        public static List<string> GetAllFiles(string folderPath, bool allFolders = true)
        {
            return GetFiles(folderPath, "*.*", allFolders);
        }
        public static List<string> GetFiles(string folderPath, string pattern, bool allFolders = true)
        {
            return Directory.GetFiles(folderPath, pattern, GetOption(allFolders)).ToList();
        }

        public static List<string> GetAllFolders(string rootFolderPath, bool allFolders = true)
        {
            return Directory.GetDirectories(rootFolderPath, "*", GetOption(allFolders)).ToList();
        }

        private static SearchOption GetOption(bool allFolders)
        {
            return (allFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public static bool IsFolderEmpty(string folderPath) => IsFolderEmpty(new DirectoryInfo(folderPath));
        public static bool IsFolderEmpty(DirectoryInfo directoryInfo)
        {
            try
            {
                var files = directoryInfo.GetFiles("*.*");
                files = files.Where(f => !IsMetaFile(f)).ToArray();
                return (files == null || files.Length == 0);
            }
            catch { }
            return false;
        }

        private static bool IsMetaFile(FileInfo file) => file.Extension == ".meta";

        public static byte[] ReadBytesFromFile(string filePath) => ReadFromFile(filePath, File.ReadAllBytes);
        public static string ReadTextFromFile(string filePath) => ReadFromFile(filePath, File.ReadAllText);

        private static T ReadFromFile<T>(string filePath, Func<string, T> handler) where T : class
        {
            // check if folder exists, if not fail
            if (FileExists(filePath) == false)
            {
                UnityEngine.Debug.LogError($"Failed to load file at: \"{filePath}\". Path does not exist!\n");
                return null;
            }

            // try to open file and read text
            try
            {
                return handler(filePath);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log($"Failed to load contents of file: \"{filePath}\"\n{e.Message}");
            }
            return null;
        }

        public static void WriteBytesToFile(string filePath, byte[] bytes) => WriteToFile(filePath, bytes, File.WriteAllBytes);
        public static void WriteTextToFile(string filePath, string text) => WriteToFile(filePath, text, File.WriteAllText);

        private static void WriteToFile<T>(string filePath, T contents, Action<string, T> handler)
        {
            ForceFolder(Path.GetDirectoryName(filePath));
            handler(filePath, contents);
        }

        public static bool OpenFolder(string path)
        {
            if (FolderExists(path) == true)
            {
                if (path.StartsWith("/") == true)
                {
                    path = "file:" + path;
                }

                try
                {
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    return false;
                }
            }
            return true;
        }
    }
}
