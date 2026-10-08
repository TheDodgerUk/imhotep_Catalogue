using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using System.Linq;

public class DuplicateTimeline
{
    [MenuItem("Timeline/Duplicate With Bindings", true)]
    public static bool DuplicateWithBindingsValidate()
    {
        if (UnityEditor.Selection.activeGameObject == null)
            return false;

        var playableDirector = UnityEditor.Selection.activeGameObject.GetComponent<PlayableDirector>();
        if (playableDirector == null)
            return false;

        var playableAsset = playableDirector.playableAsset;
        if (playableAsset == null)
            return false;

        var path = AssetDatabase.GetAssetPath(playableAsset);
        if (string.IsNullOrEmpty(path))
            return false;

        return true;
    }

    [MenuItem("Timeline/Duplicate With Bindings &t")]
    public static void DuplicateWithBindings()
    {
        if (UnityEditor.Selection.activeGameObject == null)
            return;

        var playableDirector = UnityEditor.Selection.activeGameObject.GetComponent<PlayableDirector>();
        if (playableDirector == null)
            return;

        var playableAsset = playableDirector.playableAsset;
        if (playableAsset == null)
            return;

        var path = AssetDatabase.GetAssetPath(playableAsset);
        if (string.IsNullOrEmpty(path))
            return;

        string newPath = path.Replace(".", "(Clone).");
        if (!AssetDatabase.CopyAsset(path, newPath))
        {
            Debug.LogError("Couldn't Clone Asset");
            return;
        }

        var newPlayableAsset = AssetDatabase.LoadMainAssetAtPath(newPath) as PlayableAsset;
        var gameObject = GameObject.Instantiate(UnityEditor.Selection.activeGameObject);
        gameObject.transform.parent = UnityEditor.Selection.activeGameObject.transform.parent;
        var newPlayableDirector = gameObject.GetComponent<PlayableDirector>();
        newPlayableDirector.playableAsset = newPlayableAsset;

        var oldBindings = playableAsset.outputs.ToArray();
        var newBindings = newPlayableAsset.outputs.ToArray();

        for (int i = 0; i < oldBindings.Length; i++)
        {
            newPlayableDirector.SetGenericBinding(newBindings[i].sourceObject,
                    playableDirector.GetGenericBinding(oldBindings[i].sourceObject)
                );
        }
    }

    [MenuItem("Timeline/PlaySequences &p")]
    public static void PlaySequence()
    {
        if(false == Application.isPlaying)
        {
            Debug.LogWarning("Application must be playing to perform this action");
            return;
        }

        if (UnityEditor.Selection.activeGameObject == null)
            return;

        List<PlayableDirector> directors = new List<PlayableDirector>();
        GameObject[] selected = UnityEditor.Selection.gameObjects;

        for(int i = 0; i < selected.Length; i++)
        {
            var dir = selected[i].GetComponent<PlayableDirector>();
            if(null != dir)
            {
                directors.Add(dir);
            }
        }

        directors.Reverse();
        if (directors.Count > 0)
        {
            directors[0].Play();
            //Play all directors in sequence
            for (int i = 1; i < directors.Count; i++)
            {
                var dir = directors[i];
                var prev = directors[i - 1];
                prev.stopped += (e) =>
                {
                    dir.Play();
                };
            }
        }
    }

    [MenuItem("Timeline/Log Timeline Track Bindings &b")]
    public static void LogTrackBindings()
    {
        if (UnityEditor.Selection.activeGameObject == null)
            return;

        var playableDirector = UnityEditor.Selection.activeGameObject.GetComponent<PlayableDirector>();
        if (playableDirector == null)
            return;

        var playableAsset = playableDirector.playableAsset;
        if (playableAsset == null)
            return;

        
        List<PlayableBinding> bindings = playableAsset.outputs.ToList();

        for (int i = 0; i < bindings.Count; i++)
        {
            var track = bindings[i];
            Debug.Log($"Track: {i} name: {track.streamName}");            
        }
    }
}
