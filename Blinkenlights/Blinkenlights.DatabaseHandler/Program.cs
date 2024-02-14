using Blinkenlights.DatabaseHandler.WWII;
using System.Text.Json;

namespace Blinkenlights.DatabaseHandler
{
    public class Program
    {
        //private IDatabaseHandler DatabaseHandler;

        public Program()
        {
            var rootPath = "../../Blinkenlights/Blinkenlights";
            var fullDatabaseRoot = Path.GetFullPath(rootPath);
            //this.DatabaseHandler = new DatabaseHandler(fullDatabaseRoot);
        }

        private void IngestWWIIData()
        {
            var WWIIDataPath = Path.GetFullPath("DataSources/WWII/WWII_DayByDay.json");
            if (!File.Exists(WWIIDataPath))
            {
                return;
            }

            var data = File.ReadAllText(WWIIDataPath);
            WWIIJsonModel wwiiData = JsonSerializer.Deserialize<WWIIJsonModel>(data);
        }

        public void Run()
        {
            IngestWWIIData();
        }

        public static void Main(string[] args)
        {
            new Program().Run();
        }
    }
}