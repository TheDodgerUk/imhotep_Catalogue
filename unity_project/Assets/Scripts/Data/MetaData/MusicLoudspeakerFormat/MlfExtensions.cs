using MathNet.Numerics;
using UnityEngine;

namespace MusicLoudspeakerFormat
{
    public static class MlfExtensions
    {
        public static Vector3 ToVector3(this float[] array)
        {
            if (array == null)
            {
                return Vector3.zero;
            }
            return new Vector3(-array[1], array[2], array[0]);
        }

        public static Vector3 ToVector2d(this float[] array)
        {
            if (array == null)
            {
                return Vector3.zero;
            }
            return new Vector3(0, array[1], array[0]);
        }

        public static float[] ToVector3(this Vector3 v)
        {
            return new[] { v.z, -v.x, v.y };
        }

        public static float[] ToVector2d(this Vector3 v)
        {
            return new[] { v.z, v.y };
        }

        public static MagPhase GetMagPhase(this FrequencyResponseData data, int index)
        {
            return new MagPhase { IsWrapped = false, Mag = data.Mag[index], Phase = data.Phase[index] };
        }

        public static Complex32 ToComplex(this MagPhase mp) => Complex32.FromPolarCoordinates(mp.Mag, mp.Phase);
    }
}
