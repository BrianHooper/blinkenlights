using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.FlightStatus;
using Blinkenlights.Models.ViewModels.OuterSpace;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class FlightStatusDataFetcher : DataFetcherBase<FlightStatusData>
    {
		private readonly TimeSpan RetryDelta = TimeSpan.FromMinutes(30);

		// 45.67 -122.50 47.30 -122.08
		private const double mapMinLat = 45.67;
		private const double mapMinLon = -122.50;
		private const double mapMaxLat = 47.30;
		private const double mapMaxLon = -122.08;

		public FlightStatusDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<FlightStatusDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

        public override FlightStatusData GetRemoteData(FlightStatusData existingData = null, bool overwrite = false)
        {
			return GetFlightStatusData(existingData, overwrite).Result;
		}

		private async Task<FlightStatusData> GetFlightStatusData(FlightStatusData existingData = null, bool overwrite = false)
		{
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.FlightAware.Info()))
			{
				return existingData;
			}

			if (existingData?.Status?.NextValidRequestTime != null && DateTime.Now < existingData.Status.NextValidRequestTime)
			{
				this.Logger.LogWarning($"Waiting for rate limit for {ApiType.FlightAware} API, next valid request: {existingData.Status.NextValidRequestTime}");
				return existingData;
			}

			this.Logger.LogInformation($"Calling {ApiType.FlightAware} remote API");
			var apiResponse = await this.ApiHandler.Fetch(ApiType.FlightAware);

			if (apiResponse is null)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightAware, "API Response is null", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightAware, "API Response data is empty", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			FlightAwareJsonModel flightStatusData;
			try
			{
				flightStatusData = JsonSerializer.Deserialize<FlightAwareJsonModel>(apiResponse.Data);
			}
			catch (JsonException)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightAware, "Exception while deserializing API response", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			if (flightStatusData?.Flights?.Any() != true)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightAware, "Response was empty", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			var filteredFlights = flightStatusData?.Flights
				?.Select(f => f.LastPosition)
				?.Where(f => f != null && f.Latitude > mapMinLat && f.Latitude < mapMaxLat && f.Longitude > mapMinLon && f.Longitude < mapMaxLon)
				?.Take(6);

			if (filteredFlights?.Any() != true)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightAware, "No flights in map range");
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			return new FlightStatusData()
			{
				Status = this.ApiStatusFactory.Success(ApiType.FlightAware, DateTime.Now, ApiSource.Prod),
				TimeStamp = DateTime.Now,
			};
		}
    }
}
