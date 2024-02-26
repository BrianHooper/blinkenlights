using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;

namespace Blinkenlights.DataFetchers
{
    public class FlightStatusDataFetcher : DataFetcherBase<FlightStatusData>
    {
        public FlightStatusDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<FlightStatusDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

        public override FlightStatusData GetRemoteData(FlightStatusData existingData = null, bool overwrite = false)
        {
            throw new NotImplementedException();
        }
    }
}
