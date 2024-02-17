using System.Text.Json;

namespace Blinkenlights.Utilities
{
    public class Helpers
    {
        public static DateTime FromEpoch(long epoch, bool useUtc = false, bool addOffset = false, double? tzOffset = null)
        {
            var dtKind = useUtc ? DateTimeKind.Utc : DateTimeKind.Local;
            var dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, dtKind).AddSeconds(epoch);

            if (addOffset && tzOffset.HasValue)
            {
                var offset = tzOffset is null ? DateTimeOffset.Now.Offset.Hours : tzOffset;
                dt = dt.AddHours(offset.Value);
            }
            return dt;
        }

        public static string ApiError(string errorMessage)
        {
            return JsonSerializer.Serialize(new Dictionary<string, string>() { { "Error", errorMessage } });
        }

        public static bool TryDeserialize<T>(string jsonData, out T deserializedObject)
        {
            try
            {
                deserializedObject = JsonSerializer.Deserialize<T>(jsonData);
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
