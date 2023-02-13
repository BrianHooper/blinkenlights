using Blinkenlights.Models.ApiCache;
using Blinkenlights.Models.ApiResult;
using BlinkenLights.Models.ApiCache;
using BlinkenLights.Models.WWII;
using Humanizer;
using Newtonsoft.Json;

namespace BlinkenLights.Transformers
{
    public class WWIITransformer
    {
        private const ApiType apiType = ApiType.WWII;

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
                var status = ApiStatus.Failed(apiType, null, "Failed to deserialize data");
                return new WWIIDayModel(null, null, null, status);
            }

            var now = DateTime.Now;
            var date = now.AddYears(-80);
            var dateStr = date.ToString("d MMM yyyy");
            if (wWIIViewModel?.Days?.TryGetValue(dateStr, out var wWIIDayJsonModel) == true)
            {
                var dateFormatted = String.Format("{0} {1:MMMM}, {2:yyyy}", date.Day.Ordinalize(), date, date);
                var globalEvents = wWIIDayJsonModel.Events.FirstOrDefault(kv => string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).Value;
                var regionalEvents = wWIIDayJsonModel.Events.Where(kv => !string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).ToList();

                var apiResponse = new ApiResponse(apiType, null, ApiSource.Cache, now);
                var status = ApiStatus.Success(apiType, apiResponse);
                return new WWIIDayModel(dateFormatted, globalEvents, regionalEvents, status);
            }
            else
            {
                var status = ApiStatus.Failed(apiType, null, "Failed to get data");
                return new WWIIDayModel(null, null, null, status);
            }
        }
    }
}
