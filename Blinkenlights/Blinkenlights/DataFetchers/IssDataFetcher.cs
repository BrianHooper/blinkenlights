using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.IssTracker;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class IssDataFetcher : DataFetcherBase<IssTrackerData>
    {
        IWebHostEnvironment WebHostEnvironment;

        public IssDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, IWebHostEnvironment webHostEnvironment) : base(TimeSpan.FromMinutes(5), databaseHandler, apiHandler)
        {
            this.WebHostEnvironment = webHostEnvironment;

            this.Start();
        }

        protected override IssTrackerData GetRemoteData(IssTrackerData existingData = null)
        {
            var apiResponse = this.ApiHandler.Fetch(ApiType.IssTracker).Result;

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
                TimeStamp = DateTime.Now
            };
        }
    }
}
