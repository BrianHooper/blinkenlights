using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.Stock;
using System.Text.Json;

namespace Blinkenlights.DataFetchers
{
    public class StockDataFetcher : DataFetcherBase<StockData>
    {
        public StockDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<WeatherDataFetcher> logger) : base(databaseHandler, apiHandler, logger)
        {
            Start();
        }

		public override StockData GetRemoteData(StockData existingData = null, bool overwrite = false)
		{
			var msft = FetchStockData(existingData?.FinanceData?.FirstOrDefault(d => string.Equals("MSFT", d?.Symbol, StringComparison.OrdinalIgnoreCase)), "MSFT", overwrite);
			var aapl = FetchStockData(existingData?.FinanceData?.FirstOrDefault(d => string.Equals("AAPL", d?.Symbol, StringComparison.OrdinalIgnoreCase)), "AAPL", overwrite);
			var data = new List<FinanceData>() { msft.Result, aapl.Result };

            return new StockData()
            {
                FinanceData = data,
                TimeStamp = DateTime.Now,
            };
        }

        private async Task<FinanceData> FetchStockData(FinanceData existingData, string ticker, bool overwrite)
        {
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.AlphaVantage.Info()))
			{
				this.Logger.LogDebug($"Using cached data for {ApiType.AlphaVantage} API");
				return existingData;
			}

			this.Logger.LogInformation($"Calling {ApiType.AlphaVantage} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.AlphaVantage, null, ticker);
			if (response is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Api response is null");
				return FinanceData.Clone(existingData, errorStatus);
			}
			else if (string.IsNullOrWhiteSpace(response.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Api response is empty", response.LastUpdateTime);
				return FinanceData.Clone(existingData, errorStatus);
			}

			StockErrorModel errorResponse = null;
			try
			{
				errorResponse = JsonSerializer.Deserialize<StockErrorModel>(response.Data);
			}
			catch (Exception ex) { }
			if (!string.IsNullOrWhiteSpace(errorResponse?.Information) && errorResponse.Information.Contains("rate limit"))
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Rate limited");
				return FinanceData.Clone(existingData, errorStatus);
			}

			StockJsonModel model;
			try
			{
				model = StockJsonModel.FromJson(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Error deserializing api response", response.LastUpdateTime);
				return FinanceData.Clone(existingData, errorStatus);
			}

			var symbol = model?.MetaData?.Symbol;
			if (string.IsNullOrWhiteSpace(symbol))
			{
				var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Required data missing in api response", response.LastUpdateTime);
				return FinanceData.Clone(existingData, errorStatus);
			}
			var timeIndex = 0;
			var dataPoints = model.TimeSeries.Select(kv => new GraphDataPoint() { X = timeIndex++, Y = double.Parse(kv.Value.Close) }).ToArray();
			var price = $"${Math.Round(dataPoints.First().Y, 2)}";

			var status = ApiStatus.Success(ApiType.AlphaVantage.ToString(), response.LastUpdateTime, response.ApiSource);
			return new FinanceData()
			{
				Symbol = symbol,
				Price = price,
				DataPoints = dataPoints,
				Status = status
			};
		}
    }
}
