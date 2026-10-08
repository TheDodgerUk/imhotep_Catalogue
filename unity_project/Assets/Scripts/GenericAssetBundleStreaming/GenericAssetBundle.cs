using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericAssetBundle<T> where T : UnityEngine.Object
{
    protected virtual  string ASSET_TYPE => nameof(MaterialAssetBundle);

    public virtual string AssetBundleLocation => $"windows/environment/{PlayerPrefs.GetString(GlobalConsts.ENVIRONMENT_COMBINED_NAME_SHORT)}/{ASSET_TYPE}".ToLower();

    private Dictionary<string, T> m_Stored = new Dictionary<string, T>();
    private MonoBehaviour m_Mono;
    public AssetLoader m_AssetLoader;
    public void Setup(MonoBehaviour mono)
    {
        m_Mono = mono;
        m_AssetLoader = new AssetLoader();

    }

    public virtual void GetItem(MonoBehaviour host, string name, Action<T> callback)
    {
        MonoBehaviour mono = host;
        if(mono == null)
        {
            mono = m_Mono;
        }
        if (true == string.IsNullOrEmpty(name))
        {
            callback?.Invoke(default(T));
        }
        else
        {
            if (m_Stored.ContainsKey(name) == false)
            {
                m_AssetLoader.LoadAsset<T>(mono, name, AssetBundleLocation, (item) =>
                {
                    // need to re-check because of the time delay 
                    if (m_Stored.ContainsKey(name) == false)
                    {
                        m_Stored.Add(name, item);
                    }
                    callback?.Invoke(item);
                });
            }
            else
            {
                callback?.Invoke(m_Stored[name]);
            }

        }
    }



    public void GetItemList(MonoBehaviour host, Action<List<string>> callbackList)
    {
        if(m_AssetLoader == null)
        {
            m_AssetLoader = new AssetLoader();
        }
        m_AssetLoader.WhatsInAssetBundle(host, AssetBundleLocation, callbackList);
    }

    public void GetItemList(MonoBehaviour host, string NameStartsWith, Action<List<string>> callbackList)
    {
        if (m_AssetLoader == null)
        {
            m_AssetLoader = new AssetLoader();
        }
        m_AssetLoader.WhatsInAssetBundle(host, AssetBundleLocation, (list) =>
        {
            List<string> contents = new List<string>();
            foreach (var item in list)
            {
                string lower = item.ToLower();
                if (true == lower.StartsWith(NameStartsWith.ToLower()))
                {
                    contents.Add(item);
                }
            }
            callbackList?.Invoke(contents);
        });
    }






}
