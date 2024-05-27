using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.WWII;

namespace Blinkenlights.Transformers
{
    public class WWIITransformer : TransformerBase
    {
        IDataFetcher<WWIIData> DataFetcher { get; set; }

        public WWIITransformer(IApiHandler apiHandler, IDataFetcher<WWIIData> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var data = this.DataFetcher.FetchRemoteData();
            if (data is null)
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.WWII, "Database lookup failed");
                return new WWIIDayModel();
            }

            var key = DateTime.Now.ToString("d MMM yyyy");
            if (data?.Days?.TryGetValue(key, out var currentDayData) != true || (currentDayData?.GlobalEvents?.Any() != true && currentDayData?.RegionalEvents?.Any() != true))
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.WWII, "No data available for today");
                return new WWIIDayModel();
            }

            return new WWIIDayModel(currentDayData.Date, currentDayData.GlobalEvents, currentDayData.RegionalEvents);
        }
    }
}
