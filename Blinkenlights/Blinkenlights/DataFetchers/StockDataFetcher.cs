using Blinkenlights.ApiHandlers;
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
        public StockDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<WeatherDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
            Start();
        }

		public override StockData GetRemoteData(StockData existingData = null, bool overwrite = false)
		{
			var msft = FetchStockData(existingData, "MSFT", overwrite);
			var btc = FetchCurrencyData(existingData, "BTC", overwrite);

            return new StockData()
			{
				FinanceData = new List<FinanceData>() { msft.Result },
				CurrencyData = new List<CurrencyData>() { btc.Result },
				TimeStamp = DateTime.Now,
            };
        }

		private async Task<CurrencyData> FetchCurrencyData(StockData existingStockData, string symbol, bool overwrite)
		{
			var existingData = existingStockData?.CurrencyData?.FirstOrDefault(d => string.Equals(symbol, d?.Symbol, StringComparison.OrdinalIgnoreCase));
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.AlphaVantageCurrency.Info()))
			{
				return existingData;
			}

			if (existingData?.Status?.NextValidRequestTime != null && DateTime.Now < existingData.Status.NextValidRequestTime)
			{
				this.Logger.LogWarning($"Waiting for rate limit for {ApiType.AlphaVantageCurrency} API, next valid request: {existingData.Status.NextValidRequestTime}");
				return existingData;
			}

			this.Logger.LogInformation($"Calling {ApiType.AlphaVantageCurrency} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.AlphaVantageCurrency, null, symbol, "USD");
			if (response is null)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantageCurrency, "Api response is null");
				return CurrencyData.Clone(existingData, symbol, errorStatus);
			}
			else if (response.ResultStatus != ApiResultStatus.Success)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantageCurrency, $"Api failed: {response.StatusMessage}", response.LastUpdateTime);
				return CurrencyData.Clone(existingData, symbol, errorStatus);
			}
			else if (string.IsNullOrWhiteSpace(response.Data))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantageCurrency, "Api response is empty", response.LastUpdateTime);
				return CurrencyData.Clone(existingData, symbol, errorStatus);
			}

			StockErrorModel errorResponse = null;
			try
			{
				errorResponse = JsonSerializer.Deserialize<StockErrorModel>(response.Data);
			}
			catch (Exception ex) { }
			if (!string.IsNullOrWhiteSpace(errorResponse?.Information) && errorResponse.Information.Contains("rate limit"))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantageCurrency, "Rate limited", existingData?.Status?.LastUpdate, DateTime.Now.AddHours(2));
				return CurrencyData.Clone(existingData, symbol, errorStatus);
			}

			CurrencyJsonModel model;
			try
			{
				model = JsonSerializer.Deserialize<CurrencyJsonModel>(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantageCurrency, "Error deserializing api response", response.LastUpdateTime);
				return CurrencyData.Clone(existingData, symbol, errorStatus);
			}

			if (model?.RealtimeCurrencyExchangeRate == null || string.IsNullOrWhiteSpace(model.RealtimeCurrencyExchangeRate.The5ExchangeRate))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantageCurrency, "Required data missing in api response", response.LastUpdateTime);
				return CurrencyData.Clone(existingData, symbol, errorStatus);
			}

			if (!Double.TryParse(model.RealtimeCurrencyExchangeRate.The5ExchangeRate, out var exchangeRate))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantageCurrency, "Error parsing exchange rate from api response", response.LastUpdateTime);
				return CurrencyData.Clone(existingData, symbol, errorStatus);
			}

			var status = this.ApiStatusFactory.Success(ApiType.AlphaVantageCurrency, response.LastUpdateTime, response.ApiSource);
			return new CurrencyData()
			{
				Symbol = symbol,
				Price = exchangeRate.ToString("C2"),
				Status = status
			};
		}

		private async Task<FinanceData> FetchStockData(StockData existingStockData, string ticker, bool overwrite)
        {
			var existingData = existingStockData?.FinanceData?.FirstOrDefault(d => string.Equals(ticker, d?.Symbol, StringComparison.OrdinalIgnoreCase));
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.AlphaVantage.Info()))
			{
				return existingData;
			}

			if (existingData?.Status?.NextValidRequestTime != null && DateTime.Now < existingData.Status.NextValidRequestTime)
			{
				this.Logger.LogWarning($"Waiting for rate limit for {ApiType.AlphaVantage} API, next valid request: {existingData.Status.NextValidRequestTime}");
				return existingData;
			}

			this.Logger.LogInformation($"Calling {ApiType.AlphaVantage} remote API");
			var response = await this.ApiHandler.Fetch(ApiType.AlphaVantage, null, ticker);
			if (response is null)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, "Api response is null");
				return FinanceData.Clone(existingData, ticker, errorStatus);
			}
			else if (response.ResultStatus != ApiResultStatus.Success)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, $"Api failed: {response.StatusMessage}", response.LastUpdateTime);
				return FinanceData.Clone(existingData, ticker, errorStatus);
			}
			else if (string.IsNullOrWhiteSpace(response.Data))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, "Api response is empty", response.LastUpdateTime);
				return FinanceData.Clone(existingData, ticker, errorStatus);
			}

			StockErrorModel errorResponse = null;
			try
			{
				errorResponse = JsonSerializer.Deserialize<StockErrorModel>(response.Data);
			}
			catch (Exception ex) { }
			if (!string.IsNullOrWhiteSpace(errorResponse?.Information) && errorResponse.Information.Contains("rate limit"))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, "Rate limited", existingData?.Status?.LastUpdate, DateTime.Now.AddHours(2));
				return FinanceData.Clone(existingData, ticker, errorStatus);
			}

			StockJsonModel model;
			try
			{
				model = StockJsonModel.FromJson(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, "Error deserializing api response", response.LastUpdateTime);
				return FinanceData.Clone(existingData, ticker, errorStatus);
			}

			var symbol = model?.MetaData?.Symbol;
			if (string.IsNullOrWhiteSpace(symbol))
			{
				var errorStatus = this.ApiStatusFactory.Failed(ApiType.AlphaVantage, "Required data missing in api response", response.LastUpdateTime);
				return FinanceData.Clone(existingData, ticker, errorStatus);
			}
			var timeIndex = 0;
			var dataPoints = model.TimeSeries.Select(kv => new GraphDataPoint() { X = timeIndex++, Y = double.Parse(kv.Value.Close) }).ToArray();
			var price = $"${Math.Round(dataPoints.First().Y, 2)}";

			var status = this.ApiStatusFactory.Success(ApiType.AlphaVantage, response.LastUpdateTime, response.ApiSource);
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
