using Blinkenlights.Dataschemas;

namespace Blinkenlights.DataFetchers
{
    public interface IDataFetcher<T> where T : IDatabaseData
    {
        public T FetchRemoteData(bool overwrite = false);

		//public T GetLocalData();
    }
}