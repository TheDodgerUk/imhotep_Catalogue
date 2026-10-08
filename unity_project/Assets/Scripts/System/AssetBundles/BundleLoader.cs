using System;
using System.Collections.Generic;
using UnityEngine;

public class BundleLoader
{
    public class DependencyData
    {
        public string PrefabName;
        public string BundleName;
        public string FullName;
        public string GUID;
        public int Version;
        public float SizeMB;
    }

    public static readonly string DependencyPath = Device.StreamingPath + "/bundles/master";
    private static readonly string BundlePath = Device.StreamingPath + "/bundles/{0}";
    private const string BundleExtension = ".unity3d";

    private Dictionary<string, List<Action<AssetBundle>>> m_PendingCallbacks;
    private Dictionary<string, List<DependencyData>> m_BundleDependencies;
    private Dictionary<string, AssetBundleRef> m_LoadedBundles;

    public void LoadAssetDatabase(Action callback)
    {
        m_PendingCallbacks = new Dictionary<string, List<Action<AssetBundle>>>();
        m_LoadedBundles = new Dictionary<string, AssetBundleRef>();

        m_BundleDependencies = Json.JsonNet.ReadFromFile<Dictionary<string, List<DependencyData>>>(DependencyPath);
        callback();
    }

    public void Unload(string path, int version, bool allObjects)
    {
        string key = GetBundleKey(path, version);
        if (m_LoadedBundles.ContainsKey(key) == true)
        {
            m_LoadedBundles[key].Bundle.Unload(allObjects);
            m_LoadedBundles.Remove(key);
        }
    }

    public void UnloadAll()
    {
        foreach (var kvp in m_LoadedBundles)
        {
            kvp.Value.Bundle.Unload(true);
        }

        m_LoadedBundles.Clear();
    }

    public void GetBundle(string bundleName, int version, Action<AssetBundle> callback, MonoBehaviour host)
    {
        RetrieveBundle(AppendExtension(bundleName), version, callback, host);
    }

    private void RetrieveBundle(string bundleName, int version, Action<AssetBundle> callback, MonoBehaviour host)
    {
        if(m_LoadedBundles == null)
        {
            m_LoadedBundles = new Dictionary<string, AssetBundleRef>();
        }
        // check if bundle is already loaded
        string key = GetBundleKey(bundleName, version);
        if (m_LoadedBundles.ContainsKey(key) == false)
        {
            LoadBundle(bundleName, version, callback, host);
        }
        else
        {
            callback(m_LoadedBundles[key].Bundle);
        }
    }

    private void LoadBundle(string bundleName, int version, Action<AssetBundle> callback, MonoBehaviour host)
    {
        // register callback, continue if unique
        if (AddCallback(bundleName, callback) == false)
        {
            return;
        }

        // grab dependencies that need loading
        var dependencies = GetDependencies(bundleName);
        var task = new TaskAction(dependencies.Count + 1, () => InvokeCallback(bundleName, version));
        for (int i = 0; i < dependencies.Count; ++i)
        {
            // load each one 
            RetrieveBundle(dependencies[i].FullName, version, bundle => task.Increment(), host);
        }

        // load the bundle
        string path = string.Format(BundlePath, bundleName);
        host.WaitForOp(AssetBundle.LoadFromFileAsync(path), request =>
        {
            // create new ref & add to dict
            var bundleRef = new AssetBundleRef(request.assetBundle, bundleName, version);
            string key = GetBundleKey(bundleName, version);
            m_LoadedBundles.Add(key, bundleRef);

            // invoke callback
            task.Increment();
        });
    }

    private bool AddCallback(string bundleName, Action<AssetBundle> callback)
    {
        if (m_PendingCallbacks == null)
        {
            m_PendingCallbacks = new Dictionary<string, List<Action<AssetBundle>>>();
        }

        if (m_PendingCallbacks.ContainsKey(bundleName) == false)
        {
            m_PendingCallbacks.Add(bundleName, new[] { callback }.ToList());
            return true;
        }
        else
        {
            m_PendingCallbacks[bundleName].Add(callback);
            return false;
        }
    }

    private void InvokeCallback(string bundleName, int version)
    {
        if (m_PendingCallbacks.ContainsKey(bundleName) == true)
        {
            string key = GetBundleKey(bundleName, version);
            var bundle = m_LoadedBundles[key].Bundle;
            if (bundle == null)
            {
                Debug.LogError($"Asset bundle: \"{bundleName}\" failed to load\n");
            }
            m_PendingCallbacks[bundleName].ForEach(c => c(bundle));
            m_PendingCallbacks.Remove(bundleName);
        }
    }

    private List<DependencyData> GetDependencies(string bundleName)
    {
        if(m_BundleDependencies == null)
        {
            m_BundleDependencies = new Dictionary<string, List<DependencyData>>();
        }
        return m_BundleDependencies.Get(bundleName) ?? new List<DependencyData>();
    }

    private static string GetBundleKey(string path, int version) => string.Concat(path, "_", version);

    private static string AppendExtension(string bundleName)
    {
        if (bundleName.EndsWith(BundleExtension) == false)
        {
            return $"{bundleName}{BundleExtension}";
        }
        return bundleName;
    }

    private class AssetBundleRef
    {
        public AssetBundle Bundle { get; private set; }
        public int Version { get; private set; }
        public string Url { get; private set; }

        public AssetBundleRef(AssetBundle bundle, string url, int version)
        {
            Bundle = bundle;
            Version = version;
            Url = url;
        }
    };
}
