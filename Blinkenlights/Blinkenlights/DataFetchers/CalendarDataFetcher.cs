using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.Calendar;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class CalendarDataFetcher : DataFetcherBase<CalendarModuleData>
    {
        public CalendarDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromMinutes(60), databaseHandler, apiHandler)
        {
            this.Start();
        }

        protected override CalendarModuleData GetRemoteData(CalendarModuleData existingData = null)
        {
            var apiResponse = this.ApiHandler.Fetch(ApiType.GoogleCalendar).Result;

            if (apiResponse is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "API Response is null");
                return CalendarModuleData.Clone(existingData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                var errorStatus = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "API Response data is empty");
                return CalendarModuleData.Clone(existingData, errorStatus);
            }

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
            {
                var errorResult = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), errorMessage);
                return CalendarModuleData.Clone(existingData, errorResult);
            }

            CalendarData calendarData;
            try
            {
                calendarData = JsonSerializer.Deserialize<CalendarData>(apiResponse.Data);
            }
            catch (JsonException)
            {
                var errorStatus = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "Exception while deserializing API response", apiResponse.LastUpdateTime);
                return CalendarModuleData.Clone(existingData, errorStatus);
            }

            // TODO Dates/Times should be properly formatted
            var events = calendarData?.Events
                ?.Where(e => e?.IsValid() == true)
                ?.Select(e => new CalendarModuleEvent()
                {
                    Name = e.Name,
                    Date = e.Date,
                }).ToList();

            if (events?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed(ApiType.GoogleCalendar.ToString(), "Events list is empty", apiResponse.LastUpdateTime);
                return CalendarModuleData.Clone(existingData, errorStatus);
            }

            var status = ApiStatus.Success(ApiType.IssTracker.ToString(), DateTime.Now, ApiSource.Prod);
            return new CalendarModuleData()
            {
                Status = status,
                TimeStamp = DateTime.Now,
                Events = events,
            };
        }

        protected override bool IsValid(CalendarModuleData existingData = null)
        {
            if (existingData?.Events?.Any() != true || existingData.Status == null)
            {
                return false;
            }

            return !existingData.Status.Expired(TimeSpan.FromHours(4));
        }
    }
}
