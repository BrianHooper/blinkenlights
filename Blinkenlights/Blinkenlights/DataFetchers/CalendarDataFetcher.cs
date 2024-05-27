using Blinkenlights.ApiHandlers;
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
        public CalendarDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<CalendarDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

		protected override CalendarModuleData GetRemoteData(CalendarModuleData existingData = null, bool overwrite = false)
        {
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.GoogleCalendar.Info()) && IsValid(existingData))
			{
				return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.GoogleCalendar} remote API");
			var apiResponse = this.ApiHandler.Fetch(ApiType.GoogleCalendar).Result;

            if (apiResponse is null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.GoogleCalendar, "API Response is null");
                return CalendarModuleData.Clone(existingData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.GoogleCalendar, "API Response data is empty");
                return CalendarModuleData.Clone(existingData, errorStatus);
            }

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
            {
                var errorResult = this.ApiStatusFactory.Failed(ApiType.GoogleCalendar, errorMessage);
                return CalendarModuleData.Clone(existingData, errorResult);
            }

            CalendarData calendarData;
            try
            {
                calendarData = JsonSerializer.Deserialize<CalendarData>(apiResponse.Data);
            }
            catch (JsonException)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.GoogleCalendar, "Exception while deserializing API response", apiResponse.LastUpdateTime);
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
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.GoogleCalendar, "Events list is empty", apiResponse.LastUpdateTime);
                return CalendarModuleData.Clone(existingData, errorStatus);
            }

            var status = this.ApiStatusFactory.Success(ApiType.GoogleCalendar, DateTime.Now, ApiSource.Prod);
            return new CalendarModuleData()
            {
                Status = status,
                TimeStamp = DateTime.Now,
                Events = events,
            };
        }

        protected bool IsValid(CalendarModuleData existingData = null)
        {
            if (existingData.Events?.Any() != true)
            {
                this.Logger.LogInformation($"GoogleCalendar data invalid, missing required data");
                return false;
            }

            return true;
        }
    }
}
