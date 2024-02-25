using Blinkenlights.Dataschemas;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blinkenlights.DatabaseHandler
{
    public class DatabaseHandler : IDatabaseHandler
    {
        public const string DATABASE_FILENAME = "BlinkenLights_Db_Prod.json";

        private string DatabaseAbsoluteFilePath { get; init; }

        private Mutex Mutex { get; init; }

        private JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)

            }
        };

        public DatabaseHandler(IWebHostEnvironment environment) : this(environment.ContentRootPath)
        {
        }

        public DatabaseHandler(string contentRootPath)
        {
            var pathParts = new string[] { contentRootPath, DATABASE_FILENAME };
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
                return JsonSerializer.Deserialize<Dictionary<string, string>>(stringData, this.JsonSerializerOptions);
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        private bool WriteDatabase(Dictionary<string, string> data)
        {
            try
            {
                var stringData = JsonSerializer.Serialize(data, this.JsonSerializerOptions);
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

        public bool TryRead(string key, out string value)
        {
            var data = LoadDatabase();
            if (data.TryGetValue(key, out value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Write(string key, string value)
        {
            var data = LoadDatabase();
            data[key] = value;
            return WriteDatabase(data);
        }

        public T Get<T>() where T : IDatabaseData
        {
            var key = typeof(T).Name;
            if (!TryRead(key, out var value) || string.IsNullOrWhiteSpace(value))
            {
                return default;
            }

            try
            {
                return JsonSerializer.Deserialize<T>(value, this.JsonSerializerOptions);
            }
            catch (Exception)
            {
                return default;
            }
        }

        public bool Set<T>(T moduleData) where T : IDatabaseData
        {
            var key = moduleData.GetType().Name;
            var data = JsonSerializer.Serialize<T>(moduleData, this.JsonSerializerOptions);
            return Write(key, data);
        }
    }
}