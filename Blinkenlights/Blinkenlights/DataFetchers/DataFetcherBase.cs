using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using System.Timers;

namespace Blinkenlights.DataFetchers
{
    public abstract class DataFetcherBase<T> : IDataFetcher<T> where T : IModuleData
    {
        protected TimeSpan TimerInterval { get; init; }
        public IDatabaseHandler DatabaseHandler { get; }
        public IApiHandler ApiHandler { get; }
        protected System.Timers.Timer FetchTimer { get; init; }

        public DataFetcherBase(TimeSpan timerInterval, IDatabaseHandler databaseHandler, IApiHandler apiHandler)
        {
            this.DatabaseHandler = databaseHandler;
            this.ApiHandler = apiHandler;

            this.TimerInterval = timerInterval;
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
            this.FetchTimer.Interval = this.TimerInterval.TotalMilliseconds;
            this.FetchTimer.Start();
        }

        public void FetchRemoteData(bool overwrite = false)
        {
            Console.WriteLine($"Fetch {typeof(T).Name} remote data");
            var existingData = this.DatabaseHandler.Get<T>();
            if (!overwrite && existingData != null && (DateTime.Now - existingData.TimeStamp) < this.TimerInterval)
            {
                return;
            }

            var remoteData = GetRemoteData(existingData);
            if (remoteData != null)
            {
                this.DatabaseHandler.Set(remoteData);
            }
        }

        public T GetLocalData()
        {
            return this.DatabaseHandler.Get<T>();
        }

        protected abstract T GetRemoteData(T existingData = default);
    }
}
