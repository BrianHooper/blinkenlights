using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;

namespace Blinkenlights.DataFetchers
{
    public class StockDataFetcher : DataFetcherBase<StockData>
    {
        public StockDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromMinutes(60), databaseHandler, apiHandler)
        {
        }

        protected override StockData GetRemoteData(StockData existingData = null)
        {
            throw new NotImplementedException();
        }
    }
}
