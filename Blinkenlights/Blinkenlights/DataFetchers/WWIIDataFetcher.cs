using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.WWII;
using System.Text.Json;
using Humanizer;
using Blinkenlights.ApiHandlers;

namespace Blinkenlights.DataFetchers
{
    public class WWIIDataFetcher : DataFetcherBase<WWIIData>
    {
        public WWIIDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<WWIIDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
		}

		protected override WWIIData GetRemoteData(WWIIData existingData = null, bool overwrite = false)
		{
			if (!overwrite && !IsExpired(existingData?.Status, ApiType.WWII.Info()) && IsValid(existingData))
			{
				return existingData;
			}
             
			this.Logger.LogInformation($"Calling {ApiType.WWII} remote API");
			var response = this.ApiHandler.Fetch(ApiType.WWII).Result;
            if (string.IsNullOrWhiteSpace(response?.Data))
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.WWII, "Failed to get local data");
                return WWIIData.Clone(existingData, errorStatus);
            }

            WWIIJsonModel wWIIViewModel = null;
            try
            {
                wWIIViewModel = JsonSerializer.Deserialize<WWIIJsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.WWII, "Failed to deserialize data");
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
                var errorStatus = this.ApiStatusFactory.Failed(ApiType.WWII, "Failed to parse any day data");
                return WWIIData.Clone(existingData, errorStatus);
            }

            var status = this.ApiStatusFactory.Success(ApiType.WWII, DateTime.Now, ApiSource.Prod);
            return new WWIIData()
            {
                Status = status,
                Days = dayData,
                TimeStamp = DateTime.Now,
            };
        }

        protected bool IsValid(WWIIData existingData = null)
        {
            var key = DateTime.Now.ToString("d MMM yyyy");
            return existingData?.Days?.TryGetValue(key, out var currentDayData) == true && currentDayData?.GlobalEvents?.Any() == true;
        }
    }
}
