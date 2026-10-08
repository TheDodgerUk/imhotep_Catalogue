using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace AB
{
    [Serializable]
    public class EmotionClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private EmotionControlBehaviour template = new EmotionControlBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<EmotionControlBehaviour>.Create(graph, template);
        }
    }
}
