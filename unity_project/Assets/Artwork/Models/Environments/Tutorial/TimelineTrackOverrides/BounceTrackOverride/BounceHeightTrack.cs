using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(241f / 255f, 149f / 255f, 99f / 255f)]
[TrackBindingType(typeof(BounceHeight))]
[TrackClipType(typeof(BounceHeightClip))]
public class BounceHeightTrack : TrackAsset
{

    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<BounceHeightMixer>.Create(graph, inputCount);
    }
}
