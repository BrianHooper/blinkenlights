using Blinkenlights.Data.LiteDb;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;
using Blinkenlights.Transformers;
using LiteDbLibrary;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
    public class CalendarTransformer : TransformerBase
    {
		public CalendarTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var response = this.ApiHandler.Fetch(ApiType.GoogleCalendar).Result;

			if (response is null)
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "API Response is null");
                return new CalendarViewModel(null, status);
            }

            if (string.IsNullOrWhiteSpace(response.Data))
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "API Response data is empty", response.LastUpdateTime);
                return new CalendarViewModel(null, status);
            }

            if (ApiError.IsApiError(response.Data, out var errorMessage))
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), errorMessage, response.LastUpdateTime);
            }

            CalendarData calendarData;
            try
            {
                calendarData = JsonConvert.DeserializeObject<CalendarData>(response.Data);
            }
            catch (JsonException)
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "Exception while deserializing API response", response.LastUpdateTime);
                return new CalendarViewModel(null, status);
            }

            // TODO Dates/Times should be properly formatted
            var events = calendarData?.Events?.Where(e => e?.IsValid() == true)?.ToList();
            
            if (events?.Any() == true)
            {
                var status = ApiStatus.Success(ApiType.GoogleCalendar.ToString(), response.LastUpdateTime, response.ApiSource);
                this.ApiHandler.TryUpdateCache(response);
                return new CalendarViewModel(events, status);
            }
            else
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "Events list is empty", response.LastUpdateTime);
                return new CalendarViewModel(null, status);
            }
        }
	}
}
