using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.OuterSpace;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class OuterSpaceDataFetcher : DataFetcherBase<OuterSpaceData>
    {
        IWebHostEnvironment WebHostEnvironment;

        public OuterSpaceDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, IWebHostEnvironment webHostEnvironment, ILogger<OuterSpaceDataFetcher> logger) : base(databaseHandler, apiHandler, logger)
        {
            this.WebHostEnvironment = webHostEnvironment;
            this.Logger = logger;
            this.Start();
        }

        public override OuterSpaceData GetRemoteData(OuterSpaceData existingData = null, bool overwrite = false)
        {
            var issTrackerData = GetTrackerData(existingData?.IssTrackerData, overwrite);
            return new OuterSpaceData()
            {
                IssTrackerData = issTrackerData.Result
            };
        }

        private async Task<IssTrackerData> GetTrackerData(IssTrackerData existingData, bool overwrite)
        { 
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.IssTracker.Info()))
			{
				this.Logger.LogDebug($"Using cached data for {ApiType.IssTracker} API");
				return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.IssTracker} remote API");
			var apiResponse = this.ApiHandler.Fetch(ApiType.IssTracker).Result;
            //var peopleResponse = this.ApiHandler.Fetch(ApiType.PeopleInSpace).Result;

            //PeopleInSpaceJsonModel people;
            //try
            //{
            //    people = JsonSerializer.Deserialize<PeopleInSpaceJsonModel>(peopleResponse.Data);
            //}
            //catch (JsonException)
            //{
            //}

            if (apiResponse is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "API Response is null");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "API Response data is empty");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
            {
                var errorResult = ApiStatus.Failed(ApiType.IssTracker.ToString(), errorMessage);
                return IssTrackerData.Clone(existingData, errorResult);
            }

            IssTrackerJsonModel trackerData;
            try
            {
                trackerData = JsonSerializer.Deserialize<IssTrackerJsonModel>(apiResponse.Data);
            }
            catch (JsonException)
            {
                var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "Exception while deserializing API response");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(trackerData?.ImagePath) || !File.Exists(trackerData.ImagePath))
            {
                var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "ISS Image does not exist on disk");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            var filename = Path.GetFileName(trackerData?.ImagePath);
            var relativePath = Path.Combine("images", filename);
            var destination = Path.Combine(this.WebHostEnvironment.WebRootPath, "images", filename);

            try
            {
                File.Copy(trackerData.ImagePath, destination, true);
            }
            catch (Exception ex)
            {
                var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), ex.Message);
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            var status = ApiStatus.Success(ApiType.IssTracker.ToString(), DateTime.Now, ApiSource.Prod);
            return new IssTrackerData()
            {
                Status = status,
                FilePath = relativePath,
                Latitude = trackerData.Latitude,
                Longitude = trackerData.Longitude,
            };
        }
    }
}
