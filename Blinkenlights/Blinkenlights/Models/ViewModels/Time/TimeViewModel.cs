using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Time
{
    public class TimeViewModel : ApiResultBase
    {
        public TimeViewModel(ApiStatus status) : base("Time", status) { }

        public Dictionary<string, int> TimeZoneInfos { get; set; }

        public SortedDictionary<string, string> CountdownInfos { get; set; }
    }
}