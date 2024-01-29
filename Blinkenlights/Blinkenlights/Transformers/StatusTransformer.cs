using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Status;
using LiteDbLibrary;
using LiteDbLibrary.Schemas;

namespace Blinkenlights.Transformers
{
	public class StatusTransformer : TransformerBase
	{

		public StatusTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var modulesToLoad = this.LiteDb.Read<ModuleItem>().Select(m => m.Name);

			var apiTypes = Enum.GetValues<ApiType>();
			var apiInfoPairs = apiTypes.Select(t => (apiType: t, apiInfo: t.Info()));
			var validApiTypes = apiInfoPairs.Where(t => t.apiInfo != null && t.apiInfo.ReportedInModule && modulesToLoad.Contains(t.apiInfo.ModuleRootName));
			var apisToReport = validApiTypes.Select(t => t.apiType);

			return new StatusViewModel(apisToReport);
		}
	}
}
