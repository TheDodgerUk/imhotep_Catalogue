using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AB
{
    public class HeadEyeClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private HeadEyeControlBehaviour template = new HeadEyeControlBehaviour();

        public ClipCaps clipCaps => ClipCaps.Blending;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<HeadEyeControlBehaviour>.Create(graph, template);
        }
    }
}