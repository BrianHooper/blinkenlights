using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Life360
{
    public class Life360JsonModel
    {
        [JsonPropertyName("members")]
        public List<Member> Members { get; set; }
    }

    public class Member
    {
        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Location
    {

        [JsonPropertyName("latitude")]
        [JsonConverter(typeof(DoubleStringConverter))]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        [JsonConverter(typeof(DoubleStringConverter))]
        public double? Longitude { get; set; }

        [JsonPropertyName("timestamp")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Timestamp { get; set; }
    }

    internal class ParseStringConverter : JsonConverter<long>
    {
        public override bool CanConvert(Type t) => t == typeof(long);

        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToString(), options);
            return;
        }
    }


    internal class DoubleStringConverter : JsonConverter<double>
    {
        public override bool CanConvert(Type t) => t == typeof(double);

        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            double l;
            if (Double.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type double");
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToString(), options);
            return;
        }
    }
}