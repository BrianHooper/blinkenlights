using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class CalendarModuleData : IModuleData
    {
        public string Key() => typeof(CalendarModuleData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public List<CalendarModuleEvent> Events { get; init; }

        public static CalendarModuleData Clone(CalendarModuleData other, ApiStatus status)
        {
            return new CalendarModuleData()
            {
                Status = status,
                TimeStamp = other?.TimeStamp,
                Events = other?.Events
            };
        }
    }
}
