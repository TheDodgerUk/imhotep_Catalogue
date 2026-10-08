using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class WaitExtensions
{
    public static void WaitForClone(this MonoBehaviour mono, string path, Action<GameObject> callback)
    {
        mono.WaitForResources<GameObject>(path, prefab =>
        {
            if (prefab == null)
            {
                Debug.LogError($"Prefab was null at path \"{path}\"\n");
            }
            callback(Utils.Unity.Clone(prefab));
        });
    }

    public static void WaitForResources<T>(this MonoBehaviour mono, string path, Action<T> callback) where T : UnityEngine.Object
    {
        mono.WaitForOp(Resources.LoadAsync<T>(path), op => callback(op.asset as T));
    }

    public static void WaitForReal(this MonoBehaviour mono, float seconds, Action callback)
    {
        float endTime = Time.realtimeSinceStartup + seconds;
        mono.WaitUntil(1, () => Time.realtimeSinceStartup >= endTime, callback);
    }

    public static void WaitFor(this MonoBehaviour mono, float seconds, Action callback)
    {
        mono.WaitForYield(new WaitForSeconds(seconds), callback);
    }

    public static void WaitForEndOfFrame(this MonoBehaviour mono, Action callback)
    {
        mono.WaitForYield(new WaitForEndOfFrame(), callback);
    }

    public static void WaitForFrame(this MonoBehaviour mono, Action callback)
    {
        mono.StartCoroutine(WaitForYield(null, callback));
    }

    public static void WaitForFrames(this MonoBehaviour mono, int frames, Action callback)
    {
        mono.StartCoroutine(WaitForFrames(frames, callback));
    }

    public static void WaitForOp<U>(this MonoBehaviour mono, U operation, Action<U> callback) where U : AsyncOperation
    {
        mono.WaitUntil(1, () => operation.isDone, () => callback(operation));
    }

    public static void WaitForWeb(this MonoBehaviour mono, WWW www, Action<WWW> callback)
    {
        mono.WaitUntil(1, () => www.isDone, () => callback(www));
    }

    public static void WaitForRequest(this MonoBehaviour mono, UnityWebRequest request, Action<UnityWebRequest> callback)
    {
        mono.WaitForOp(request.SendWebRequest(), op => callback(op.webRequest));
    }

    public static void WaitUntil(this MonoBehaviour mono, int frames, Func<bool> handler, Action callback)
    {
        mono.StartCoroutine(WaitForHandler(frames, handler, callback));
    }

    public static void LoopUntil(this MonoBehaviour mono, Func<bool> handler, Action onFrame)
    {
        mono.StartCoroutine(LoopForHandler(handler, onFrame));
    }

    public static void LoopForWeb(this MonoBehaviour mono, WWW www, Action<WWW> onFrame)
    {
        mono.LoopUntil(() => www.isDone, () => onFrame(www));
    }

    public static void WaitForYield(this MonoBehaviour mono, YieldInstruction instruction, Action callback)
    {
        mono.StartCoroutine(WaitForYield(instruction, callback));
    }

    private static IEnumerator WaitForYield(YieldInstruction instruction, Action callback)
    {
        yield return instruction;

        callback();
    }

    private static IEnumerator LoopForHandler(Func<bool> handler, Action onFrame)
    {
        while (handler != null && handler() == false)
        {
            onFrame();
            yield return null;
        }
        onFrame();
    }

    private static IEnumerator WaitForHandler(int frames, Func<bool> handler, Action callback)
    {
        while (handler != null && handler() == false)
        {
            for (int i = 0; i < frames; ++i)
            {
                yield return null;
            }
        }

        callback?.Invoke();
    }

    private static IEnumerator WaitForFrames(int frames, Action callback)
    {
        for (int i = 0; i < frames; ++i)
        {
            yield return null;
        }
        callback();
    }
}
