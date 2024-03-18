using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Transformers;
using Microsoft.AspNetCore.Mvc;

namespace Blinkenlights.Controllers
{
    public class ModulesController : BlinkenController
    {
        private readonly ILogger<ModulesController> logger;

        public ModulesController(IServiceProvider serviceProvider, ILogger<ModulesController> logger) : base(serviceProvider)
        {
            this.logger = logger;
        }

        public async Task<IActionResult> GetSingleFlightData(string fid)
        {
            this.logger.LogInformation($"Getting flight data for {fid}");
            var dataFetcher = this.ServiceProvider.GetService<IDataFetcher<FlightStatusData>>() as FlightStatusDataFetcher;
            if (dataFetcher == null)
            {
                return Problem($"Failed to get DataFetcher<{typeof(FlightStatusData).Name}>");
            }
            var singleflightdata = await dataFetcher.GetFlightData(fid);
            if (!string.IsNullOrWhiteSpace(singleflightdata))
            {
                return Ok(singleflightdata);
            }
            else
            {
                return Problem("API response empty");
            }
        }

        private IActionResult FetchRemoteData<T>() where T : IDatabaseData
        {
            var dataFetcher = this.ServiceProvider.GetService<IDataFetcher<T>>();
            if (dataFetcher != null)
            {
                dataFetcher.FetchRemoteData(true);
                return Ok("Success");
            }
            return Problem($"Failed to get DataFetcher<{typeof(T).Name}>");
        }

        public IActionResult GetData(string id)
        {
            return id switch
            {
                "Calendar" => FetchRemoteData<CalendarModuleData>(),
                "Headlines" => FetchRemoteData<HeadlinesData>(),
                "OuterSpace" => FetchRemoteData<OuterSpaceData>(),
                "Life360" => FetchRemoteData<Life360Data>(),
                "Slideshow" => FetchRemoteData<SlideshowData>(),
                "Stock" => FetchRemoteData<StockData>(),
                "Time" => FetchRemoteData<TimeData>(),
                "Utility" => FetchRemoteData<UtilityData>(),
                "Weather" => FetchRemoteData<WeatherData>(),
                "WWII" => FetchRemoteData<WWIIData>(),
                "FlightStatus" => FetchRemoteData<FlightStatusData>(),
                _ => Problem($"Failed to match {id}")
            };
        }

        public IActionResult GetCalendarModule()
        {
            return GetPartialView<CalendarTransformer>("CalendarModule");
        }

        public IActionResult GetTimeModule()
        {
            return GetPartialView<TimeTransformer>("TimeModule");
        }

        public IActionResult GetWWIIModule()
        {
            return GetPartialView<WWIITransformer>("WWIIModule");
        }

        public IActionResult GetLife360Module()
        {
            return GetPartialView<Life360Transformer>("Life360Module");
        }

        public IActionResult GetWeatherData()
        {
            return GetPartialView<WeatherTransformer>("WeatherModule");
        }

        public IActionResult GetUtilityData()
        {
            return GetPartialView<UtilityTransformer>("UtilityModule");
        }

        public IActionResult GetHeadlinesModule()
        {
            return GetPartialView<HeadlinesTransformer>("HeadlinesModule");
        }

        public IActionResult GetSlideshowModule()
        {
            return GetPartialView<SlideshowTransformer>("SlideshowModule");
        }

        public IActionResult GetOuterSpaceModule()
        {
            return GetPartialView<OuterSpaceTransformer>("OuterSpaceModule");
		}

		public IActionResult GetStockModule()
		{
			return GetPartialView<StockTransformer>("StockModule");
		}

		public IActionResult GetFlightStatusModule()
		{
			return GetPartialView<FlightStatusTransformer>("FlightStatusModule");
		}
	}
}