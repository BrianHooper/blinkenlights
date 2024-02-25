using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Stock;

namespace Blinkenlights.Transformers
{
    public class StockTransformer : TransformerBase
    {
        private IDataFetcher<StockData> DataFetcher { get; init; }

        public StockTransformer(IApiHandler apiHandler, IDataFetcher<StockData> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.GetLocalData();
            if (response == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Database lookup failed");
                return new StockViewModel(errorStatus);
            }

            if (response.FinanceData?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Database record was empty");
                return new StockViewModel(errorStatus);
            }

			return new StockViewModel(response.FinanceData);
        }
    }
}
