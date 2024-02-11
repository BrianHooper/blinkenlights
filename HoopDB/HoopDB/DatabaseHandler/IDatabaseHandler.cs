namespace Blinkenlights.HoopDB
{
    public interface IDatabaseHandler
    {
        public bool Write(string key, string value);

        public string Read(string key);

        public void Delete(string key);
    }
}
