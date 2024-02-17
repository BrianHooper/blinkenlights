using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;

namespace Blinkenlights.DataFetchers
{
    public class IndexDataFetcher : DataFetcherBase<IndexModuleData>
    {
        public IndexDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromMinutes(2), databaseHandler, apiHandler)
        {
        }

        protected override IndexModuleData GetRemoteData(IndexModuleData existingData = null)
        {
            return new IndexModuleData()
            {
                TimeStamp = DateTime.Now,
                Modules = CreateModulesData()
            };
        }

        protected override bool IsValid(IndexModuleData existingData = null)
        {
            return false;
        }

        private List<ModulePlacementData> CreateModulesData()
        {
            return new List<ModulePlacementData>()
            {
                // Row 1
                ModulePlacementData.Create(name: "Time", endpoint: "/Modules/GetTimeModule", refreshRateMs: 5 * 1000, row: 1, col: 1),
                ModulePlacementData.Create(name: "Weather", endpoint: "/Modules/GetWeatherData", refreshRateMs: 5 * 1000, row: 1, col: 2, colSpan: 5),
                ModulePlacementData.Create(name : "IssTracker", endpoint : "/Modules/GetIssTrackerModule", refreshRateMs : 5 * 1000, row : 1, col : 7, colSpan : 2),

                // Row 2
                ModulePlacementData.Create(name: "WWII", endpoint: "/Modules/GetWWIIModule", refreshRateMs: 5 * 1000, row: 2, col: 1, colSpan: 2),
                ModulePlacementData.Create(name : "Headlines", endpoint : "/Modules/GetHeadlinesModule", refreshRateMs : 5 * 1000, row : 2, col : 3, colSpan : 4),
                ModulePlacementData.Create(name : "Stock", endpoint : "/Modules/GetStockModule", refreshRateMs : 5 * 1000, row : 2, col : 7, colSpan : 2),

                // Row 3
                ModulePlacementData.Create(name : "Calendar", endpoint : "/Modules/GetCalendarModule", refreshRateMs : 5 * 1000, row : 3, col : 1, colSpan : 3),
                ModulePlacementData.Create(name : "Utility", endpoint : "/Modules/GetUtilityData", refreshRateMs : 5 * 1000, row : 3, col : 4, colSpan : 3),
                //ModulePlacementData.Create(name : "Life360", endpoint : "/Modules/GetLife360Module", refreshRateMs : 2 * 60 * 1000, row : 3, col : 5, colSpan : 2),
                ModulePlacementData.Create(name : "Slideshow", endpoint : "/Modules/GetSlideshowModule", refreshRateMs : 5 * 1000, row : 3, col : 7, colSpan : 2),
            };
        }
    }
}
