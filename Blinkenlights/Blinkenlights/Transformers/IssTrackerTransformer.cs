using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.IssTracker;
using LiteDbLibrary;

namespace Blinkenlights.Transformers
{
	public class IssTrackerTransformer : TransformerBase
	{
		private IIssDataFetcher dataFetcher { get; init; }

		public IssTrackerTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler, IIssDataFetcher dataFetcher) : base(apiHandler, liteDbHandler)
		{
			this.dataFetcher = dataFetcher;
		}

		public override IModuleViewModel Transform()
		{
			var trackerData = this.dataFetcher.GetData();
			if (trackerData == null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.IssTracker.ToString(), "Database lookup failed");
				return new IssTrackerViewModel(errorStatus);
			}

			var latitude = trackerData.Latitude >= 0.0 ? $"{Math.Round(trackerData.Latitude.Value, 2)}° N" : $"{Math.Round(trackerData.Latitude.Value, 2) * -1}° S";
			var longitude = trackerData.Longitude >= 0.0 ? $"{Math.Round(trackerData.Longitude.Value, 2)}° E" : $"{Math.Round(trackerData.Longitude.Value, 2) * -1}° W";
			var report = $"{latitude}, {longitude}";

			return new IssTrackerViewModel
			(
				imagePath: trackerData.FilePath,
				report: report,
				status: ApiStatus.Success(ApiType.IssTracker.ToString(), trackerData.TimeStamp, ApiSource.Prod)
			);
		}
	}
}
