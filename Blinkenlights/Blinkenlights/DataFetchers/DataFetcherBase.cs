using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using System.Timers;

namespace Blinkenlights.DataFetchers
{
    public abstract class DataFetcherBase<T> : IDataFetcher<T> where T : IDatabaseData
    {
        protected IDatabaseHandler DatabaseHandler { get; }
        protected IApiHandler ApiHandler { get; }
        protected ILogger Logger { get; init; }

        protected System.Timers.Timer FetchTimer { get; init; }

        public DataFetcherBase(IDatabaseHandler databaseHandler, IApiHandler apiHandler, ILogger logger)
        {
            this.DatabaseHandler = databaseHandler;
            this.ApiHandler = apiHandler;
            this.Logger = logger;

            this.FetchTimer = new System.Timers.Timer();

            this.FetchTimer.Elapsed += OnTimer;
            this.FetchTimer.AutoReset = false;
        }

        public void Start()
        {
            this.FetchTimer.Start();
        }

        public void OnTimer(Object source, ElapsedEventArgs e)
        {
            this.FetchRemoteData();
            this.FetchTimer.Stop();
            this.FetchTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            this.FetchTimer.Start();
        }

        public T GetLocalData()
        {
            return this.DatabaseHandler.Get<T>();
        }

		public T FetchRemoteData(bool overwrite = false)
        {
            var existingData = GetLocalData();
            var updatedData = GetRemoteData(existingData, overwrite);
            this.DatabaseHandler.Set(updatedData);
            return updatedData;
        }

		public abstract T GetRemoteData(T existingData = default, bool overwrite = false);

        public static bool IsExpired(ApiStatus status, IApiInfo apiInfo)
        {
            if (status?.LastUpdate == null || apiInfo == null)
            {
                return true;
            }

            var timeDelta = DateTime.Now - status.LastUpdate;
			return timeDelta > apiInfo.Timeout;
        }
    }
}
