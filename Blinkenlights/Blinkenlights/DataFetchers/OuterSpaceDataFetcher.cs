using Blinkenlights.ApiHandlers;
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

		public OuterSpaceDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, IWebHostEnvironment webHostEnvironment, ILogger<OuterSpaceDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
            this.WebHostEnvironment = webHostEnvironment;
            this.Start();
        }

        public override OuterSpaceData GetRemoteData(OuterSpaceData existingData = null, bool overwrite = false)
        {
            var issTrackerData = GetTrackerData(existingData?.IssTrackerData, overwrite); 
            var rocketModel = ProcessPageParseApiResponse(existingData?.RocketLaunches, overwrite);
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

		public async Task<RocketLaunches> ProcessPageParseApiResponse(RocketLaunches existingData, bool overwrite)
		{
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.RocketLaunches.Info()))
			{
				return existingData;
			}

			this.Logger.LogInformation($"Calling {ApiType.RocketLaunches} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.RocketLaunches);
			if (response is null)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunches, "Api response is null", response.LastUpdateTime);
				return RocketLaunches.Clone(existingData, errorStatus);
			}

			if (ApiError.IsApiError(response.Data, out var errorMessage))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunches, errorMessage, response.LastUpdateTime);
				return RocketLaunches.Clone(existingData, errorStatus);
			}

			if (string.IsNullOrWhiteSpace(response?.Data))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunches, "Response data is null", response.LastUpdateTime);
				return RocketLaunches.Clone(existingData, errorStatus);
			}

			PageParseApiModel model;
			try
			{
				model = JsonSerializer.Deserialize<PageParseApiModel>(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunches, "Exception deserializing response", response.LastUpdateTime);
				return RocketLaunches.Clone(existingData, errorStatus);
			}

			var launches = model?.stories?.Select(a => new RocketLaunch() { Title = a.title, Url = a.url })?.Take(6)?.ToList();
			if (launches?.Any() == true)
			{
				var status = this.ApiStatusFactory.Success(ApiType.RocketLaunches, response.LastUpdateTime, response.ApiSource);
				var rocketLaunches = new RocketLaunches()
				{
					Launches = launches,
					Status = status,
				};

				return rocketLaunches;
			}
			else
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.RocketLaunches, "No headlines created", response.LastUpdateTime);
				return RocketLaunches.Clone(existingData, errorStatus);
			}
		}
	}
}
