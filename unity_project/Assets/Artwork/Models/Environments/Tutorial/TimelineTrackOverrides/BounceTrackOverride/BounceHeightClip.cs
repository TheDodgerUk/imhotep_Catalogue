using UnityEngine;
using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BounceHeightClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private BounceHeightBehaviour template = new BounceHeightBehaviour();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<BounceHeightBehaviour>.Create(graph, template);
    }
}
