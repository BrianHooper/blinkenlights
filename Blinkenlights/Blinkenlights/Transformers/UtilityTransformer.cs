﻿using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Utility;
using LiteDbLibrary;
using LiteDbLibrary.Schemas;
using Newtonsoft.Json;
using static Google.Apis.Requests.BatchRequest;

namespace Blinkenlights.Transformers
{
    public class UtilityTransformer : TransformerBase
	{
		private IWebHostEnvironment WebHostEnvironment { get; init; }

		public UtilityTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler, IWebHostEnvironment webHostEnvironment) : base(apiHandler, liteDbHandler)
		{
			WebHostEnvironment = webHostEnvironment;
		}

		public override IModuleViewModel Transform()
		{
			return new UtilityViewModel() 
			{ 
				MehData = GetMehData(),
				PackageTrackingData = GetPackageTrackingData()
			};
        }

		private PackageTrackingViewModel GetPackageTrackingData()
		{
			this.ApiHandler.TryGetCachedValue(ApiType.Ship24, out var ship24CacheData);
			var ship24Cache = Ship24Cache.Deserialize(ship24CacheData);

			var trackings = this.LiteDb.Read<PackageTrackingItem>()
				?.Where(t => !string.IsNullOrWhiteSpace(t?.TrackingNumber));

			if (trackings?.Any() != true)
			{
				var errorStatus = ApiStatus.Failed(ApiType.Ship24, null, "No requests in database");
				return new PackageTrackingViewModel(errorStatus);
			}

			var apiRequests = new Dictionary<string, Task<ApiResponse>>();
			foreach (var tracking in trackings)
			{
				if (!ship24Cache.Responses.ContainsKey(tracking.TrackingNumber))
				{
					var apiRequestData = new Ship24Request(tracking.TrackingNumber);
					var serializedRequest = JsonConvert.SerializeObject(apiRequestData);
					apiRequests.Add(tracking.TrackingNumber, this.ApiHandler.Fetch(ApiType.Ship24, serializedRequest));
				}
			}
			
			var apiResponses = new Dictionary<string, Ship24Response>();
			var successCount = 0;
			var failedCount = 0;
			var lastUpdateTime = ship24Cache.LastUpdateTime;
			foreach (var tracking in trackings)
			{
				if (apiRequests.TryGetValue(tracking.TrackingNumber, out var apiResponse))
				{
					var apiResponseResult = apiResponse.Result;
					var ship24Response = Ship24Response.Deserialize(apiResponse.Result?.Data);
					apiResponses.Add(tracking.TrackingNumber, ship24Response);

					if (ship24Response != null) 
					{
						successCount++;
						ship24Cache.Responses.Add(tracking.TrackingNumber, ship24Response);
						lastUpdateTime = DateTime.UtcNow;
					}
					else
					{
						failedCount++;
					}
				}
				else if (ship24Cache.Responses.TryGetValue(tracking.TrackingNumber, out var cachedValue))
				{
					successCount++;
					apiResponses.Add(tracking.TrackingNumber, cachedValue);
				}
				else
				{
					failedCount++;
					apiResponses.Add(tracking.TrackingNumber, null);
				}
			}

			var cacheApiResponse = ApiResponse.Success(ApiType.Ship24, ship24Cache.Serialize(), ApiSource.Cache, lastUpdateTime);
			this.ApiHandler.TryUpdateCache(cacheApiResponse);

			var packages = apiResponses.Select(kv => Package.FromResponse(kv, trackings)).Where(p => p != null).ToList();

			var source = apiRequests.Any() ? ApiSource.Prod : ApiSource.Cache;


			var status = failedCount > 0
				? ApiStatus.Failed(ApiType.Ship24, lastUpdateTime, source, $"{successCount} success, {failedCount} failed")
				: ApiStatus.Success(ApiType.Ship24, lastUpdateTime, source);
			return new PackageTrackingViewModel(status, packages);
		}

		private MehViewModel GetMehData()
        {
			var response = this.ApiHandler.Fetch(ApiType.Meh).Result;
			if (response is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, null, "Api response is null");
				return new MehViewModel(errorStatus);
			}
			else if (string.IsNullOrWhiteSpace(response.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, response, "Api response is empty");
				return new MehViewModel(errorStatus);
			}

			MehJsonModel model;
			try
			{
				model = JsonConvert.DeserializeObject<MehJsonModel>(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, response, "Error deserializing api response");
				return new MehViewModel(errorStatus);
			}

			var title = model?.Deal?.Title;
			var item = model?.Deal?.Items?.FirstOrDefault();
			var url = model?.Deal?.Url;
			var imageUrl = model?.Deal?.Photos?.FirstOrDefault(i => Path.GetExtension(i)?.ToLower()?.Equals(".gif") != true);
			var price = item?.Price.ToString();
			if (string.IsNullOrWhiteSpace(title)
				|| string.IsNullOrWhiteSpace(url)
				|| string.IsNullOrWhiteSpace(imageUrl)
				|| string.IsNullOrWhiteSpace(price))
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, response, "Required data missing in api response");
				return new MehViewModel(errorStatus);
			}

			var status = ApiStatus.Success(ApiType.Meh, response);
			this.ApiHandler.TryUpdateCache(response);
			return new MehViewModel(status, $"Meh - ${price} - {title}", url, imageUrl);
		}
    }
}
