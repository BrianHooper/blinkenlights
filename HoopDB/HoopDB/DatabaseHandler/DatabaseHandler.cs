using System.Text.Json;

namespace Blinkenlights.HoopDB
{
    public class DatabaseHandler : IDatabaseHandler
    {
        public const string DATABASE_FILENAME = "BlinkenLights_Db_Prod.json";

        private string DatabaseAbsoluteFilePath { get; init; }

        private Mutex Mutex { get; init; }

        public DatabaseHandler(IWebHostEnvironment environment)
        {
            var pathParts = new string[] { environment.ContentRootPath, DATABASE_FILENAME };
            var databaseAbsoluteFilePath = Path.Combine(pathParts);
            this.DatabaseAbsoluteFilePath = databaseAbsoluteFilePath;
            this.Mutex = new Mutex();
        }

        private Dictionary<string, string> LoadDatabase()
        {
            if (!File.Exists(this.DatabaseAbsoluteFilePath))
            {
                return new Dictionary<string, string>();
            }

            try
            {
                Mutex.WaitOne();
                var stringData = File.ReadAllText(this.DatabaseAbsoluteFilePath);
                Mutex.ReleaseMutex();
                return JsonSerializer.Deserialize<Dictionary<string, string>>(stringData);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private bool WriteDatabase(Dictionary<string, string> data)
        {
            try
            {
                var stringData = JsonSerializer.Serialize(data);
                Mutex.WaitOne();
                File.WriteAllText(this.DatabaseAbsoluteFilePath, stringData);
                Mutex.ReleaseMutex();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void Delete(string key)
        {
            throw new NotImplementedException();
        }

        public string Read(string key)
        {
            var data = LoadDatabase();
            if (data.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }

        public bool Write(string key, string value)
        {
            var data = LoadDatabase();
            data[key] = value;
            return WriteDatabase(data);
        }
    }
}
