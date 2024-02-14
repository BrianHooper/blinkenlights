using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Transformers
{
    public class IndexTransformer : TransformerBase
    {
        private IDataFetcher<IndexModuleData> dataFetcher { get; init; }

        public IndexTransformer(IApiHandler apiHandler, IDataFetcher<IndexModuleData> dataFetcher) : base(apiHandler)
        {
            this.dataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var indexModuleData = this.dataFetcher.GetLocalData();
            if (indexModuleData?.Modules?.Any() != true)
            {
                return null;
            }

            var numColumns = indexModuleData.Modules.Max(m => m.ColEnd) - 1;
            var numRows = indexModuleData.Modules.Max(m => m.RowEnd) - 1;

            var viewModel = new IndexViewModel()
            {
                Modules = indexModuleData.Modules,
                GridTemplateStyle = $"grid-template-columns: repeat({numColumns}, minmax(0, 1fr)); grid-template-rows: repeat({numRows}, minmax(0, 1fr));"
            };

            return viewModel;
        }
    }
}
