using Blinkenlights.Dataschemas;
using System.Buffers.Text;
using System.Buffers;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blinkenlights.Models.Api.ApiResult
{
    public class ApiStatusList
    {
        public List<ApiStatus> Items { get; }

        private ApiStatusList(List<ApiStatus> items)
        {
            Items = items;
        }

        public static string Serialize(params ApiStatus[] statusItems)
        {
            if (statusItems?.Any() != true)
            {
                return string.Empty;
            }
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeConverterForCustomStandardFormatR());

            var apiStatusList = new ApiStatusList(statusItems.ToList());
            return JsonSerializer.Serialize(apiStatusList, options);
        }
    }

    public class DateTimeConverterForCustomStandardFormatR : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));

            if (Utf8Parser.TryParse(reader.ValueSpan, out DateTime value, out _, 'R'))
            {
                return value;
            }

            throw new FormatException();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            var data = value.ToShortTimeString();
            writer.WriteStringValue(data);
        }
    }
}
