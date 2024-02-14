using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class UtilityData : IModuleData
    {
        public string Key() => typeof(UtilityData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public static UtilityData Clone(UtilityData other, ApiStatus status)
        {
            return new UtilityData()
            {
                Status = status,
                TimeStamp = other?.TimeStamp
            };
        }
    }
}