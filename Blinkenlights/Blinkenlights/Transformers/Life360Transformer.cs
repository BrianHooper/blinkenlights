using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Life360;

namespace Blinkenlights.Transformers
{
    public class Life360Transformer : TransformerBase
    {
        private IDataFetcher<Life360Data> DataFetcher { get; init; }

        public Life360Transformer(IApiHandler apiHandler, IDataFetcher<Life360Data> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.FetchRemoteData();
            if (response == null)
            {
                //var status = this.ApiStatusFactory.Failed(ApiType.Life360, "Api response is null");
                return new Life360ViewModel();
            }

            if (response.Locations?.Any() != true)
            {
                //var status = this.ApiStatusFactory.Failed(ApiType.Life360, "Api response is empty");
                return new Life360ViewModel();
            }

            return new Life360ViewModel(response.Locations, response.Status);
        }
    }
}
