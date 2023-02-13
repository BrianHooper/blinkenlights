using BlinkenLights.Models.ApiCache;
using Newtonsoft.Json;
using RestSharp;

namespace BlinkenLights.Utilities
{
    public class Helpers
    {
        public static DateTime FromEpoch(long epoch, bool useUtc = false, bool addOffset = false)
        {
            var dtKind = useUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, dtKind).AddSeconds(epoch);

            if (addOffset)
            {
                dt.Add(DateTimeOffset.Now.Offset);
            }
            return dt;
        }

        public static string ApiError(string errorMessage)
        {
            return JsonConvert.SerializeObject(new Dictionary<string, string>() { { "Error", errorMessage } });
        }

        public static bool TryDeserialize<T>(string jsonData, out T deserializedObject)
        {
            try
            {
                deserializedObject = JsonConvert.DeserializeObject<T>(jsonData);
                return deserializedObject != null;
            }
            catch (JsonException)
            {
                deserializedObject = default(T);
                return false;
            }
        }
    }
}
