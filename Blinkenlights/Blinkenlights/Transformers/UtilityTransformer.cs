using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Utility;

namespace Blinkenlights.Transformers
{
    public class UtilityTransformer : TransformerBase
	{
		private IDataFetcher<UtilityData> DataFetcher { get; init; }

		private IDataFetcher<Life360Data> Life360DataFetcher { get; init; }

        public UtilityTransformer(IApiHandler apiHandler, IDataFetcher<UtilityData> dataFetcher, IDataFetcher<Life360Data> life360DataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
            this.Life360DataFetcher = life360DataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.FetchRemoteData();
            if (response == null)
            {
                //var errorStatus = ApiStatus.Failed("Utility", "Database lookup failed");
                return new UtilityViewModel();
            }

            var life360Data = GetLife360Data(out var life360Status);

            return new UtilityViewModel(response.MehData?.ApiStatus, life360Status)
            {
                MehData = response.MehData,
                PackageTrackingData = response.PackageTrackingData,
                Life360Data = life360Data,
            };
        }

        private Life360UtilityModel GetLife360Data(out ApiStatus apiStatus)
        {
            var life360data = this.Life360DataFetcher.FetchRemoteData();
            if (life360data == null)
            {
                //apiStatus = this.ApiStatusFactory.Failed(ApiType.Life360, "Database lookup failed");
                apiStatus = null;
                return null;
            }

            if (life360data.DistanceData == null)
            {
                apiStatus = life360data.Status;
                return null;
            }

            apiStatus = life360data.Status;
            return new Life360UtilityModel()
            {
                Distance = life360data.DistanceData.Distance,
                TimeDelta = life360data.DistanceData.TimeDelta
            };
        }
    }
}
