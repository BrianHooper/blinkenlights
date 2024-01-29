using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.FinanceAnswer
{
	public class FinanceAnswerViewModel : ApiResultBase
	{
		public List<FinanceData> Data { get; set; }

		public FinanceAnswerViewModel(ApiStatus status, List<FinanceData> data = null) : base(status)
		{
			this.Data = data;
		}
	}

	public class FinanceData
	{
		public string Symbol { get; set; }

		public string Price { get; set; }

		public List<GraphDataPoint> DataPoints { get; set; }
	}

	public class GraphDataPoint
	{
		public string X { get; set; }
		public string Y { get; set; }

		public GraphDataPoint(string X, string Y) 
		{
			this.X = X;
			this.Y = Y;
		}

		public GraphDataPoint(string timestamp, TimeSeriesDataPoint timeSeriesDataPoint)
		{
			this.X = timestamp;
			this.Y = timeSeriesDataPoint.Close;
		}
	}
}
