using System.Collections.Generic;

namespace MusicLoudspeakerFormat
{
    public class InputChannelData
    {
        public readonly string[] ChildIds;
    }

    public class DspPresetData
    {
        public readonly string Name;
        public readonly string Info;
        public readonly int Number;

        public readonly List<DspPresetChannelData> Channels;
    }

    public class DspPresetChannelData
    {
        public readonly string UUID;
        public readonly List<string> ChildIds;

        public readonly float Gain;
        public readonly float Delay;
        public readonly FrequencyResponseData FrequencyResponse;
    }

    public class AmplifierData
    {
        public readonly string UUID;
        public readonly List<string> ChildIds;

        public readonly float Gain;
        public readonly FrequencyResponseData FrequencyResponse;
    }

    public class PassiveFilterData
    {
        public readonly string UUID;
        public readonly List<string> ChildIds;

        public readonly float VoltageSplit;
        public readonly FrequencyResponseData FrequencyResponse;
    }

    public class FrequencyResponseData
    {
        public readonly List<float> Frequencies;
        public readonly float[] Mag;
        public readonly float[] Phase;
        public readonly bool IsWrapped;
    }
}