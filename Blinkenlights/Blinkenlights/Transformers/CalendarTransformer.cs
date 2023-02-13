using Blinkenlights.Models.ApiResult;
using Blinkenlights.Models.Calendar;
using BlinkenLights.Models.ApiCache;
using Newtonsoft.Json;

namespace BlinkenLights.Transformers
{
    public class CalendarTransformer
    {
        private const ApiType apiType = ApiType.GoogleCalendar;


        public static CalendarViewModel GetCalendarViewModel(ApiResponse apiResponse)
        {
            if (apiResponse is null)
            {
                var status = ApiStatus.Failed(apiType, null, "API Response is null");
                return new CalendarViewModel(null, status);
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                var status = ApiStatus.Failed(apiType, apiResponse, "API Response data is empty");
                return new CalendarViewModel(null, status);
            }

            CalendarData calendarData;
            try
            {
                calendarData = JsonConvert.DeserializeObject<CalendarData>(apiResponse.Data);
            }
            catch (JsonException)
            {
                var status = ApiStatus.Failed(apiType, apiResponse, "Exception while deserializing API response");
                return new CalendarViewModel(null, status);
            }

            // TODO Dates/Times should be properly formatted
            var events = calendarData?.Events?.Where(e => e?.IsValid() == true)?.Take(20)?.ToList();
            
            if (events?.Any() == true)
            {
                var status = ApiStatus.Success(apiType, apiResponse);
                return new CalendarViewModel(events, status);
            }
            else
            {
                var status = ApiStatus.Failed(apiType, apiResponse, "Events list is empty");
                return new CalendarViewModel(null, status);
            }
        }
    }
}
