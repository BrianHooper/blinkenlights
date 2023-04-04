using Blinkenlights.Models.ViewModels;
using Blinkenlights.Transformers;
using Microsoft.AspNetCore.Mvc;

namespace Blinkenlights.Controllers
{
	public abstract class BlinkenController : Controller
	{
		protected readonly IServiceProvider ServiceProvider;

		public BlinkenController(IServiceProvider serviceProvider)
		{
			this.ServiceProvider = serviceProvider;
		}

		protected IModuleViewModel Transform<T>() where T : TransformerBase
		{
			var transformer = (TransformerBase)ActivatorUtilities.CreateInstance<T>(this.ServiceProvider);
			return transformer.Transform();
		}

		protected ViewResult GetView<T>(string viewName) where T : TransformerBase
		{
			var viewModel = Transform<T>();
			return View(viewName, viewModel);
		}

		protected PartialViewResult GetPartialView<T>(string partialViewName) where T : TransformerBase
		{
			var viewModel = Transform<T>();
			return PartialView(partialViewName, viewModel);
		}

	}
}
