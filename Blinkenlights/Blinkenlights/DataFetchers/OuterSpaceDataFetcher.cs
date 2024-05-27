using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.OuterSpace;
using NodaTime;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class OuterSpaceDataFetcher : DataFetcherBase<OuterSpaceData>
	{
		private readonly TimeSpan RetryDelta = TimeSpan.FromMinutes(30);

		public OuterSpaceDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<OuterSpaceDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

		protected override OuterSpaceData GetRemoteData(OuterSpaceData existingData = null, bool overwrite = false)
        {
            var issTrackerData = GetTrackerData(existingData?.IssTrackerData, overwrite); 
            var rocketModel = GetRocketLaunchLiveData(existingData?.RocketLaunches, overwrite);
			var peopleInSpace = GetPeopleInSpace(existingData?.PeopleInSpace, overwrite);

			return new OuterSpaceData()
            {
                IssTrackerData = issTrackerData.Result,
                RocketLaunches = rocketModel.Result,
				PeopleInSpace = peopleInSpace.Result,
                TimeStamp = DateTime.Now
            };
		}

		private async Task<PeopleInSpace> GetPeopleInSpace(PeopleInSpace existingData, bool overwrite)
		{
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.PeopleInSpace.Info()))
			{
				return existingData;
			}

			this.Logger.LogInformation($"Calling {ApiType.PeopleInSpace} remote API");
			var peopleResponse = await this.ApiHandler.Fetch(ApiType.PeopleInSpace);
			PeopleInSpaceJsonModel peopleData;
			try
			{
				peopleData = JsonSerializer.Deserialize<PeopleInSpaceJsonModel>(peopleResponse.Data);
			}
			catch (JsonException)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.PeopleInSpace, "Exception while deserializing API response");
				return PeopleInSpace.Clone(existingData, errorStatus);
			}

			if (peopleData?.People?.Any() != true)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.PeopleInSpace, "API returned no data");
				return PeopleInSpace.Clone(existingData, errorStatus);
			}

			var people = peopleData.People.Select(p => new PersonInSpace() { Name = p.Name, Craft = p.Craft }).ToList();
			var status = this.ApiStatusFactory.Success(ApiType.PeopleInSpace, DateTime.Now, ApiSource.Prod);
			return new PeopleInSpace()
			{
				Status = status,
				People = people
			};
		}

		private async Task<IssTrackerData> GetTrackerData(IssTrackerData existingData, bool overwrite)
        { 
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.IssTracker.Info()))
			{
				return existingData;
            }

			this.Logger.LogInformation($"Calling {ApiType.IssTracker} remote API");
			var apiResponse = await this.ApiHandler.Fetch(ApiType.IssTracker);

            if (apiResponse is null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.IssTracker, "API Response is null");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.IssTracker, "API Response data is empty");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
            {
                var errorResult = this.ApiStatusFactory.Failed(ApiType.IssTracker, errorMessage);
                return IssTrackerData.Clone(existingData, errorResult);
            }

            IssTrackerJsonModel trackerData;
            try
            {
                trackerData = JsonSerializer.Deserialize<IssTrackerJsonModel>(apiResponse.Data);
            }
            catch (JsonException)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.IssTracker, "Exception while deserializing API response");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            if (string.IsNullOrWhiteSpace(trackerData.ImageEncoded))
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.IssTracker, "ISS Image does not exist");
                return IssTrackerData.Clone(existingData, errorStatus);
            }

            var status = this.ApiStatusFactory.Success(ApiType.IssTracker, DateTime.Now, ApiSource.Prod);
            return new IssTrackerData()
            {
                Status = status,
                Latitude = trackerData.Latitude,
                Longitude = trackerData.Longitude,
				ImageData = trackerData.ImageEncoded
            };
        }

		private async Task<RocketLaunches> GetRocketLaunchLiveData(RocketLaunches existingData, bool overwrite)
		{
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.RocketLaunchLive.Info()))
			{
				return existingData;
			}

			this.Logger.LogInformation($"Calling {ApiType.RocketLaunchLive} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.RocketLaunchLive);
			RocketLaunchLiveJsonModel rocketsJsonModel;
			try
			{
				rocketsJsonModel = JsonSerializer.Deserialize<RocketLaunchLiveJsonModel>(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunchLive, "Exception while deserializing API response");
				return RocketLaunches.Clone(existingData, errorStatus);
			}

			if (rocketsJsonModel == null || rocketsJsonModel.Result == null || rocketsJsonModel.Result.Count == 0)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunchLive, "API response was empty", null, DateTime.Now.Add(RetryDelta));
				return RocketLaunches.Clone(existingData, errorStatus);
			}

			var launches = new List<RocketLaunch>();

			foreach(var launchJsonModel in rocketsJsonModel.Result)
			{
				if (TryCreateLaunch(launchJsonModel, out RocketLaunch launch))
				{
					launches.Add(launch);
				}
			}

			if (!launches.Any())
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunchLive, "Failed to create any launches from API response", null, DateTime.Now.Add(RetryDelta));
				return RocketLaunches.Clone(existingData, errorStatus);
			}

			var status = this.ApiStatusFactory.Success(ApiType.RocketLaunchLive, DateTime.Now, ApiSource.Prod);
			return new RocketLaunches()
			{
				Status = status,
				Launches = launches
			};
		}

		private bool TryCreateLaunch(Result launchJsonModel, out RocketLaunch launch)
		{
			if (launchJsonModel == null || string.IsNullOrWhiteSpace(launchJsonModel.LaunchDescription))
			{
				launch = default;
				return false;
			}

			var launchTime = string.Empty;
			if (DateTime.TryParse(launchJsonModel.T0, out var t0datetime))
			{
				Instant now = SystemClock.Instance.GetCurrentInstant();
				var tz = DateTimeZoneProviders.Tzdb["America/Los_Angeles"];
				var utcOffset = tz.GetUtcOffset(now).Seconds;
				t0datetime.AddSeconds(utcOffset);
				launchTime = t0datetime.ToString("dd MMMM hh:mm tt");
			}
			else
			{
				this.Logger.LogWarning($"{ApiType.RocketLaunchLive} failed to parse datetime for t0: {launchJsonModel.T0}");
			}

			launch = new RocketLaunch()
			{
				Title = launchJsonModel.LaunchDescription,
				Url = "https://www.spacelaunchschedule.com/",
				LaunchTime = launchTime,
				Provider = launchJsonModel.Provider?.Name ?? string.Empty,
				Name = launchJsonModel.Name,
				Location = launchJsonModel.Pad?.Location?.Name ?? string.Empty,
			};

			return true;
		}
	}
}
