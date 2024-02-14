using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Headlines
{
    public class HeadlinesViewModel : ApiResultBase
    {
        public List<HeadlinesContainer> Headlines { get; set; }

        public HeadlinesViewModel(List<HeadlinesContainer> headlines) : base("Headlines", headlines.Select(h => h.Status).ToArray())
        {
            Headlines = headlines;
        }

        public HeadlinesViewModel(ApiStatus status) : base("Headlines", status)
        {

        }
    }
}
