using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Headlines;

namespace Blinkenlights.Transformers
{

    public class HeadlinesTransformer : TransformerBase
    {
        private IDataFetcher<HeadlinesData> DataFetcher { get; init; }

        public HeadlinesTransformer(IApiHandler apiHandler, IDataFetcher<HeadlinesData> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.FetchRemoteData();
            if (response == null)
            {
                //var errorStatus = ApiStatus.Failed("Headlines data", "Database Lookup failed");
                return new HeadlinesViewModel();
            }

            if (response.Headlines?.Any() != true)
            {
                //var errorStatus = ApiStatus.Failed("Headlines data", "No headlines in local data");
                return new HeadlinesViewModel();
            }

            return new HeadlinesViewModel(response.Headlines);
        }

    }
}
