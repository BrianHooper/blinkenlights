using Blinkenlights.ApiHandlers;
using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using NodaTime;

namespace Blinkenlights.DataFetchers
{
    public class TimeDataFetcher : DataFetcherBase<TimeData>
    {
        public TimeDataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger<TimeDataFetcher> logger, IApiStatusFactory apiStatusFactory) : base(databaseHandler, apiHandler, logger, apiStatusFactory)
        {
        }

		protected override TimeData GetRemoteData(TimeData existingData = null, bool overwrite = false)
        {
            if (!overwrite && !IsExpired(existingData?.Status, ApiType.TimeZone.Info()))
			{
				return existingData;
            }

            this.Logger.LogInformation($"Calling {ApiType.TimeZone} remote API");
            var timezones = new Dictionary<string, string>()
            {
                { "Seattle", "America/Los_Angeles" },
                { "Beijing", "Asia/Shanghai"},
                { "HCMC", "Asia/Ho_Chi_Minh"},
                { "D.C.", "America/New_York"},
                { "India", "Asia/Kolkata"}
            };

            Instant now = SystemClock.Instance.GetCurrentInstant();

            var tzInfos = new Dictionary<string, int>();
            foreach (var kv in timezones)
            {
                var tz = DateTimeZoneProviders.Tzdb[kv.Value];
                var utcOffset = tz.GetUtcOffset(now).Seconds;
                tzInfos.Add(kv.Key, utcOffset);
            }
            tzInfos.Add("UTC", 0);

            var countdownInfoDates = new SortedDictionary<string, DateTime>()
            {
                { "Mexico", new DateTime(2024, 3, 21) },
                { "Naynay's party", new DateTime(2024, 5, 16) },
                { "APOG", new DateTime(2024, 7, 5) },
            };

            var countdownInfos = new SortedDictionary<string, string>();
            foreach (var kv in countdownInfoDates)
            {
                countdownInfos.Add(kv.Value.ToString("yyyy-MM-dd"), kv.Key);
            }

            return new TimeData()
            {
                Status = this.ApiStatusFactory.Success(ApiType.TimeZone, DateTime.Now, ApiSource.Prod),
                TimeZoneInfos = tzInfos,
                CountdownInfos = countdownInfos,
                TimeStamp = DateTime.Now,
            };
        }
    }
}
