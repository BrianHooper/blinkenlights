using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Time;

namespace Blinkenlights.Transformers
{
    public class TimeTransformer : TransformerBase
    {
        private IDataFetcher<TimeData> DataFetcher { get; init; }

        public TimeTransformer(IApiHandler apiHandler, IDataFetcher<TimeData> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.FetchRemoteData();
            if (response is null)
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.TimeZone, "Failed to get local data");
                return new TimeViewModel();
            }

            if (response.TimeZoneInfos?.Any() != true && response.CountdownInfos?.Any() != true)
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.TimeZone, "Database response was empty");
                return new TimeViewModel();
            }

            //var status = this.ApiStatusFactory.Success(ApiType.TimeZone, response.TimeStamp, ApiSource.Prod);
            var viewModel = new TimeViewModel(response.Status)
            {
                TimeZoneInfos = response.TimeZoneInfos,
                CountdownInfos = response.CountdownInfos
            };

            return viewModel;
        }
    }
}
