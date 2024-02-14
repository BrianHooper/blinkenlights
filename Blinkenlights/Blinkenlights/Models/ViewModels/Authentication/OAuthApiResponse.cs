using Blinkenlights.Models.ViewModels.Weather;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Authentication
{
    public class OAuthApiResponse
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("issued_at")]
        public string IssuedAt { get; set; }

        [JsonProperty("client_id")]
        public string ClientId { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("expires_in")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long ExpiresIn { get; set; }

        [JsonProperty("refresh_count")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long RefreshCount { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
