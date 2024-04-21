using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;

namespace Blinkenlights.DataFetchers
{
    public class IndexDataFetcher : DataFetcherBase<IndexModuleData>
    {
        public IndexDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<IndexDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

		protected override IndexModuleData GetRemoteData(IndexModuleData existingData = null, bool overwrite = false)
        {
            return new IndexModuleData()
            {
                TimeStamp = DateTime.Now,
                Modules = CreateModulesData()
            };
        }

        private List<ModulePlacementData> CreateModulesData()
        {
            return new List<ModulePlacementData>()
            {
                
    //            // Row 1
    //            ModulePlacementData.Create(name: "Time", endpoint: "/Modules/GetTimeModule", refreshRateMs: 60 * 5 * 1000, row: 1, col: 1, rowSpan: 2),
    //            ModulePlacementData.Create(name: "Weather", endpoint: "/Modules/GetWeatherData", refreshRateMs: 60 * 5 * 1000, row: 1, col: 2, colSpan: 5, rowSpan: 2),
				//ModulePlacementData.Create(name : "Slideshow", endpoint : "/Modules/GetSlideshowModule", refreshRateMs : 10 * 60 * 1000, row : 1, col : 7, colSpan : 2, rowSpan: 2),

    //            // Row 2
    //            ModulePlacementData.Create(name: "WWII", endpoint: "/Modules/GetWWIIModule", refreshRateMs: 60 * 5 * 1000, row: 3, col: 1, colSpan: 2, rowSpan: 2),
    //            ModulePlacementData.Create(name : "Headlines", endpoint : "/Modules/GetHeadlinesModule", refreshRateMs : 60 * 5 * 1000, row : 3, col : 3, colSpan : 4, rowSpan: 2),
    //            ModulePlacementData.Create(name : "Stock", endpoint : "/Modules/GetStockModule", refreshRateMs : 60 * 5 * 1000, row : 3, col : 7, colSpan : 2, rowSpan: 2),

    //            // Row 3
				//ModulePlacementData.Create(name : "Utility", endpoint : "/Modules/GetUtilityData", refreshRateMs : 60 * 5 * 1000, row : 5, col : 1, colSpan : 2, rowSpan: 2),
				//ModulePlacementData.Create(name : "Calendar", endpoint : "/Modules/GetCalendarModule", refreshRateMs : 60 * 5 * 1000, row : 5, col : 3, colSpan: 2, rowSpan: 2),
    //            ModulePlacementData.Create(name : "FlightStatus", endpoint : "/Modules/GetFlightStatusModule", refreshRateMs : 60 * 1000, row : 5, col : 5, colSpan : 2, rowSpan: 2),
    //            ModulePlacementData.Create(name : "OuterSpace", endpoint : "/Modules/GetOuterSpaceModule", refreshRateMs : 60 * 5 * 1000, row : 5, col : 7, colSpan : 2, rowSpan: 2),
                

                //ModulePlacementData.Create(name : "FlightStatus", endpoint : "/Modules/GetFlightStatusModule", refreshRateMs : 5 * 60 * 1000, row : 1, col : 1, colSpan : 1, rowSpan: 1),
                ModulePlacementData.Create(name : "Automata", endpoint : "/Modules/GetAutomataModule", refreshRateMs : 60 * 5 * 1000, row : 1, col : 1),

			};
        }
    }
}
