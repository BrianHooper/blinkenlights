using BlinkenLights.Modules.WorldClock;
using Newtonsoft.Json;

namespace BlinkenLights.Transformers
{
    public class WorldClockTransformer
    {
        public static WorldClockViewModel GetWorldClockViewModel(IWebHostEnvironment webHostEnvironment)
        {
            string path = Path.Combine(webHostEnvironment.WebRootPath, "DataSources", "TimeZoneInfo.json");
            var stringData = File.ReadAllText(path);
            WorldClockViewModel viewModel = null;
            try
            {
                viewModel = JsonConvert.DeserializeObject<WorldClockViewModel>(stringData);
            }
            catch (JsonException)
            {
                return null;
            }

            viewModel.CountdownInfos = new SortedDictionary<string, string>()
            {
                { "2023-03-22", "Ecuador" },
                { "2023-06-10", "Wedding" },
                { "2023-07-03", "Portugal" },
                { "2023-08-27", "Burning Man" },
            };

            return viewModel;
        }
    }
}
