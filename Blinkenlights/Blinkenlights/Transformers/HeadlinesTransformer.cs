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
            var response = this.DataFetcher.GetLocalData();
            if (response == null)
            {
                var errorStatus = ApiStatus.Failed("Headlines data", "Database Lookup failed");
                return new HeadlinesViewModel(errorStatus);
            }

            if (response.Headlines?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed("Headlines data", "No headlines in local data");
                return new HeadlinesViewModel(errorStatus);
            }

            return new HeadlinesViewModel(response.Headlines);
        }

    }
}
