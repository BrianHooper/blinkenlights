using Blinkenlights.Dataschemas;

namespace Blinkenlights.DataFetchers
{
    public interface IDataFetcher<T> where T : IDatabaseData
    {
        public void Start();

		public T GetRemoteData(T existingData = default, bool overwrite = false);

        public T FetchRemoteData(bool overwrite = false);

		public T GetLocalData();
    }
}