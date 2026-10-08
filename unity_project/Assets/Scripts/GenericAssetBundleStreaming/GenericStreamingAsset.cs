using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;


public abstract class GenericStreamingAsset<T> where T : UnityEngine.Object
{
    protected List<string> m_StreamingNames = new List<string>();

    protected virtual List<string> ValidExtensions => new List<string>() { "*.virtual", "*.virtual" };
    protected virtual string STREAMING_FOLDER => $"".ToLower();


    protected void Setup()
    {
    }

    public void OnPreEnvironmentLoad()
    {
        OnSceneLoaded(null);

    }

    protected void OnSceneLoaded(Action callback)
    {
        m_StreamingNames.Clear();

        if (Directory.Exists(STREAMING_FOLDER) == true)
        {
            List<string> streamList = DirSearch(STREAMING_FOLDER, ValidExtensions);
            for (int i = 0; i < streamList.Count; i++)
            {
                streamList[i] = streamList[i].Replace(@"\", @"/");
            }
            for (int i = 0; i < streamList.Count; i++)
            {
                string name = Path.GetFileName(streamList[i]);
                m_StreamingNames.Add(name);
            }
        }
        callback?.Invoke();
    }

    protected List<String> DirSearch(string sDir, List<string> searchPtn)
    {
        List<String> files = new List<String>();
        try
        {
            foreach (string ptn in searchPtn)
            {
                foreach (string f in Directory.GetFiles(sDir, ptn))
                {
                    files.Add(f);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Message: {e.Message}");
        }

        return files.Distinct().ToList();
    }



    #region audioClip


    public void GetItem(MonoBehaviour mono, string fileName, Action<T> callback)
    {
        mono.StartCoroutine(RequestStreamingRoutine($"{STREAMING_FOLDER}/{fileName}", callback));
    }

    public void GetItem(Action<List<string>> callback)
    {
        callback?.Invoke(m_StreamingNames);
    }

    public void GetItemList(string NameStartsWith, Action<List<string>> callback)
    {
        List<string> startsWith = new List<string>();
        foreach (var item in m_StreamingNames)
        {
            if (true == item.StartsWith(NameStartsWith, StringComparison.CurrentCultureIgnoreCase))
            {
                startsWith.Add(item);
            }
        }
        callback?.Invoke(startsWith);
    }

    public void GetItemList(Action<List<string>> callback)
    {
        List<string> startsWith = new List<string>();
        foreach (var item in m_StreamingNames)
        {
            startsWith.Add(System.IO.Path.GetFileNameWithoutExtension(item));
        }
        callback?.Invoke(startsWith);
    }


    #endregion


    protected abstract IEnumerator RequestStreamingRoutine(string fullName, Action<T> callback = null);




    protected static GameObject GetSingletonRoot()
    {
        const string RootName = "Singletons";
        var root = GameObject.Find(RootName);
        if (root == null)
        {
            root = new GameObject(RootName);
            root.transform.Reset(true);
        }
        return root;
    }
}
