using Blinkenlights.Dataschemas;

namespace Blinkenlights.DatabaseHandler
{
    public interface IDatabaseHandler
    {
        public bool Write(string key, string value);

        public bool TryRead(string key, out string value);

        public void Delete(string key);

        public T Get<T>() where T : IModuleData;

        public bool Set<T>(T moduleData) where T : IModuleData;
    }
}
