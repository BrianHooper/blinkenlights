using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.OuterSpace
{
    public class PeopleInSpaceJsonModel
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("people")]
        public List<PersonInSpace> People { get; set; }
    }

    public class PersonInSpace
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("craft")]
        public string Craft { get; set; }
    }
}
