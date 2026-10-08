using System.Collections.Generic;

namespace EditorTools.Environments
{
    public class EnvironmentSceneData
    {
        public LocalisedValue Name { get; private set; }

        public string Guid;
        public List<EnvironmentVariantData> Variants;

        public EnvironmentSceneData()
        {
            Guid = Utils.Code.GenerateGuid();
            Variants = new List<EnvironmentVariantData>();
            OnPostLoad();
        }

        public void OnPostLoad() => Name = new LocalisedValue(EnvironmentData.EnvironmentKey, Guid, "name");
    }
}
