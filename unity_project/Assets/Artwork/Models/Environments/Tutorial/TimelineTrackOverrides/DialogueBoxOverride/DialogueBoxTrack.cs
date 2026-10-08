using UnityEngine.Timeline;

namespace UI
{
    [TrackColor(0f / 255f, 249f / 255f, 99f / 255f)]
    [TrackBindingType(typeof(DialogueBoxControl))]
    [TrackClipType(typeof(DialogueBoxClip))]
    public class DialogueBoxTrack : TrackAsset
    {
    }
}
