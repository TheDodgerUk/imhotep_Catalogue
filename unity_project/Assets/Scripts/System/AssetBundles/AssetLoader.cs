using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader
{
    private BundleLoader m_BundleLoader;
    private int m_Version = 0;

    public void Initialise(Action callback)
    {
        m_BundleLoader = new BundleLoader();
        m_BundleLoader.LoadAssetDatabase(callback);
    }

    public void Shutdown()
    {
        m_BundleLoader?.UnloadAll();
        m_BundleLoader = null;
    }

    public void LoadBundle(MonoBehaviour host, string bundleName, Action<AssetBundle> callback)
    {
        if (m_BundleLoader == null)
        {
            m_BundleLoader = new BundleLoader();
        }
        // load or retrieve bundle from loader
        m_BundleLoader.GetBundle(bundleName, m_Version, callback, host);
    }

    public void LoadAsset<T>(MonoBehaviour host, string assetName, string bundleName, Action<T> callback) where T : UnityEngine.Object
    {
        // load from resources or bundles?
        LoadBundle(host, bundleName, bundle =>
        {
            if (bundle != null)
            {
                // load asset from bundle
                host.WaitForOp(bundle.LoadAssetAsync<T>(assetName), op =>
                {
                    callback(op.asset as T);
                });
            }
            else
            {
                callback(null);
            }
        });
    }

    public void WhatsInAssetBundle(MonoBehaviour host, string bundleName, Action<List<string>> callBackNames)
    {
        LoadBundle(host, bundleName, (bundle) =>
        {
            if (bundle != null)
            {
                List<string> without = new List<string>();
                foreach (var item in bundle.GetAllAssetNames())
                {
                    var fileName = System.IO.Path.GetFileNameWithoutExtension(item);
                    without.Add(fileName);

                }
                callBackNames?.Invoke(without);
            }
            else
            {
                callBackNames?.Invoke(new List<string>());
            }
        });
    }
}
