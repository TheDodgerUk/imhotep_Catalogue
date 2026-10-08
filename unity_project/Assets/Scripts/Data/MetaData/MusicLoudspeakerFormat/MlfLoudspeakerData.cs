using System.Collections.Generic;

namespace MusicLoudspeakerFormat
{
    public class LoudspeakerData
    {
        public readonly string UUID;
        public readonly string Model;

        public readonly HandleData Handles;

        public readonly List<TransducerInstanceData> TransducerInstances = new List<TransducerInstanceData>();
        public readonly List<TransducerData> Transducers = new List<TransducerData>();

        public readonly List<InputChannelData> InputChannels = new List<InputChannelData>();
        public readonly List<DspPresetData> DspPresets = new List<DspPresetData>();
        public readonly List<PassiveFilterData> PassiveFilters = new List<PassiveFilterData>();
        public readonly List<AmplifierData> Amplifiers = new List<AmplifierData>();
    }

    public class HandleData
    {
        public readonly float[] Default;
        public readonly float[] Ground;
        public readonly float[] Flown;
    }
}