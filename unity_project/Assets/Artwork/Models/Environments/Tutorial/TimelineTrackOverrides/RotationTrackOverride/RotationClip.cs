using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class RotationClip : PlayableAsset, ITimelineClipAsset
{
    [SerializeField]
    private RotationControlBehaviour template = new RotationControlBehaviour();

    public ClipCaps clipCaps => ClipCaps.Blending;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        return ScriptPlayable<RotationControlBehaviour>.Create(graph, template);
    }
}