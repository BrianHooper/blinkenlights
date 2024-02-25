using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.Life360;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class Life360DataFetcher : DataFetcherBase<Life360Data>
    {
        public Life360DataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<Life360DataFetcher> logger) : base(databaseHandler, apiHandler, logger)
        {
            Start();
        }

        public override Life360Data GetRemoteData(Life360Data existingData = null, bool overwrite = false)
        {
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.Life360.Info()) && IsValid(existingData))
			{
				this.Logger.LogDebug($"Using cached data for {ApiType.Life360} API");
				return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.Life360} remote API");
			var response = this.ApiHandler.Fetch(ApiType.Life360).Result;
            if (response == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), "Api response is null");
                return Life360Data.Clone(existingData, errorStatus);
            }
            else if (response.ResultStatus != ApiResultStatus.Success)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), response.StatusMessage, response.LastUpdateTime);
                return Life360Data.Clone(existingData, errorStatus);
            }

            Life360JsonModel serverModel;
            try
            {
                serverModel = Life360JsonModel.FromJson(response.Data);
            }
            catch (JsonException e)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), $"Exception while deserializing API response: {e.Message}");
                return Life360Data.Clone(existingData, errorStatus);
            }

            var models = serverModel?.Members?.Select(m => Parse(m))?.Where(m => m != null)?.ToList();
            if (models?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), "Models list was empty", response.LastUpdateTime);
                return Life360Data.Clone(existingData, errorStatus);
            }

            var locA = models.ElementAtOrDefault(0);
            var locB = models.ElementAtOrDefault(1);
            var distance = FetchDistance(existingData?.DistanceData, locA, locB);

            var status = ApiStatus.Success(ApiType.Life360.ToString(), response.LastUpdateTime, response.ApiSource);
            return new Life360Data()
            {
                Status = status,
                TimeStamp = DateTime.Now,
                Locations = models,
                DistanceData = distance,
            };
        }

        private Life360DistanceData FetchDistance(Life360DistanceData existingData, Life360LocationData locA, Life360LocationData locB)
        {
            if (locA == null || locB == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Distance.ToString(), $"Insufficient data to call api");
                return Life360DistanceData.Clone(existingData, errorStatus);
            }

			this.Logger.LogInformation($"Calling {ApiType.Distance} remote API");
			var response = this.ApiHandler.Fetch(ApiType.Distance, "", locA.Latitude.ToString(), locA.Longitude.ToString(), locB.Latitude.ToString(), locB.Longitude.ToString()).Result;
            if (response == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Distance.ToString(), $"API response was null");
                return Life360DistanceData.Clone(existingData, errorStatus);
            }

            DistanceJsonModel serverModel;
            try
            {
                serverModel = JsonSerializer.Deserialize<DistanceJsonModel>(response.Data);
            }
            catch (JsonException e)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Distance.ToString(), $"Exception while deserializing API response: {e.Message}");
                return Life360DistanceData.Clone(existingData, errorStatus);
            }

            var timeDelta = locA.TimeDeltaSeconds < locB.TimeDeltaSeconds ? locA.TimeDeltaStr : locB.TimeDeltaStr;

            var status = ApiStatus.Success(ApiType.Distance.ToString(), DateTime.Now, ApiSource.Prod);
            return new Life360DistanceData()
            {
                Distance = serverModel.distance.ToString(),
                TimeDelta = timeDelta,
                Status = status,
            };
        }

        private static Life360LocationData Parse(Member member)
        {
            return Life360LocationData.Parse(member?.FirstName, member?.Location?.Timestamp?.ToString(), member?.Location?.Latitude?.ToString(), member?.Location?.Longitude?.ToString());
        }

        protected bool IsValid(Life360Data existingData = null)
        {
            var apiInfo = ApiType.Life360.Info();
            if (existingData?.Status?.LastUpdate == null)
            {
                this.Logger.LogInformation($"Life360 data invalid, previous data is null or empty");
                return false;
            }

            var lastUpdateDelta = DateTime.Now - existingData.Status.LastUpdate;
            if (lastUpdateDelta > apiInfo.Timeout)
            {
                this.Logger.LogInformation($"Life360 data invalid, previous data is expired");
                return false;
            }

            if (existingData.Locations?.Any() != true)
            {
                this.Logger.LogInformation($"Life360 data invalid, missing required data");
                return false;
            }

            return true;
        }
    }
}
