using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Meh
{
    public class MehViewModel : ApiResultBase
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public MehViewModel(ApiStatus status, string title = null, string url = null, string imageUrl = null) : base(status)
        {
            Title = title;
            Url = url;
            ImageUrl = imageUrl;
        }
    }
}
