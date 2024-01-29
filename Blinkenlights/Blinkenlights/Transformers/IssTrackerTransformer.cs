using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.IssTracker;
using Blinkenlights.Models.ViewModels.Slideshow;
using LiteDbLibrary;
using Newtonsoft.Json;
using System;

namespace Blinkenlights.Transformers
{
	public class IssTrackerTransformer : TransformerBase
	{
		private IWebHostEnvironment WebHostEnvironment { get; init; }

		public IssTrackerTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler, IWebHostEnvironment webHostEnvironment) : base(apiHandler, liteDbHandler)
		{
			this.WebHostEnvironment = webHostEnvironment;
		}

		public override IModuleViewModel Transform()
		{
			var trackerResponse = GetIssTrackerImage().Result;
			var trackerData = trackerResponse.trackerData;
			if (trackerData == null)
			{
				return new IssTrackerViewModel(trackerResponse.Status);
			}

			var latitude = trackerData.Latitude >= 0.0 ? $"{Math.Round(trackerData.Latitude, 2)}° N" : $"{Math.Round(trackerData.Latitude, 2) * -1}° S";
			var longitude = trackerData.Longitude >= 0.0 ? $"{Math.Round(trackerData.Longitude, 2)}° E" : $"{Math.Round(trackerData.Longitude, 2) * -1}° W";
			var report = $"{trackerData.LastUpdateTime} - {latitude}, {longitude}";

			return new IssTrackerViewModel
			(
				imagePath: trackerData.ImagePath,
				report: report,
				status: trackerResponse.Status
			);
		}

		public async Task<(IssTrackerJsonModel trackerData, ApiStatus Status)> GetIssTrackerImage()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.IssTracker);
			if (apiResponse is null)
			{
				return (null, ApiStatus.Failed(ApiType.IssTracker, null, "API Response is null"));
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				return (null, ApiStatus.Failed(ApiType.IssTracker, apiResponse, "API Response data is empty"));
			}

			if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
			{
				return (null, ApiStatus.Failed(ApiType.IssTracker, apiResponse, errorMessage));
			}

			IssTrackerJsonModel trackerData;
			try
			{
				trackerData = JsonConvert.DeserializeObject<IssTrackerJsonModel>(apiResponse.Data);
			}
			catch (JsonException)
			{
				return (null, ApiStatus.Failed(ApiType.IssTracker, apiResponse, "Exception while deserializing API response"));
			}

			if (string.IsNullOrWhiteSpace(trackerData?.ImagePath))
			{
				return (null, ApiStatus.Failed(ApiType.IssTracker, apiResponse, "Missing required data from api"));
			}

			if (!File.Exists(trackerData?.ImagePath))
			{
				return (null, ApiStatus.Failed(ApiType.IssTracker, apiResponse, "Image file not found"));
			}

			var filename = Path.GetFileName(trackerData?.ImagePath);
			var relativePath = Path.Combine("images", filename);
			var destination = Path.Combine(this.WebHostEnvironment.WebRootPath, "images", filename);

			try
			{
				File.Copy(trackerData.ImagePath, destination, true);
			}
			catch(Exception ex)
			{
				return (null, ApiStatus.Failed(ApiType.IssTracker, apiResponse, ex.Message));
			}
			trackerData.ImagePath = relativePath;

			var status = ApiStatus.Success(ApiType.IssTracker, apiResponse);
			return (trackerData, status);
		}
	}
}
