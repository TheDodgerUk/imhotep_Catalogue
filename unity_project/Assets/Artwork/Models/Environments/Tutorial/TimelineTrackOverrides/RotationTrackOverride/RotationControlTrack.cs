using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor;
using UnityEngine.Playables;

[TrackColor(141f / 255f, 149f / 255f, 99f / 255f)]
[TrackBindingType(typeof(RotationControl))]
[TrackClipType(typeof(RotationClip))]
public class RotationControlTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<RotationControlMixer>.Create(graph, inputCount);
    }
}