using Blinkenlights.ApiHandlers;
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
        public Life360DataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<Life360DataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
		{
        }

		protected override Life360Data GetRemoteData(Life360Data existingData = null, bool overwrite = false)
        {
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.Life360.Info()) && IsValid(existingData))
			{
				return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.Life360} remote API");
			var response = this.ApiHandler.Fetch(ApiType.Life360).Result;
            if (response == null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Life360, "Api response is null");
                return Life360Data.Clone(existingData, errorStatus);
            }
            else if (response.ResultStatus != ApiResultStatus.Success)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Life360, response.StatusMessage, response.LastUpdateTime);
                return Life360Data.Clone(existingData, errorStatus);
            }

            Life360JsonModel serverModel;
            try
            {
                serverModel = JsonSerializer.Deserialize<Life360JsonModel>(response.Data);
            }
            catch (JsonException e)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Life360, $"Exception while deserializing API response: {e.Message}");
                return Life360Data.Clone(existingData, errorStatus);
            }

            var models = serverModel?.Members?.Select(m => Parse(m))?.Where(m => m != null)?.ToList();
            if (models?.Any() != true)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.Life360, "Models list was empty", response.LastUpdateTime);
                return Life360Data.Clone(existingData, errorStatus);
            }

            var locA = models.ElementAtOrDefault(0);
            var locB = models.ElementAtOrDefault(1);
            var distance = CalculateDistance(existingData?.DistanceData, locA, locB);

            var status = this.ApiStatusFactory.Success(ApiType.Life360, response.LastUpdateTime, response.ApiSource);
            return new Life360Data()
            {
                Status = status,
                TimeStamp = DateTime.Now,
                Locations = models,
                DistanceData = distance,
            };
        }

        private Life360DistanceData CalculateDistance(Life360DistanceData existingData, Life360LocationData locA, Life360LocationData locB)
        {
            if (locA == null || locB == null)
            {
                return existingData;
            }

            var distanceKm = Haversine(locA.Latitude, locB.Latitude, locA.Longitude, locB.Longitude);
            var distance = (distanceKm / 1.609344).ToString("0.##");
            var timeDeltaSeconds = locA.TimeDeltaSeconds < locB.TimeDeltaSeconds ? locA.TimeDeltaSeconds : locB.TimeDeltaSeconds;

            string time;
            if (string.Equals(locA.Id, "5fc014c9-645c-4d0a-9dc4-205293ab2ba3", StringComparison.OrdinalIgnoreCase))
            {
                time = locA.ToString();
            }
            else
            {
                time = locB.Time.ToString();
            }

            return new Life360DistanceData()
            {
                Distance = distance.ToString(),
                TimeDelta = timeDeltaSeconds,
                Time = time,
            };
        }

        private static double Haversine(double lat1, double lat2, double lon1, double lon2)
        {
            double R = 6371; // Earth radius in kilometers

            // Convert latitude and longitude to radians
            double lat1Rad = lat1 * Math.PI / 180;
            double lat2Rad = lat2 * Math.PI / 180;
            double lon1Rad = lon1 * Math.PI / 180;
            double lon2Rad = lon2 * Math.PI / 180;

            // Calculate differences
            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            // Calculate flat earth distance
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;

            return distance;
        }

        private static Life360LocationData Parse(Member member)
        {
            return Life360LocationData.Parse(member?.FirstName, member?.Location?.Timestamp?.ToString(), member?.Location?.Latitude?.ToString(), member?.Location?.Longitude?.ToString(), member?.Id);
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
