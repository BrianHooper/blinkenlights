using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class IndexModuleData : IDatabaseData
    {
        public string Key() => typeof(IndexModuleData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public List<ModulePlacementData> Modules { get; init; }
    }
}
