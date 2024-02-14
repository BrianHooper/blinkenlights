using Blinkenlights.Dataschemas;

namespace Blinkenlights.DataFetchers
{
    public interface IDataFetcher<T> where T : IModuleData
    {
        public void Start();

        public void FetchRemoteData(bool overwrite = false);

        public T GetLocalData();
    }
}