using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.OuterSpace;

namespace Blinkenlights.Transformers
{
    public class OuterSpaceTransformer : TransformerBase
    {
        private IDataFetcher<OuterSpaceData> dataFetcher { get; init; }

        public OuterSpaceTransformer(IApiHandler apiHandler, IDataFetcher<OuterSpaceData> dataFetcher) : base(apiHandler)
        {
            this.dataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var outerSpaceData = this.dataFetcher.FetchRemoteData();
            if (outerSpaceData == null)
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.IssTracker, "Database lookup failed");
                return new OuterSpaceViewModel();
            }

            if (outerSpaceData.IssTrackerData == null)
            {
				//var errorStatus = this.ApiStatusFactory.Failed(ApiType.IssTracker, "Database data is empty");
				return new OuterSpaceViewModel();
			}
            var trackerData = outerSpaceData.IssTrackerData;
            var latitude = trackerData.Latitude >= 0.0 ? $"{Math.Round(trackerData.Latitude.Value, 2)}° N" : $"{Math.Round(trackerData.Latitude.Value, 2) * -1}° S";
            var longitude = trackerData.Longitude >= 0.0 ? $"{Math.Round(trackerData.Longitude.Value, 2)}° E" : $"{Math.Round(trackerData.Longitude.Value, 2) * -1}° W";
            var report = $"{latitude}, {longitude}";

            return new OuterSpaceViewModel()
            {
                ImagePath = $"data:image/png;base64, {trackerData.ImageData}",
                Report = report,
                PeopleInSpace = outerSpaceData.PeopleInSpace,
                UpcomingRocketLaunches = outerSpaceData.RocketLaunches,
                IssTrackerStatus = trackerData.Status,
            };
        }
    }
}
