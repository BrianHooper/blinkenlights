using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Stock;
using LiteDbLibrary;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
	public class StockTransformer : TransformerBase
	{
		public StockTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var response = this.ApiHandler.Fetch(ApiType.AlphaVantage).Result;
			if (response is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Api response is null");
				return new StockViewModel(errorStatus);
			}
			else if (string.IsNullOrWhiteSpace(response.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Api response is empty", response.LastUpdateTime);
				return new StockViewModel(errorStatus);
			}

			StockJsonModel model;
			try
			{
				model = JsonConvert.DeserializeObject<StockJsonModel>(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Error deserializing api response", response.LastUpdateTime);
				return new StockViewModel(errorStatus);
			}

			var symbol = model?.MetaData?.Symbol;
			if (string.IsNullOrWhiteSpace(symbol))
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Required data missing in api response", response.LastUpdateTime);
				return new StockViewModel(errorStatus);
			}
			var timeIndex = 0;
			var dataPoints = model.TimeSeriesDataPoints.Select(kv => new GraphDataPoint(timeIndex++, kv.Value)).ToArray();
			var price = $"${Math.Round(dataPoints.First().Y, 2)}";
			var dataModel = new FinanceData()
			{
				Symbol = symbol,
				Price = price,
				DataPoints = dataPoints
			};
			var data = new List<FinanceData>() { dataModel };

			var status = ApiStatus.Success(ApiType.AlphaVantage.ToString(), response.LastUpdateTime, response.ApiSource);
			this.ApiHandler.TryUpdateCache(response);
			return new StockViewModel(status, data);
		}
	}
}
