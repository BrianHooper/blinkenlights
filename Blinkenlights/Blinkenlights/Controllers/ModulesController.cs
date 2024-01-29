using Blinkenlights.Transformers;
using Microsoft.AspNetCore.Mvc;

// TODO Stock/Currency graphs

namespace Blinkenlights.Controllers
{
    public class ModulesController : BlinkenController
	{
		public ModulesController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
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

		public IActionResult GetIssTrackerModule()
		{
			return GetPartialView<IssTrackerTransformer>("IssTrackerModule");
		}

		public IActionResult GetFinanceAnswerModule()
		{
			return GetPartialView<FinanceAnswerTransformer>("FinanceAnswerModule");
		}
	}
}
