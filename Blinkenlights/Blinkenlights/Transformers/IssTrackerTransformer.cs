using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.IssTracker;

namespace Blinkenlights.Transformers
{
    public class IssTrackerTransformer : TransformerBase
    {
        private IDataFetcher<IssTrackerData> dataFetcher { get; init; }

        public IssTrackerTransformer(IApiHandler apiHandler, IDataFetcher<IssTrackerData> dataFetcher) : base(apiHandler)
        {
            this.dataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var trackerData = this.dataFetcher.GetLocalData();
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
