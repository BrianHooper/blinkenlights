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

            var life360Data = GetLife360Data();

            return new UtilityViewModel(response.MehData?.Status)
            {
                MehData = response.MehData,
                PackageTrackingData = response.PackageTrackingData,
                Life360Data = life360Data,
            };
        }

        private Life360UtilityModel GetLife360Data()
        {
            var life360data = this.Life360DataFetcher.FetchRemoteData();
            if (life360data == null)
            {
                //apiStatus = this.ApiStatusFactory.Failed(ApiType.Life360, "Database lookup failed");
                return null;
            }

            if (life360data.DistanceData == null)
            {
                return null;
            }

            return new Life360UtilityModel()
            {
                Status = life360data.Status,
                Distance = life360data.DistanceData.Distance,
                TimeDelta = life360data.DistanceData.Time,
            };
        }
    }
}
