using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Transformers;
using System.Xml.Linq;

namespace Blinkenlights.Models.Api.ApiInfoTypes
{

    public interface IApiInfo
    {
        public ApiServerType ServerType { get; }

        public int CacheTimeout { get; }

        public StringSecretsPair Endpoint();

        public Dictionary<string, StringSecretsPair> Headers() => null;
    }

    public class TimeZoneApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 120;

        ApiServerType IApiInfo.ServerType => ApiServerType.Local;

        public StringSecretsPair Endpoint()
        {
            var endpoint = Path.Combine("DataSources", "TimeZoneInfo.json");
            return new StringSecretsPair(endpoint);
        }
    }

    public class WWIIApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 120;

        ApiServerType IApiInfo.ServerType => ApiServerType.Local;

        public StringSecretsPair Endpoint()
        {
            var endpoint = Path.Combine("DataSources", "WWII_DayByDay.json");
            return new StringSecretsPair(endpoint);
        }
    }

    public class NewYorkTimesApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 120;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        public StringSecretsPair Endpoint()
        {
            var endpoint = "https://api.nytimes.com/svc/topstories/v2/home.json?api-key={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.NewYorkTimesServiceApiKey);
        }
    }

    public class WeatherApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 60;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        public StringSecretsPair Endpoint()
        {
            var endpoint = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/98148?unitGroup=us&key={0}&contentType=json";
            return new StringSecretsPair(endpoint, ApiSecretType.VisualCrossingServiceApiKey);
        }
    }

    public class MehApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 60;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        public StringSecretsPair Endpoint()
        {
            var endpoint = "https://meh.com/api/1/current.json?apikey={0}";
            return new StringSecretsPair(endpoint, ApiSecretType.MehServiceApiKey);
        }
    }

    public class WikipediaApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 120;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        public StringSecretsPair Endpoint()
        {
            var endpoint = "http://127.0.0.1:5001/wikipedia";
            return new StringSecretsPair(endpoint);
        }
    }

    public class GoogleCalendarApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 120;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        StringSecretsPair IApiInfo.Endpoint()
        {
            var endpoint = "http://127.0.0.1:5001/googlecalendar";
            return new StringSecretsPair(endpoint);
        }

        Dictionary<string, StringSecretsPair> IApiInfo.Headers()
        {
            return new Dictionary<string, StringSecretsPair>()
            {
                { "X-user-account", new StringSecretsPair(ApiSecretType.GoogleCalendarUserAccount) },
                { "X-user-secret", new StringSecretsPair(ApiSecretType.GoogleCalendarApiServiceKey) },
            };
        }
    }

    public class RocketLaunchesApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 120;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        public StringSecretsPair Endpoint()
        {
            var endpoint = "http://127.0.0.1:5001/rockets";
            return new StringSecretsPair(endpoint);
        }
    }

    public class YCombinatorApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 120;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        public StringSecretsPair Endpoint()
        {
            var endpoint = "http://127.0.0.1:5001/ycombinator";
            return new StringSecretsPair(endpoint);
        }
    }

    public class Life360ApiInfo : IApiInfo
    {
        int IApiInfo.CacheTimeout => 2;

        ApiServerType IApiInfo.ServerType => ApiServerType.Remote;

        public StringSecretsPair Endpoint()
        {
            var endpoint = "https://www.life360.com/v3/circles/{0}/members";
            return new StringSecretsPair(endpoint, ApiSecretType.Life360CircleId);
        }

        Dictionary<string, StringSecretsPair> IApiInfo.Headers()
        {
            return new Dictionary<string, StringSecretsPair>()
            {
                { "Authorization", new StringSecretsPair("Bearer {0}", ApiSecretType.Life360AuthorizationToken) }
            };
        }
    }
}
