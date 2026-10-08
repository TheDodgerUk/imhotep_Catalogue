using UnityEditor.AI;
using UnityEditor;
using UnityEngine;

public static class OcclusionSceneSettings
{
    public static bool Apply()
    {
        float smallestOccluder = 2f;
        float smallestHole = 1f;
        float backfaceThreshold = 100f;

        bool updated = false;

        if(Mathf.Abs(smallestOccluder - StaticOcclusionCulling.smallestOccluder) > Mathf.Epsilon)
        {
            updated = true;
            StaticOcclusionCulling.smallestOccluder = smallestOccluder;
        }
        if (Mathf.Abs(smallestHole - StaticOcclusionCulling.smallestHole) > Mathf.Epsilon)
        {
            updated = true;
            StaticOcclusionCulling.smallestHole = smallestHole;
        }
        if (Mathf.Abs(backfaceThreshold - StaticOcclusionCulling.backfaceThreshold) > Mathf.Epsilon)
        {
            updated = true;
            StaticOcclusionCulling.backfaceThreshold = backfaceThreshold;
        }

        return updated;
    }
}
