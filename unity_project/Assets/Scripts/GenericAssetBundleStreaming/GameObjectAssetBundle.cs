using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectAssetBundle : GenericAssetBundle<GameObject>
{
    protected override string ASSET_TYPE => nameof(GameObjectAssetBundle);

    public static GameObjectAssetBundle Instance { get; private set; }

    public static void CreateInstance(Transform root)
    {
        if (null == Instance)
        {
            Instance = new GameObjectAssetBundle();
        }
    }

    public override void GetItem(MonoBehaviour host, string name, Action<GameObject> callback)
    {
        base.GetItem(host, name, (callbackObject) =>
        {
            callbackObject.SetActive(false);
            var newItem = GameObject.Instantiate(callbackObject);
            newItem.SetActive(true);
            callback?.Invoke(newItem);
        });
    }
}
