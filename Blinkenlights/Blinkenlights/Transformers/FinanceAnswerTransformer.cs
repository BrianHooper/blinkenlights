using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.FinanceAnswer;
using LiteDbLibrary;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
	public class FinanceAnswerTransformer : TransformerBase
	{
		public FinanceAnswerTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var response = this.ApiHandler.Fetch(ApiType.AlphaVantage).Result;
			if (response is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage, null, "Api response is null");
				return new FinanceAnswerViewModel(errorStatus);
			}
			else if (string.IsNullOrWhiteSpace(response.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage, response, "Api response is empty");
				return new FinanceAnswerViewModel(errorStatus);
			}

			FinanceAnswerJsonModel model;
			try
			{
				model = JsonConvert.DeserializeObject<FinanceAnswerJsonModel>(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage, response, "Error deserializing api response");
				return new FinanceAnswerViewModel(errorStatus);
			}

			var symbol = model?.MetaData?.Symbol;
			if (string.IsNullOrWhiteSpace(symbol))
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage, response, "Required data missing in api response");
				return new FinanceAnswerViewModel(errorStatus);
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

			var status = ApiStatus.Success(ApiType.AlphaVantage, response);
			this.ApiHandler.TryUpdateCache(response);
			return new FinanceAnswerViewModel(status, data);
		}
	}
}
