using Blinkenlights.ApiHandlers;
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
		private IApiStatusFactory ApiStatusFactory { get; init; }

		public StockTransformer(IApiHandler apiHandler, IDataFetcher<StockData> dataFetcher, IApiStatusFactory apiStatusFactory) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
            this.ApiStatusFactory = apiStatusFactory;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.FetchRemoteData();
            if (response == null)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, "Database lookup failed");
                return new StockViewModel(errorStatus);
            }

            if (response.FinanceData?.Any() != true)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, "Database record was empty");
                return new StockViewModel(errorStatus);
            }

            var statuses = new List<ApiStatus>();
			if (response.FinanceData?.Any() == true)
			{
				statuses.Concat(response.FinanceData.Select(f => f.Status));
			}
			if (response.CurrencyData?.Any() == true)
			{
				statuses.Concat(response.CurrencyData.Select(f => f.Status));
			}
            return new StockViewModel(statuses.ToArray())
            {
                Data = response.FinanceData,
                CurrencyData = response.CurrencyData,
            };
        }
    }
}
