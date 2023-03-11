using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;
using Blinkenlights.Transformers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// TODO Stock/Currency graphs

namespace Blinkenlights.Controllers
{
    public class ModulesController : Controller
	{
		private readonly IServiceProvider ServiceProvider;

		public ModulesController(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
		}

		private PartialViewResult GetModule<T>(string partialViewName) where T : TransformerBase
		{
			var transformer = (TransformerBase)ActivatorUtilities.CreateInstance<T>(this.ServiceProvider);
			var viewModel = transformer.Transform();
			return PartialView(partialViewName, viewModel);
		}

		public IActionResult GetCalendarModule()
		{
			return GetModule<CalendarTransformer>("CalendarModule");
		}

		public IActionResult GetTimeModule()
		{
			return GetModule<TimeTransformer>("TimeModule");
		}

		public IActionResult GetWWIIModule()
		{
			return GetModule<WWIITransformer>("WWIIModule");
		}

        public IActionResult GetLife360Module()
		{
			return GetModule<Life360Transformer>("Life360Module");
		}

        public IActionResult GetWeatherData()
        {
			return GetModule<WeatherTransformer>("WeatherModule");
		}

        public IActionResult GetMehData()
		{
			return GetModule<MehTransformer>("MehModule");
		}

        public IActionResult GetHeadlinesModule()
		{
			return GetModule<HeadlinesTransformer>("HeadlinesModule");
		}

		public IActionResult GetSlideshowModule()
		{
			return GetModule<SlideshowTransformer>("SlideshowModule");
		}
	}
}
