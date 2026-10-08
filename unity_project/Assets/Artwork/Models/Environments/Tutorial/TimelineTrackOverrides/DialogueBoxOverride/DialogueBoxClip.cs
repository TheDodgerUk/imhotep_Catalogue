using UnityEngine;
using System;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace UI
{
    [Serializable]
    public class DialogueBoxClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private DialogueBoxBehaviour template = new DialogueBoxBehaviour();

        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<DialogueBoxBehaviour>.Create(graph, template);
        }
    }
}