using Blinkenlights.Models.ApiCache;
using BlinkenLights.Models.WWII;
using Humanizer;
using Newtonsoft.Json;

namespace BlinkenLights.Transformers
{
    public class WWIITransformer
    {
        public static WWIIDayModel GetWWIIViewModel(IWebHostEnvironment webHostEnvironment)
        {
            string path = Path.Combine(webHostEnvironment.WebRootPath, "DataSources", "WWII_DayByDay.json");
            var stringData = System.IO.File.ReadAllText(path);
            WWIIJsonModel wWIIViewModel = null;
            try
            {
                wWIIViewModel = JsonConvert.DeserializeObject<WWIIJsonModel>(stringData);
            }
            catch (JsonException)
            {
                return null;
            }

            var now = DateTime.Now;
            var date = now.AddYears(-80);
            var dateStr = date.ToString("d MMM yyyy");
            if (wWIIViewModel?.Days?.TryGetValue(dateStr, out var wWIIDayJsonModel) == true)
            {
                var dateFormatted = String.Format("{0} {1:MMMM}, {2:yyyy}", date.Day.Ordinalize(), date, date);
                var globalEvents = wWIIDayJsonModel.Events.FirstOrDefault(kv => string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).Value;
                var regionalEvents = wWIIDayJsonModel.Events.Where(kv => !string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).ToList();

                return new WWIIDayModel()
                {
                    Date = dateFormatted,
                    GlobalEvents = globalEvents,
                    RegionalEvents = regionalEvents,
                    Status = ApiStatus.Serialize(
                        name: "WWII On this day",
                        key: "WWII",
                        status: "Up to date",
                        lastUpdate: now.ToString(),
                        state: ApiState.Good)
                };
            }
            else
            {
                return new WWIIDayModel()
                {
                    Date = null,
                    GlobalEvents = null,
                    RegionalEvents = null,
                    Status = ApiStatus.Serialize(
                        name: "WWII On this day",
                        key: "WWII",
                        status: "Failed to get data",
                        lastUpdate: now.ToString(),
                        state: ApiState.Error)
                };
            }
        }
    }
}
