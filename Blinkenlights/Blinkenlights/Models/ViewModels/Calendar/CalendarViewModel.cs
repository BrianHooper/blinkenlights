using Blinkenlights.Dataschemas;
using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Calendar
{
    public class CalendarViewModel : ModuleViewModelBase
    {
		public CalendarViewModel() : base("Calendar")
		{
		}

		public CalendarViewModel(List<CalendarModuleEvent> events, ApiStatus status) : base("Calendar", status)
        {
            Events = events;
        }

        public List<CalendarModuleEvent> Events { get; set; }
    }

    public class CalendarData
    {
        [JsonPropertyName("Events")]
        public List<Event> Events { get; set; }
    }

    public class Event
    {
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("Date")]
        public string Date { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Date);
        }
    }
}
