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
            var response = this.DataFetcher.GetLocalData();
            if (response is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.TimeZone.ToString(), "Failed to get local data");
                return new TimeViewModel(errorStatus);
            }

            if (response.TimeZoneInfos?.Any() != true && response.CountdownInfos?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed(ApiType.TimeZone.ToString(), "Database response was empty");
                return new TimeViewModel(errorStatus);
            }

            var status = ApiStatus.Success(ApiType.TimeZone.ToString(), response.TimeStamp, ApiSource.Prod);
            var viewModel = new TimeViewModel(status)
            {
                TimeZoneInfos = response.TimeZoneInfos,
                CountdownInfos = response.CountdownInfos
            };

            return viewModel;
        }
    }
}
