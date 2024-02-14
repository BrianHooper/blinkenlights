using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Calendar
{
    public class CalendarViewModel : ApiResultBase
    {
        public CalendarViewModel(List<CalendarModuleEvent> events, ApiStatus status) : base("Calendar", status)
        {
            Events = events;
        }

        public List<CalendarModuleEvent> Events { get; set; }
    }

    public class CalendarData
    {
        [JsonProperty("Events")]
        public List<Event> Events { get; set; }
    }

    public class Event
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Date")]
        public string Date { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && !string.IsNullOrEmpty(Date);
        }
    }
}
