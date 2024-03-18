using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.FlightStatus;
using Blinkenlights.Models.ViewModels.OuterSpace;
using System.Security.AccessControl;
using System.Text.Json;
using static Humanizer.In;

namespace Blinkenlights.DataFetchers
{
    public class FlightStatusDataFetcher : DataFetcherBase<FlightStatusData>
    {
		private readonly TimeSpan RetryDelta = TimeSpan.FromMinutes(5);

		// 45.67 -122.50 47.30 -122.08
		private const double mapMinLat = 46.80;
		private const double mapMinLon = -121.30;
		private const double mapMaxLat = 48.00;
		private const double mapMaxLon = -123.30;

		private readonly double mapWidthGps = mapMaxLat - mapMinLat;
		private readonly double mapHeightGps = mapMaxLon - mapMinLon;

		private readonly string[] colors = {
			"#003f5c",
			"#2f4b7c",
			"#665191",
			"#a05195",
			"#d45087",
			"#f95d6a",
			"#ff7c43",
			"#ffa600",
		};

		public FlightStatusDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<FlightStatusDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

        protected override FlightStatusData GetRemoteData(FlightStatusData existingData = null, bool overwrite = false)
		{
			return GetFlightStatusData(existingData, overwrite).Result;
		}

		private async Task<FlightStatusData> GetFlightStatusData(FlightStatusData existingData = null, bool overwrite = false)
		{
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.FlightRadar.Info()))
			{
				return existingData;
			}

			//if (existingData?.Status?.NextValidRequestTime != null && DateTime.Now < existingData.Status.NextValidRequestTime)
			//{
			//	this.Logger.LogWarning($"Waiting for rate limit for {ApiType.FlightRadar} API, next valid request: {existingData.Status.NextValidRequestTime}");
			//	return existingData;
			//}

			this.Logger.LogInformation($"Calling {ApiType.FlightRadar} remote API");
			var apiResponse = await this.ApiHandler.Fetch(ApiType.FlightRadar);

			if (apiResponse is null)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightRadar, "API Response is null", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightRadar, "API Response data is empty", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

            if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightRadar, errorMessage);
                return FlightStatusData.Clone(existingData, errorStatus);
            }

            FlightRadarJsonModel flightStatusData;
			try
			{
				flightStatusData = JsonSerializer.Deserialize<FlightRadarJsonModel>(apiResponse.Data);
			}
			catch (JsonException)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightRadar, "Exception while deserializing API response", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			if (flightStatusData?.Flights?.Any() != true)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightRadar, "Response was empty", null, DateTime.Now.Add(RetryDelta));
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			var filteredFlights = flightStatusData?.Flights
				?.Where(f => f != null
				&& f.Latitude > mapMinLat
				&& f.Latitude < mapMaxLat
				&& f.Longitude < mapMinLon
				&& f.Longitude > mapMaxLon
				&& (string.Equals("SEA", f.Origin, StringComparison.OrdinalIgnoreCase) || string.Equals("SEA", f.Destination, StringComparison.OrdinalIgnoreCase)));

			var flights = new List<FlightData>();
			var colorIdx = 0;
			foreach(var flightJsonModel in filteredFlights)
			{
				if (colorIdx >= colors.Length)
				{
					colorIdx = 0;
				}
				var flightData = CreateFlightData(flightJsonModel, colorIdx);
				if (flightData != null)
                {
                    colorIdx++;
                    flights.Add(flightData);
                }
			}

            if (flights?.Any() != true)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.FlightRadar, "No flights in map range");
				return FlightStatusData.Clone(existingData, errorStatus);
			}

			//var additionalFlightData = new Dictionary<FlightData, Task<SingleFlightData>>();
			//foreach(var flight in flightResults)
			//{
			//	additionalFlightData.Add(flight, GetFlightData(flight.Fid));
   //         }

			//foreach(var kvp in additionalFlightData)
			//{
			//	var response = kvp.Value.Result;
			//	if (response != null)
			//	{
			//		kvp.Key.SingleFlightData = response;
			//	}
			//}

			return new FlightStatusData()
			{
				Status = this.ApiStatusFactory.Success(ApiType.FlightRadar, DateTime.Now, ApiSource.Prod),
				TimeStamp = DateTime.Now,
				Flights = flights,
			};
		}

		public async Task<string> GetFlightData(string fid)
		{
			var response = await this.ApiHandler.Fetch(ApiType.FlightRadar, null, fid);
			return response?.Data;
        }

            private FlightData CreateFlightData(Flight flightJsonModel, int colorIdx)
		{
			var flightData = new FlightData();
			if (flightJsonModel == null || flightJsonModel.Latitude == null || flightJsonModel.Longitude == null)
			{
				return null;
			}

			flightData.Fid = flightJsonModel.Fid;
			flightData.Color = colors[colorIdx];
			flightData.AircraftType = flightJsonModel.Aircraft;
			flightData.FlightCode = flightJsonModel.Name;
			flightData.SourceAirport = flightJsonModel.Origin;
			flightData.DestAirport = flightJsonModel.Destination;

			flightData.Latitude = flightJsonModel.Latitude.Value;
			flightData.Longitude = flightJsonModel.Longitude.Value;
			flightData.Heading = flightJsonModel.Heading == null ? 0 : flightJsonModel.Heading.Value;

			flightData.MapTopPercentage = 100 - (int) (100 * ((flightData.Latitude - mapMinLat) / mapWidthGps) - 1);

			flightData.MapLeftPercentage = 100 - (int) (100 * ((flightData.Longitude - mapMinLon) / mapHeightGps) - 2);
			
			if (flightData.MapTopPercentage <= 5 || flightData.MapTopPercentage >= 95 || flightData.MapLeftPercentage <= 5 || flightData.MapLeftPercentage >= 95)
			{
				return null;
			}


			return flightData;
		}
	}
}
