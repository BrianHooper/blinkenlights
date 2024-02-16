﻿using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.Stock;
using Newtonsoft.Json;

namespace Blinkenlights.DataFetchers
{
    public class StockDataFetcher : DataFetcherBase<StockData>
    {
        public StockDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromMinutes(60), databaseHandler, apiHandler)
        {
            Start();
        }

        protected override StockData GetRemoteData(StockData existingData = null)
        {
            var response = this.ApiHandler.Fetch(ApiType.AlphaVantage).Result;
            if (response is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Api response is null");
                return StockData.Clone(existingData, errorStatus);
            }
            else if (string.IsNullOrWhiteSpace(response.Data))
            {
                var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Api response is empty", response.LastUpdateTime);
                return StockData.Clone(existingData, errorStatus);
            }

            StockJsonModel model;
            try
            {
                model = JsonConvert.DeserializeObject<StockJsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Error deserializing api response", response.LastUpdateTime);
                return StockData.Clone(existingData, errorStatus);
            }

            var symbol = model?.MetaData?.Symbol;
            if (string.IsNullOrWhiteSpace(symbol))
            {
                var errorStatus = ApiStatus.Failed(ApiType.AlphaVantage.ToString(), "Required data missing in api response", response.LastUpdateTime);
                return StockData.Clone(existingData, errorStatus);
            }
            var timeIndex = 0;
            var dataPoints = model.TimeSeriesDataPoints.Select(kv => new GraphDataPoint() { X = timeIndex++, Y = double.Parse(kv.Value.Close) }).ToArray();
            var price = $"${Math.Round(dataPoints.First().Y, 2)}";
            var dataModel = new FinanceData()
            {
                Symbol = symbol,
                Price = price,
                DataPoints = dataPoints
            };
            var data = new List<FinanceData>() { dataModel };

            var status = ApiStatus.Success(ApiType.AlphaVantage.ToString(), response.LastUpdateTime, response.ApiSource);
            return new StockData()
            {
                FinanceData = data,
                Status = status,
            };
        }
    }
}
