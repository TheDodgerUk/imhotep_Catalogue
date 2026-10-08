#if UNITY_EDITOR

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;



public class CopyToServer
{
    public class ServerCombinedData
    {
        public string FullPath;
        public float SizeMB;
    }
    private const string SECTRET = "2edbd85f271f7d4535e19725eafe4372f450fe8e39a39d01d4c7d2fa4be0bb160e30373340710387d4a9ff6f9561a39518623ebe356e850b6a15120f82d539ffde07ae4a0c260c966a0c6b15dd869f5e1a4fbe1fc02c45f65012529bd5d7d745b2b9cd5bd4b491e3abe7e45d0f9f109e04a7abfd7d453be6a7a00df261f330a4c27284fb62f42bcde10a5992bde9a16d3b6f4a2d3f716cd6145a3df21a3cbb22e7f6ee5b9bce9fd1891bfb6ae49c9e70f1808abcfd3ceeb5e0769eecd61f80f3c0cb5ebbf019aa656df273c6b3dcde7c51bafcc61d07b87dde67b467a3a8e1dd5ee44f54e42fc8f2725e66bbe3399bf70727dd8ee62f013af22f72a9ba706a16dba214d386cde5d0dc01bea33582e5b50004f9a6547d111181c804fb2ca9081df7c903556c39caa36d24f98f65570dd9a70a9cecd7f39ec3a45e667e02bb847810f5ca975b0dc99c7bbb57ada9a7da8b52aba60af3523dd6e1191c352aaf90ca67bd36af3ce7ecb2dc03c910387a0d6acd5e1db997007691c8b5a6cc0f1c80d039ec9a25cb784040ecd99922b5bb667a3747eed8b65b74340db4fe49db7b632e2e22412a6d95b75e931dc30076496a24f5eedc9909bc6351e6ea121882b89fda68ba578e91ec779466aff046b6474eb8e10f1d5beba98d72d6042fa020b3193965148f81a7aeaba3e5db3ba2673995eca1684f91e0a5be52467c8d578e2425cb";
    private const string BASE_TOKEN_ADDRESS = "https://cloud.midasconsoles.com/api/v2";
    private const string TOKEN_ADDRESS = BASE_TOKEN_ADDRESS + "/auth/tokens";
    private const string UUID = "c67d049a-9854-4e4a-916d-1be9fafde57a";


    private const string BASE = "https://room-builder.cloud.midasconsoles.com/builder/v1/";
    private const string ASSET = BASE + "asset";



    private const string AUT = "Authorization";
    private string BEARING = "";
    private const float WAIT_SECONDS = 0.1f;

    private const int NUMBER_OF_RETRIES = 50;


    public static string ConvertBytesToMegabytesString(byte[] data) => ConvertBytesToMegabytes(data.Length).ToString("0.00");
    public static float ConvertBytesToMegabytes(byte[] data) => ConvertBytesToMegabytes(data.Length);
    public static float ConvertBytesToMegabytes(float data) => ((data / 1024f) / 1024f);
    private string BASE_STRING => Device.StreamingPath.ToLower();


    public CopyToServer() { }


    public void UploadJsonFileDirectory(string sourceDirectory, Action callback)
    {
        sourceDirectory = sourceDirectory.SanitiseSlashes();
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(RetrieveToken(() =>
        {
            UploadCatalogueAssetJsonFileDirectory(sourceDirectory, callback);
        }));
    }

    public void UploadJsonFiles(List<string> files, Action callback)
    {
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(RetrieveToken(() =>
        {
            UploadCatalogueAssetJsonFiles(files, callback);
        }));
    }


    public void UploadJsonFile(string fullPath, Action callback)
    {
        fullPath = fullPath.SanitiseSlashes();
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(RetrieveToken(() =>
        {
            UploadCatalogueAssetJsonFile(fullPath, callback);
        }));
    }


    public void UploadAssetDirectory(string source, Action callback)
    {
        source = source.SanitiseSlashes();
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(RetrieveToken(() =>
        {
            UploadCatalogueAssetBundles(source, callback);
        }));
    }

    public void UploadAssetBundle(string file, Action callback)
    {
        file = file.SanitiseSlashes();
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(RetrieveToken(() =>
        {
            UploadCatalogueAssetBundle(file, callback);
        }));
    }

    public void UploadAssetBundles(List<string> files, Action callback)
    {
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(RetrieveToken(() =>
        {
            UploadCatalogueAssetBundles(files, callback);
        }));
    }

