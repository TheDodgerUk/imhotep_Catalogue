using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor;

namespace AB
{
    [TrackColor(241f / 255f, 249f / 255f, 99f / 255f)]
    [TrackBindingType(typeof(EyeEmotion))]
    [TrackClipType(typeof(EmotionClip))]
    public class EmotionControlTrack : TrackAsset
    {

    }
}
