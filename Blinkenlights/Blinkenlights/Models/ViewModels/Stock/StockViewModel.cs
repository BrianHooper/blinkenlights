using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels.Weather;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Stock
{
	public class StockViewModel : ApiResultBase
	{
		public List<FinanceData> Data { get; set; }

		public StockViewModel(ApiStatus status, List<FinanceData> data = null) : base("Stock", status)
		{
			this.Data = data;
		}
	}

	public class FinanceData
	{
		public string Symbol { get; set; }

		public string Price { get; set; }

		public GraphDataPoint[] DataPoints { get; init; }

		public string SerializePoints()
		{
			var jsonSettings = new JsonSerializerSettings()
			{
				NullValueHandling = NullValueHandling.Include,
			};
			return JsonConvert.SerializeObject(this.DataPoints, jsonSettings);
		}
	}

	public class GraphDataPoint
	{
		public int X { get; set; }
		public double Y { get; set; }

		public GraphDataPoint(int X, double Y) 
		{
			this.X = X;
			this.Y = Y;
		}

		public GraphDataPoint(int timeIndex, TimeSeriesDataPoint timeSeriesDataPoint)
		{
			this.X = timeIndex;
			this.Y = double.Parse(timeSeriesDataPoint.Close);
		}
	}
}