    private void UploadCatalogueAssetJsonFileDirectory(string catalogueData, Action callback)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();

        var files = Directory.GetFiles(catalogueData, "*.*", SearchOption.AllDirectories).ToList();
        files.RemoveAll(e => e.EndsWith(".json", StringComparison.CurrentCultureIgnoreCase) == false);

        for (int i = 0; i < files.Count; i++)
        {
            files[i] = files[i].SanitiseSlashes();
            string fullSource = files[i];

            ServerCombinedData data = new ServerCombinedData();
            data.FullPath = fullSource;
            data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length);
            serverDataList.Add(data);
        }


        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, callback));
    }


    private void UploadCatalogueAssetJsonFiles(List<string> fileNames, Action callback)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();
        foreach (var file in fileNames)
        {
            ServerCombinedData data = new ServerCombinedData();
            data.FullPath = file;

            string json = ".json";

            if (data.FullPath.EndsWith(json) == false)
            {
                data.FullPath += json;
            }

            try
            {
                data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length); 
                serverDataList.Add(data);
            }
            catch(Exception e)
            {
                Debug.LogError($"Cannot find file: {data.FullPath}");
            }
        }
        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, callback));
    }


    private void UploadCatalogueAssetJsonFile(string fullPath, Action callback)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();
        ServerCombinedData data = new ServerCombinedData();
        data.FullPath = fullPath;

        string json = ".json";

        if (data.FullPath.EndsWith(json) == false)
        {
            data.FullPath += json;
        }
        data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length);
        serverDataList.Add(data);

        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, callback));
    }


    private void UploadCatalogueAssetBundle(string file, Action callback)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();
        ServerCombinedData data = new ServerCombinedData();
        data.FullPath = file.SanitiseSlashes();
        data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length);
        serverDataList.Add(data);

        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, callback));
    }

    private void UploadCatalogueAssetBundles(List<string> files, Action callback)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();
        foreach (var item in files)
        {
            if (File.Exists(item.SanitiseSlashes()) == true)
            {
                ServerCombinedData data = new ServerCombinedData();
                data.FullPath = item.SanitiseSlashes();
                data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length);
                serverDataList.Add(data);
            }
        }

        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, callback));
    }


    private void UploadCatalogueAssetBundles(string sourcePath, Action callback)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();

        var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories).ToList();
        files.RemoveAll(e => e.EndsWith(".meta"));
        int fileCount = files.Count;

        for (int i = 0; i < files.Count; i++)
        {
            files[i] = files[i].SanitiseSlashes();
            string fullSource = files[i];
            ServerCombinedData data = new ServerCombinedData();
            data.FullPath = fullSource;
            data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length);
            serverDataList.Add(data);
        }


        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, callback));
    }


    private void UploadSingleCatalogueAssetJsonFiles(string fullFile,Action callback)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();

        ServerCombinedData data = new ServerCombinedData();

        data.FullPath = fullFile;
        data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length);
        serverDataList.Add(data);


        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, callback));
    }


    private void UploadSingleCatalogueAssetBundles(string sourceFile)
    {
        List<ServerCombinedData> serverDataList = new List<ServerCombinedData>();

        sourceFile = sourceFile.SanitiseSlashes();

        ServerCombinedData data = new ServerCombinedData();
        data.FullPath = sourceFile;
        data.SizeMB = ConvertBytesToMegabytes(new System.IO.FileInfo(data.FullPath).Length);
        serverDataList.Add(data);

        Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadAllFiles(serverDataList, null));
    }


    private IEnumerator RetrieveToken(Action callback)
    {
        UnityEngine.WWWForm form = new UnityEngine.WWWForm();
        form.AddField("client", UUID);
        UnityWebRequest requestU = UnityWebRequest.Post(TOKEN_ADDRESS, form);

        yield return requestU.SendWebRequest();

        while (requestU.downloadProgress < 1)
        {
            yield return new WaitForSecondsRealtime(WAIT_SECONDS);
        }

        if (IsNetworkResponseValid(requestU) == false)
        {
            Debug.LogError($"responseWeb {requestU.error}");
            Debug.LogError($"Cant connect to web server for loading and saving");
            yield break;
        }

        var parseData = JObject.Parse(requestU.downloadHandler.text);
        var proofSalt = Convert.ToString(parseData["client"]["proofSalt"]);
        var proof = Convert.ToString(parseData["client"]["proof"]);
        var salt = Convert.ToString(parseData["client"]["salt"]);
        var responsePath = Convert.ToString(parseData["responsePath"]);
        responsePath = BASE_TOKEN_ADDRESS + responsePath;


        var secretPlusProofSalt = SECTRET + proofSalt;
        var validCheck = SHA256HexHashString(secretPlusProofSalt).ToLower();
        if (validCheck != proof)
        {
            Debug.LogError("ProofSalt is not matching the proof");
            yield break;
        }

        var secretPlusSalt = SECTRET + salt;
        var response = SHA256HexHashString(secretPlusSalt).ToLower();


        UnityEngine.WWWForm form1 = new UnityEngine.WWWForm();
        form1.AddField("clientResponse", response);
        UnityWebRequest responseToken = UnityWebRequest.Post(responsePath, form1);

        yield return responseToken.SendWebRequest();
        while (responseToken.downloadProgress < 1)
        {
            yield return new WaitForSecondsRealtime(WAIT_SECONDS);
        }
        var parseDataToken = JObject.Parse(responseToken.downloadHandler.text);
        var token = Convert.ToString(parseDataToken["token"]);

        BEARING = "Bearer " + token;
        callback?.Invoke();
    }

    private bool IsNetworkResponseValid(UnityWebRequest request)
    {
        bool valid = (request.isNetworkError == false && string.IsNullOrEmpty(request.error) == true);
        return valid;
    }


    #region maths
    private string SHA256HexHashString(string StringIn)
    {
        string hashString;
        using (var sha256 = SHA256Managed.Create())
        {

            var bytes = new byte[StringIn.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                var s = StringIn.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(s, 16);
            }

            var hash = sha256.ComputeHash(bytes);
            hashString = ToHex(hash);
        }
        return hashString;
    }

    private string ToHex(byte[] bytes)
    {
        StringBuilder result = new StringBuilder(bytes.Length * 2);

        for (int i = 0; i < bytes.Length; i++)
            result.Append(bytes[i].ToString("x2"));

        return result.ToString();
    }

    private string TimeTaken(float seconds)
    {
        float hr = seconds / 3600;
        float v = seconds % 3600;
        float min = v / 60;
        float sec = v % 60;

        string timeTakenString = $"{((int)hr).ToString("D2")}:{((int)min).ToString("D2")}:{((int)sec).ToString("D2")}.";
        return timeTakenString;
    }
    #endregion




    private IEnumerator InternalUploadAllFiles(List<ServerCombinedData> serverDataList, Action callback)
    {
        yield return new WaitForSeconds(WAIT_SECONDS);

        float startTime = (float)UnityEditor.EditorApplication.timeSinceStartup;
        int count = 0;
        float totalSize = 0;
        foreach (var item in serverDataList)
        {
            totalSize += item.SizeMB;
        }



        foreach (var item in serverDataList)
        {
            count++;
            bool fileCompleted = false;

            var bytes = File.ReadAllBytes(item.FullPath);
            if (bytes != null && bytes.Length != 0)
            {
                Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(InternalUploadFile(count, serverDataList.Count, startTime, item.FullPath, bytes, totalSize, () =>
                {
                    fileCompleted = true;
                    totalSize -= item.SizeMB;
                }));
            }
            else
            {
                var filename = Path.GetFileName(item.FullPath);
                if (filename.ToLower() != "keep.json".ToLower())
                {
                    Debug.LogError($" file size is ZERO BYTE  : {item.FullPath}");
                }
                fileCompleted = true;
            }

            yield return new WaitUntil(() => fileCompleted == true);
            yield return new WaitForSecondsRealtime(WAIT_SECONDS);
        }

        float timeTakenSeconds = (float)(UnityEditor.EditorApplication.timeSinceStartup - startTime);
        string timeTakenString = TimeTaken(timeTakenSeconds);

        callback?.Invoke();
        if (callback == null)
        {
            UnityEditor.EditorUtility.DisplayDialog("Uploading File Completed", $"#{serverDataList.Count} files have been uploaded , took  {timeTakenString}", "Ok");
        }
    }

    private float m_TimeTakenSeconds = 0;
    private string m_TimeTakenString = "";
    private string m_AmountLeftString = "";
    private int m_FileNumber;
    private int m_TotalNumberOfFiles;

    private IEnumerator InternalUploadFile(int fileNumber, int totalNumberOfFiles, float startTime, string fullName, byte[] data, float totalSize,  Action callback)
    {
        m_FileNumber = fileNumber;
        m_TotalNumberOfFiles = totalNumberOfFiles;
        int count = 0;

        if(data == null || data.Length == 0)
        {
            Debug.LogError($"Data is ZERO  {fullName}");
            callback?.Invoke();
        }
        while (true)
        {
            count++;
            fullName = fullName.ToLower();
            while (string.IsNullOrEmpty(BEARING))
            {
                yield return new WaitForSecondsRealtime(WAIT_SECONDS);
            }

            List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
            string folderName = fullName.Replace(BASE_STRING, "");
            folderName = folderName.Remove(0, 1); // remove the first /
            string fileName = Path.GetFileName(folderName);
            fileName = fileName.ToLower();

            string newFolderName = folderName.Substring(0, (folderName.Length - (fileName.Length+1)));
            newFolderName = newFolderName.ToLower();

            try
            {
                formData.Add(new MultipartFormFileSection("file", data, fileName, "multipart/form-data"));
                formData.Add(new MultipartFormDataSection("folder", newFolderName));
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception  {e.Message}  fullName {fullName}");
                ErrorWhileSending(ref count);
                callback?.Invoke();
            }

            string webAsset = ASSET;


            UnityWebRequest requestU = UnityWebRequest.Post(webAsset, formData);
            requestU.SetRequestHeader(AUT, BEARING);
            Unity.EditorCoroutines.Editor.EditorCoroutineUtility.StartCoroutineOwnerless(Progress(requestU));
            yield return requestU.SendWebRequest();
            try
            {
                if (IsNetworkResponseValid(requestU) == true)
                {
                    string message = $"File {fileNumber} / {totalNumberOfFiles},  folderName: {webAsset}        {newFolderName}  fileName:{fileName}";
                    float fileMB = ConvertBytesToMegabytes(data.Length);

                    Debug.Log($"Uploading: {message}");


                    m_TimeTakenSeconds = (float)(UnityEditor.EditorApplication.timeSinceStartup - startTime);
                    m_TimeTakenString = TimeTaken(m_TimeTakenSeconds);
                    m_AmountLeftString = ByteSizeLib.ByteSize.FromMegaBytes(totalSize).ToString();
                    UnityEditor.EditorUtility.DisplayCancelableProgressBar($"File {fileNumber} / {totalNumberOfFiles},             {m_AmountLeftString} remaining", $"Upload Progress of file {fileNumber} , TotalTime So far {m_TimeTakenString}", (float)fileNumber / (float)totalNumberOfFiles);
                    Debug.Log($"Uploading Completed:{message}");
                    
                    if(callback == null)
                    {
                        UnityEditor.EditorUtility.ClearProgressBar();
                    }
                    callback?.Invoke();
                    yield break;
                }
                else
                {
                    ErrorWhileSending(ref count);
                }
            }
            catch ( Exception e)
            {
                Debug.LogError($"Exception  {e.Message}");
                ErrorWhileSending(ref count);
                callback?.Invoke();
            }
        }

    }


    IEnumerator Progress(UnityWebRequest req)
    {
        //// Use isdone property of unitywebrequest no need to use additional variable
        while (req.isDone == false)
        {
            UnityEditor.EditorUtility.DisplayCancelableProgressBar($"File {m_FileNumber} / {m_TotalNumberOfFiles},             {m_AmountLeftString} remaining", $"Upload Progress of file {m_FileNumber} , TotalTime So far {m_TimeTakenString}", req.uploadProgress);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ErrorWhileSending(ref int count)
    {
        count++;
        if (count > NUMBER_OF_RETRIES)
        {
            count = 0;
            UnityEditor.EditorUtility.DisplayDialog("Internet Issues", " Cant upload file, check internet", "OK");
        }
    }



    private IEnumerator InternalGetFile()
    {
        while (string.IsNullOrEmpty(BEARING))
        {
            yield return new WaitForSeconds(WAIT_SECONDS);
        }

        string webAsset = ASSET + "/";
        string file = webAsset + "Windows/catalogue/cataloguethumbnail.unity3d";
        file = file.ToLower();
        UnityWebRequest requestU = UnityWebRequest.Get(file);
        requestU.SetRequestHeader(AUT, BEARING);
        while (requestU.downloadProgress < 1)
        {
            yield return new WaitForSeconds(0.05f);
        }


        Debug.LogError($"cataloguethumbnail requestU.downloadedBytes {requestU.downloadedBytes}   file {file}");
    }

}
#endif

