using Blinkenlights.Data.LiteDb;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;
using LiteDbLibrary;

namespace Blinkenlights.Transformers
{
	public abstract class TransformerBase
	{
		protected IApiHandler ApiHandler { get; set; }

		protected ILiteDbHandler LiteDb { get; set; }

		protected TransformerBase(IApiHandler apiHandler, ILiteDbHandler liteDbHandler)
		{
			ApiHandler = apiHandler;
			LiteDb = liteDbHandler;
		}

		public abstract IModuleViewModel Transform();
	}
}
