using System.ComponentModel;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public enum ApiSecretType
    {
        Default = 0,

        [SecretKey("Life360:AuthorizationToken")]
        Life360AuthorizationToken = 1,

        [SecretKey("Life360:CircleId")]
        Life360CircleId = 2,

        [SecretKey("VisualCrossing:ServiceApiKey")]
        VisualCrossingServiceApiKey = 3,

        [SecretKey("Meh:ServiceApiKey")]
        MehServiceApiKey = 4,

        [SecretKey("NewYorkTimes:ServiceApiKey")]
        NewYorkTimesServiceApiKey = 5,

        [SecretKey("GoogleCalendar:UserAccount")]
        GoogleCalendarUserAccount = 6,

		[SecretKey("GoogleCalendar:ApiServiceKey")]
		GoogleCalendarApiServiceKey = 7,

		[SecretKey(key: "PackageTracking:UpsAuthToken", source: ApiType.UpsOath)]
		UpsTrackingAuthorizationToken = 8,

		[SecretKey("UpsPackageTracking:OAuthClientId")]
		UpsPackageTrackingOAuthClientId = 9,

		[SecretKey("UpsPackageTracking:OAuthSecretId")]
		UpsPackageTrackingOAuthSecretId = 10,
	}
}
