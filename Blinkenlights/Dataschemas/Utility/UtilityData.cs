using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class UtilityData : IModuleData
    {
        public string Key() => typeof(UtilityData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public MehData MehData { get; init; }

        public PackageTrackingData PackageTrackingData { get; init; }

        public static UtilityData Clone(UtilityData other)
        {
            return new UtilityData()
            {
                TimeStamp = other?.TimeStamp,
                MehData = other?.MehData,
                PackageTrackingData = other?.PackageTrackingData
            };
        }
    }
}