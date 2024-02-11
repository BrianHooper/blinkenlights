using Blinkenlights.Dataschemas;

namespace Blinkenlights.DataFetchers
{
	public interface IIndexDataFetcher : IDataFetcher
	{
		public IssTrackerData GetData();
	}

	public class IndexDataFetcher : DataFetcherBase, IIndexDataFetcher
	{
		public IndexDataFetcher(TimeSpan timerInterval) : base(TimeSpan.FromMinutes(5))
		{
		}

		public override void FetchData()
		{
			throw new NotImplementedException();
		}

		public IssTrackerData GetData()
		{
			throw new NotImplementedException();
		}
	}
}
