using Blinkenlights.Data.LiteDb;
using Blinkenlights.Models;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Calendar;
using Blinkenlights.Transformers;
using LiteDbLibrary;
using LiteDbLibrary.Schemas;
using Newtonsoft.Json;
using System.Drawing.Drawing2D;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace Blinkenlights.Transformers
{
	public class IndexTransformer : TransformerBase
	{
		public IndexTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			List<ModuleItem> modules = this.LiteDb.Read<ModuleItem>();

			var numColumns = modules.Max(m => m.ColEnd) - 1;
			var numRows = modules.Max(m => m.RowEnd) - 1;

			var viewModel = new IndexViewModel()
			{
				Modules = modules,
				GridTemplateStyle = $"grid-template-columns: repeat({numColumns}, minmax(0, 1fr)); grid-template-rows: repeat({numRows}, minmax(0, 1fr));"
			};

			return viewModel;
		}
	}
}
