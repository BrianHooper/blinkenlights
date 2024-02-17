using Blinkenlights.Models.ViewModels.Weather;
using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Authentication
{
    public class OAuthApiResponse
    {
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("issued_at")]
        public string IssuedAt { get; set; }

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonPropertyName("refresh_count")]
        public long RefreshCount { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
