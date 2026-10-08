using System.Collections.Generic;

namespace MusicLoudspeakerFormat
{
    public class TransducerInstanceData
    {
        public readonly string UUID;
        public readonly float[] Position;
        public readonly float[] Forward;
        public readonly float[] Up;
        public readonly string TransducerId;
    }

    public class TransducerData
    {
        public readonly string UUID;
        public readonly string Model;

        public readonly float MaxVoltage;
        public readonly float InputVoltage;

        public readonly List<FrequencyData> FrequencyData;
    }

    public class FrequencyData
    {
        public readonly float Frequency;
        public readonly MagPhase Impedance;
        public readonly MagPhase Sensitivity;
        public readonly SphericalHarmonicData Balloon;

        public FrequencyData(float frequency, MagPhase impedance, MagPhase sensitivity, SphericalHarmonicData balloon)
        {
            Frequency = frequency;
            Impedance = impedance;
            Sensitivity = sensitivity;
            Balloon = balloon;
        }
    }

    public class SphericalHarmonicData
    {
        public readonly float[] Mag;
        public readonly float[] Phase;

        public SphericalHarmonicData(float[] mag, float[] phase)
        {
            Mag = mag;
            Phase = phase;
        }
    }

    public class MagPhase
    {
        public float Mag;
        public float Phase;
        public bool IsWrapped;
    }
}