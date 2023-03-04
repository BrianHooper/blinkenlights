using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;
using Blinkenlights.Transformers;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
    public class CalendarTransformer : TransformerBase
    {
		public CalendarTransformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public async override Task<IModuleViewModel> Transform()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.GoogleCalendar);

			if (apiResponse is null)
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar, null, "API Response is null");
                return new CalendarViewModel(null, status);
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar, apiResponse, "API Response data is empty");
                return new CalendarViewModel(null, status);
            }

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar, apiResponse, errorMessage);
            }

            CalendarData calendarData;
            try
            {
                calendarData = JsonConvert.DeserializeObject<CalendarData>(apiResponse.Data);
            }
            catch (JsonException)
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar, apiResponse, "Exception while deserializing API response");
                return new CalendarViewModel(null, status);
            }

            // TODO Dates/Times should be properly formatted
            var events = calendarData?.Events?.Where(e => e?.IsValid() == true)?.ToList();
            
            if (events?.Any() == true)
            {
                var status = ApiStatus.Success(ApiType.GoogleCalendar, apiResponse);
                this.ApiHandler.TryUpdateCache(apiResponse);
                return new CalendarViewModel(events, status);
            }
            else
            {
                var status = ApiStatus.Failed(ApiType.GoogleCalendar, apiResponse, "Events list is empty");
                return new CalendarViewModel(null, status);
            }
        }
	}
}
