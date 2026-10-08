using System.Collections.Generic;

namespace MusicLoudspeakerFormat
{
    public class MechanicalData
    {
        public readonly string UUID;
        public readonly string Model;

        public readonly List<string> Connections;
        public readonly float[] CentreOfMass;
        public readonly float Mass;

        public readonly List<float[]> PickPoints;
        public readonly Dictionary<int, float> LoadLimit;

        public readonly Dictionary<MechanicalConfigurationType, MechanicalConfigurationData> Configurations;
    }

    public class MechanicalConfigurationData
    {
        public readonly float[] PinToPivot;
        public readonly float PinToPivotWidth;

        public readonly float[] PivotPin;
        public readonly float PivotPinWidth;

        public readonly List<float[]> SelectionPins;
        public readonly List<float> SplayAngles;

        public readonly MechanicalSetupData Default;
        public readonly Dictionary<string, MechanicalSetupData> Overrides = new Dictionary<string, MechanicalSetupData>();
    }

    public class MechanicalSetupData
    {
        public readonly float[] PinToSelection;
        public readonly float DropLinkLength;
    }

    public enum MechanicalConfigurationType { Flown, Stacked, }
}
