using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor;
using UnityEngine.Playables;

namespace AB
{
    [TrackColor(41f / 255f, 149f / 255f, 256f / 255f)]
    [TrackBindingType(typeof(FaceUI))]
    [TrackClipType(typeof(HeadEyeClip))]
    public class HeadEyeControlTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<HeadEyeControlMixer>.Create(graph, inputCount);
        }
    }
}