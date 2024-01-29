using System.ComponentModel;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public enum ApiSecretType
    {
		// dotnet user-secrets set "<SecretKey>" "<Secret>"
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

		[SecretKey("FedexPackageTracking:OAuthClientId")]
		FedexPackageTrackingOAuthClientId = 11,

		[SecretKey("FedexPackageTracking:OAuthSecretId")]
		FedexPackageTrackingOAuthSecretId = 12,

		[SecretKey("Ship24:ApiKey")]
		Ship24ApiKey = 13,

		[SecretKey("FinanceAnswer:AlphaVantage")]
		FinanceApiKey = 14,
	}
}
