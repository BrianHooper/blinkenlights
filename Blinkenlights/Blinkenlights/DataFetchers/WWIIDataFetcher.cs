﻿using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.WWII;
using Humanizer;
using Newtonsoft.Json;

namespace Blinkenlights.DataFetchers
{
    public class WWIIDataFetcher : DataFetcherBase<WWIIData>
    {
        public WWIIDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromHours(24), databaseHandler, apiHandler)
        {
            Start();
        }

        protected override WWIIData GetRemoteData(WWIIData existingData)
        {
            var response = this.ApiHandler.Fetch(ApiType.WWII).Result;
            if (string.IsNullOrWhiteSpace(response?.Data))
            {
                var errorStatus = ApiStatus.Failed(ApiType.WWII.ToString(), "Failed to get local data");
                return WWIIData.Clone(existingData, errorStatus);
            }

            WWIIJsonModel wWIIViewModel = null;
            try
            {
                wWIIViewModel = JsonConvert.DeserializeObject<WWIIJsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var errorStatus = ApiStatus.Failed(ApiType.WWII.ToString(), "Failed to deserialize data");
                return WWIIData.Clone(existingData, errorStatus);
            }

            var dayData = new Dictionary<string, WWIIDayData>();

            var now = DateTime.Now;
            var date = now.AddYears(-80);
            for (int i = 0; i < 5; i++)
            {
                var dateStr = date.AddDays(i).ToString("d MMM yyyy");

                if (wWIIViewModel?.Days?.TryGetValue(dateStr, out var wWIIDayJsonModel) != true || wWIIDayJsonModel?.Events?.Any() != true)
                {
                    continue;
                }

                var dateFormatted = String.Format("{0} {1:MMMM}, {2:yyyy}", date.Day.Ordinalize(), date, date);
                var globalEvents = wWIIDayJsonModel.Events.FirstOrDefault(kv => string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).Value;
                var regionalEvents = wWIIDayJsonModel.Events.Where(kv => !string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).ToList();

                if (globalEvents?.Any() != true)
                {
                    continue;
                }

                var key = now.AddDays(i).ToString("d MMM yyyy");
                dayData.Add(key, new WWIIDayData(dateFormatted, globalEvents, regionalEvents));
            }

            if (!dayData.Any())
            {
                var errorStatus = ApiStatus.Failed(ApiType.WWII.ToString(), "Failed to parse any day data");
                return WWIIData.Clone(existingData, errorStatus);
            }

            var status = ApiStatus.Success(ApiType.WWII.ToString(), DateTime.Now, ApiSource.Prod);
            return new WWIIData()
            {
                Status = status,
                Days = dayData,
            };
        }
    }
}