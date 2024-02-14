using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;

namespace Blinkenlights.DataFetchers
{
    public class UtilityDataFetcher : DataFetcherBase<UtilityData>
    {
        public UtilityDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromMinutes(60), databaseHandler, apiHandler)
        {
        }

        protected override UtilityData GetRemoteData(UtilityData existingData = null)
        {
            throw new NotImplementedException();
        }
    }
}
