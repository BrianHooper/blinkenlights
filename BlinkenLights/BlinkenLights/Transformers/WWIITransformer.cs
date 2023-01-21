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

            var date = DateTime.Now.AddYears(-80);
            var dateStr = date.ToString("d MMM yyyy");
            if (wWIIViewModel?.Days?.TryGetValue(dateStr, out var wWIIDayJsonModel) == true)
            {
                var dateFormatted = String.Format("{0} {1:MMMM}, {2:yyyy}", date.Day.Ordinalize(), date, date);
                var globalEvents = wWIIDayJsonModel.Events.FirstOrDefault(kv => string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).Value;
                var regionalEvents = wWIIDayJsonModel.Events.Where(kv => !string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase));

                var dayModel = new WWIIDayModel()
                {
                    Date = dateFormatted,
                    GlobalEvents = globalEvents,
                    RegionalEvents = regionalEvents
                };


                return dayModel;
            }
            else
            {
                return null;
            }
        }
    }
}
