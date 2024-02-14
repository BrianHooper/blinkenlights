using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels.Authentication;
using Newtonsoft.Json;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{
    public enum ApiRestType
    {
        Get = 0,
        Post = 1
    }

    public interface IApiInfo
    {
        public bool ReportedInModule { get; }

        public string ModuleRootName { get; }

        public ApiServerType ServerType { get; }

        public ApiRestType ApiRestType { get; }

        public int? CacheTimeout { get; }

        public StringSecretsPair Endpoint(params string[] queryParameters);

        public Dictionary<string, StringSecretsPair> Headers() => null;

        public Dictionary<string, string> Parameters() => null;

        public List<StringSecretsPair> AuthenticationSecrets() => null;

        public List<ApiSecretType> InstanceSecrets => null;

        public InstanceSecret ResponseToInstanceSecret(ApiResponse apiSecretResponse) => null;
    }

    public abstract class ApiInfoBase : IApiInfo
    {
        public virtual bool ReportedInModule { get; } = false;

        public virtual string ModuleRootName { get; } = null;

        public virtual ApiRestType ApiRestType { get; } = ApiRestType.Get;

        public virtual int? CacheTimeout { get; } = null;

        public virtual ApiServerType ServerType { get; } = ApiServerType.Unknown;

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

        public override string ModuleRootName { get; } = "TimeRoot";

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = Path.Combine("DataSources", "TimeZoneInfo.json");
            return new StringSecretsPair(endpoint);
        }
    }

    public class WWIIApiInfo : ApiInfoBase
    {
        public override ApiServerType ServerType { get; } = ApiServerType.Local;

        public override string ModuleRootName { get; } = "WWIIRoot";

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = Path.Combine("DataSources", "WWII_DayByDay.json");
            return new StringSecretsPair(endpoint);
        }
    }

    public class NewYorkTimesApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "HeadlinesRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://api.nytimes.com/svc/topstories/v2/home.json?api-key={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.NewYorkTimesServiceApiKey);
        }
    }

    public class WeatherApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "WeatherRoot";

        public override int? CacheTimeout { get; } = 60;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/98148?unitGroup=us&key={0}&contentType=json";
            return new StringSecretsPair(endpoint, ApiSecretType.VisualCrossingServiceApiKey);
        }
    }

    public class MehApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "UtilityRoot";

        public override int? CacheTimeout { get; } = 60;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://meh.com/api/1/current.json?apikey={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.MehServiceApiKey);
        }
    }

    public class WikipediaApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "HeadlinesRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://127.0.0.1:5001/wikipedia";
            return new StringSecretsPair(endpoint);
        }
    }

    public class GoogleCalendarApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "CalendarRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://127.0.0.1:5001/googlecalendar";
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
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "HeadlinesRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://127.0.0.1:5001/rockets";
            return new StringSecretsPair(endpoint);
        }
    }

    public class YCombinatorApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "HeadlinesRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://127.0.0.1:5001/ycombinator";
            return new StringSecretsPair(endpoint);
        }
    }

    public class Life360ApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "Life360Root";

        public override int? CacheTimeout { get; } = 2;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

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
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "SlideshowRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://127.0.0.1:5001/astronomypotd";
            return new StringSecretsPair(endpoint);
        }
    }

    public class UpsPackageTrackingApiInfo : ApiInfoBase
    {
        public override int? CacheTimeout { get; } = 120;

        public override string ModuleRootName { get; } = "UtilityRoot";

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
        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override string ModuleRootName { get; } = "UtilityRoot";

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
                oathResponse = JsonConvert.DeserializeObject<OAuthApiResponse>(apiResponse.Data);
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
        public override bool ReportedInModule { get; } = false;

        public override string ModuleRootName { get; } = "UtilityRoot";

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override ApiRestType ApiRestType { get; } = ApiRestType.Post;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://127.0.0.1:5001/packagetracking";
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
        public override bool ReportedInModule { get; } = false;

        public override string ModuleRootName { get; } = "UtilityRoot";

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override ApiRestType ApiRestType { get; } = ApiRestType.Post;

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
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "IssTrackerRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "http://127.0.0.1:5001/isstracker";
            return new StringSecretsPair(endpoint);
        }
    }

    public class AlphaVantageApiInfo : ApiInfoBase
    {
        public override bool ReportedInModule { get; } = true;

        public override string ModuleRootName { get; } = "StockRoot";

        public override int? CacheTimeout { get; } = 120;

        public override ApiServerType ServerType { get; } = ApiServerType.Remote;

        public override StringSecretsPair Endpoint(params string[] queryParameters)
        {
            var endpoint = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol=MSFT&interval=60min&apikey={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.FinanceApiKey);
        }
    }

	public class WikiPotdApiInfo : ApiInfoBase
	{
		public override bool ReportedInModule { get; } = true;

		public override string ModuleRootName { get; } = "SlideshowRoot";

		public override int? CacheTimeout { get; } = 120;

		public override ApiServerType ServerType { get; } = ApiServerType.Remote;

		public override StringSecretsPair Endpoint(params string[] queryParameters)
		{
			var endpoint = "http://127.0.0.1:5001/wikipotd";
			return new StringSecretsPair(endpoint);
		}
	}
}

