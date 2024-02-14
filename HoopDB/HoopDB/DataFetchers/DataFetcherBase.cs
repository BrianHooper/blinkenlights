using System.Timers;

namespace HoopDB
{
	public abstract class DataFetcherBase : IDataFetcher
	{
		protected TimeSpan TimerInterval { get; init; }

		protected System.Timers.Timer FetchTimer { get; init; }

		public DataFetcherBase(TimeSpan timerInterval)
		{
			this.TimerInterval = timerInterval;

			this.FetchTimer = new System.Timers.Timer();

			this.FetchTimer.Elapsed += OnTimer;
			this.FetchTimer.AutoReset = false;
			this.FetchTimer.Start();
		}

        public void Start()
        {
            this.FetchTimer.Start();
        }

        public void OnTimer(Object source, ElapsedEventArgs e)
		{
			Console.WriteLine("DFB at {0:HH:mm:ss.fff}", e.SignalTime);
			this.FetchData();
			this.FetchTimer.Stop();
			this.FetchTimer.Interval = this.TimerInterval.TotalMilliseconds;
			this.FetchTimer.Start();
		}

		public abstract void FetchData();
	}
}
