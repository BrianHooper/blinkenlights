using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels.Authentication;
using System.Text.Json;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public enum ApiRestType
    {
        Get = 0,
        Post = 1
    }

    public interface IApiInfo
    {

        public ApiServerType ServerType { get; }

        public ApiRestType ApiRestType { get; }

        public int? DailyRateLimit { get; }

        public TimeSpan Timeout { get; }

        public StringSecretsPair Endpoint(params string[] queryParameters);

        public Dictionary<string, StringSecretsPair> Headers() => null;

        public Dictionary<string, string> Parameters() => null;

        public List<StringSecretsPair> AuthenticationSecrets() => null;

        public List<ApiSecretType> InstanceSecrets => null;

        public InstanceSecret ResponseToInstanceSecret(ApiResponse apiSecretResponse) => null;
    }

    public abstract class ApiInfoBase : IApiInfo
    {
		protected const string PAGE_PARSE_API_HOST = "http://127.0.0.1:5001";
		//protected const string PAGE_PARSE_API_HOST = "http://192.168.1.20:5001";

		public virtual ApiRestType ApiRestType { get; } = ApiRestType.Get;

        public virtual ApiServerType ServerType { get; } = ApiServerType.Unknown;

        public virtual int? DailyRateLimit { get; } = 25;

        public virtual TimeSpan Timeout { get; } = TimeSpan.FromHours(24);

        public abstract StringSecretsPair Endpoint(params string[] queryParameters);

        public virtual Dictionary<string, StringSecretsPair> Headers() => null;

        public virtual Dictionary<string, string> Parameters() => null;

        public virtual List<StringSecretsPair> AuthenticationSecrets() => null;

        public virtual List<ApiSecretType> InstanceSecrets => null;

        public virtual InstanceSecret ResponseToInstanceSecret(ApiResponse apiSecretResponse) => null;
    }

    public class TimeZoneApiInfo : ApiInfoBase
    {
        public override ApiServerType ServerType { get; } = ApiServerType.Local;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(2);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = Path.Combine("DataSources", "TimeZoneInfo.json");
            return new StringSecretsPair(endpoint);
        }
    }

    public class WWIIApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(2);

		public override ApiServerType ServerType { get; } = ApiServerType.Local;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = Path.Combine("DataSources", "WWII_DayByDay.json");
            return new StringSecretsPair(endpoint);
        }
    }

    public class NewYorkTimesApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(1);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://api.nytimes.com/svc/topstories/v2/home.json?api-key={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.NewYorkTimesServiceApiKey);
        }
    }

    public class WeatherApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/98148?unitGroup=us&key={0}&contentType=json";
            return new StringSecretsPair(endpoint, ApiSecretType.VisualCrossingServiceApiKey);
        }
    }

    public class MehApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://meh.com/api/1/current.json?apikey={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.MehServiceApiKey);
        }
    }

    public class WikipediaApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = $"{PAGE_PARSE_API_HOST}/wikipedia";
            return new StringSecretsPair(endpoint);
        }
    }

    public class GoogleCalendarApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = $"{PAGE_PARSE_API_HOST}/googlecalendar";
            return new StringSecretsPair(endpoint);
        }

        public override Dictionary<string, StringSecretsPair> Headers()
        {
            return new Dictionary<string, StringSecretsPair>()
            {
                { "X-user-account", new StringSecretsPair(ApiSecretType.GoogleCalendarUserAccount) },
                { "X-user-secret", new StringSecretsPair(ApiSecretType.GoogleCalendarApiServiceKey) },
            };
        }
    }

    public class RocketLaunchesApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 60;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(1);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = $"{PAGE_PARSE_API_HOST}/rockets";
            return new StringSecretsPair(endpoint);
        }
    }

    public class YCombinatorApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = $"{PAGE_PARSE_API_HOST}/ycombinator";
            return new StringSecretsPair(endpoint);
        }
    }

    public class Life360ApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 200;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromMinutes(5);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://www.life360.com/v3/circles/{0}/members";
            return new StringSecretsPair(endpoint, ApiSecretType.Life360CircleId);
        }

        public override Dictionary<string, StringSecretsPair> Headers()
        {
            return new Dictionary<string, StringSecretsPair>()
            {
                { "Authorization", new StringSecretsPair("Bearer {0}", ApiSecretType.Life360AuthorizationToken) }
            };
        }
    }

    public class AstronomyApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = $"{PAGE_PARSE_API_HOST}/astronomypotd";
            return new StringSecretsPair(endpoint);
        }
    }

    public class UpsPackageTrackingApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 25;
		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override List<ApiSecretType> InstanceSecrets { get; } =
            new List<ApiSecretType>()
            {
                ApiSecretType.UpsTrackingAuthorizationToken
            };

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var trackingNumber = queryParameters?.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s));
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                return null;
            }

            var endpoint = string.Format("https://onlinetools.ups.com/api/track/v1/details/{0}", trackingNumber);
            return new StringSecretsPair(endpoint);
        }

        public override Dictionary<string, StringSecretsPair> Headers()
        {
            var transactionId = DateTime.Now.Ticks.ToString();
            return new Dictionary<string, StringSecretsPair>()
            {
                { "transId", new StringSecretsPair(transactionId) },
                { "transactionSrc", new StringSecretsPair("testing") },
                { "Authorization", new StringSecretsPair("Bearer {0}", ApiSecretType.UpsTrackingAuthorizationToken) }
            };
        }
    }

    public class UpsOAuthApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 25;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override ApiRestType ApiRestType { get; } = ApiRestType.Post;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://onlinetools.ups.com/security/v1/oauth/token";
            return new StringSecretsPair(endpoint);
        }

        public override Dictionary<string, string> Parameters()
        {
            return new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            };
        }

        public override List<StringSecretsPair> AuthenticationSecrets()
        {
            return new List<StringSecretsPair>()
            {
                new StringSecretsPair(ApiSecretType.UpsPackageTrackingOAuthClientId),
                new StringSecretsPair(ApiSecretType.UpsPackageTrackingOAuthSecretId),
            };
        }

        public override InstanceSecret ResponseToInstanceSecret(ApiResponse apiResponse)
        {
            OAuthApiResponse oathResponse;
            try
            {
                oathResponse = JsonSerializer.Deserialize<OAuthApiResponse>(apiResponse.Data);
            }
            catch (Exception)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(oathResponse.AccessToken))
            {
                return new InstanceSecret()
                {
                    Secret = oathResponse.AccessToken,
                    TimeToLiveSeconds = oathResponse.ExpiresIn,
                    DateTimeCreated = DateTime.Now
                };
            }
            else
            {
                return null;
            }
        }
    }

    public class PackageTrackingApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 25;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override ApiRestType ApiRestType { get; } = ApiRestType.Post;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = $"{PAGE_PARSE_API_HOST}/packagetracking";
            return new StringSecretsPair(endpoint);
        }

        public override Dictionary<string, StringSecretsPair> Headers()
        {
            return new Dictionary<string, StringSecretsPair>()
            {
                { "X-ups-client-id", new StringSecretsPair(ApiSecretType.UpsPackageTrackingOAuthClientId) },
                { "X-ups-client-secret", new StringSecretsPair(ApiSecretType.UpsPackageTrackingOAuthSecretId) },
                { "X-fedex-client-id", new StringSecretsPair(ApiSecretType.FedexPackageTrackingOAuthClientId) },
                { "X-fedex-client-secret", new StringSecretsPair(ApiSecretType.FedexPackageTrackingOAuthSecretId) },
            };
        }
    }

    public class Ship24ApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 25;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override ApiRestType ApiRestType { get; } = ApiRestType.Post;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = string.Format("https://api.ship24.com/public/v1/trackers/track");
            return new StringSecretsPair(endpoint);
        }

        public override Dictionary<string, StringSecretsPair> Headers()
        {
            return new Dictionary<string, StringSecretsPair>()
            {
                { "Content-Type", new StringSecretsPair("application/json") },
                { "Authorization", new StringSecretsPair(ApiSecretType.Ship24ApiKey) }
            };
        }
    }

    public class IssTrackerApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 250;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromMinutes(5);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = $"{PAGE_PARSE_API_HOST}/isstracker";
            return new StringSecretsPair(endpoint);
        }
    }

    public class AlphaVantageApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 25;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var symbol = queryParameters?.ElementAtOrDefault(0);

            var endpoint = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=" + symbol + "&interval=60min&apikey={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.FinanceApiKey);
        }
    }

    public class WikiPotdApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 100;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = $"{PAGE_PARSE_API_HOST}/wikipotd";
            return new StringSecretsPair(endpoint);
        }
    }

 //   public class DistanceApiInfo : ApiInfoBase
	//{
	//	public override int? DailyRateLimit { get; } = 200;

	//	public override ApiServerType ServerType { get; } = ApiServerType.Remote;

	//	public override TimeSpan Timeout { get; } = TimeSpan.FromMinutes(5);

	//	public override StringSecretsPair Endpoint(params string[] queryParameters)
 //       {
 //           var lat1str = queryParameters?.ElementAtOrDefault(0);
 //           var long1str = queryParameters?.ElementAtOrDefault(1);
 //           var lat2str = queryParameters?.ElementAtOrDefault(2);
 //           var long2str = queryParameters?.ElementAtOrDefault(3);

 //           var endpoint = string.Format("https://distance-calculator.p.rapidapi.com/distance/simple?lat_1={0}&long_1={1}&lat_2={2}&long_2={3}&unit=miles&decimal_places=2", lat1str, long1str, lat2str, long2str);
 //           return new StringSecretsPair(endpoint);
 //       }

 //       public override Dictionary<string, StringSecretsPair> Headers()
 //       {
 //           return new Dictionary<string, StringSecretsPair>()
 //           {
 //               { "X-RapidAPI-Key", new StringSecretsPair(ApiSecretType.RapidApiKey) },
 //               { "X-RapidAPI-Host", new StringSecretsPair("distance-calculator.p.rapidapi.com") }
 //           };
 //       }
 //   }

    public class FlightAwareApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 200;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromMinutes(15);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            //var endpoint = "https://aeroapi.flightaware.com/aeroapi/flights/search?query=-latlong %2246.80 -123.30 48.00 -121.30%22 -originOrDestination %22KSEA%22";
            var endpoint = string.Empty;
            return new StringSecretsPair(endpoint);
        }

        public override Dictionary<string, StringSecretsPair> Headers()
        {
            return new Dictionary<string, StringSecretsPair>()
            {
                { "x-apikey", new StringSecretsPair(ApiSecretType.FlightAwareApiKey) },
            };
        }
    }

    public class PeopleInSpaceApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 200;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://api.open-notify.org/astros.json";
            return new StringSecretsPair(endpoint);
        }
	}

	public class AlphaVantageCurrencyApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 25;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var from_currency = queryParameters?.ElementAtOrDefault(0);
			var to_currency = queryParameters?.ElementAtOrDefault(1);

			var endpoint = "https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency=" + from_currency + "&to_currency=" + to_currency + "&apikey={0}";
			return new StringSecretsPair(endpoint, ApiSecretType.FinanceApiKey);
		}
	}

	public class RocketLaunchLiveApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 25;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = "https://fdo.rocketlaunch.live/json/launches/next/5";
			return new StringSecretsPair(endpoint);
		}
	}

	public class FlightRadarApiInfo : ApiInfoBase
	{
		public override int? DailyRateLimit { get; } = 10000;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override TimeSpan Timeout { get; } = TimeSpan.FromMinutes(1);

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
            var fid = queryParameters.ElementAtOrDefault(0);
            if (!string.IsNullOrWhiteSpace(fid))
            {
                var endpoint = $"{PAGE_PARSE_API_HOST}/flightradar/{fid}";
                return new StringSecretsPair(endpoint);
            }
            else
            {
                var endpoint = $"{PAGE_PARSE_API_HOST}/flightradar";
                return new StringSecretsPair(endpoint);
            }
        }
    }

    public class BTownApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = $"{PAGE_PARSE_API_HOST}/btown";
            return new StringSecretsPair(endpoint);
        }
    }

    public class NprApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = $"{PAGE_PARSE_API_HOST}/npr";
            return new StringSecretsPair(endpoint);
        }
    }

    public class APNewsApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = $"{PAGE_PARSE_API_HOST}/apnews";
            return new StringSecretsPair(endpoint);
        }
    }

    public class SeattleTimesApiInfo : ApiInfoBase
    {
        public override int? DailyRateLimit { get; } = 25;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override TimeSpan Timeout { get; } = TimeSpan.FromHours(4);

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = $"{PAGE_PARSE_API_HOST}/seattletimes";
            return new StringSecretsPair(endpoint);
        }
    }
}

