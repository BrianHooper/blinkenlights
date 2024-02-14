using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Slideshow
{
	public class SlideshowViewModel : ApiResultBase
	{
		public SlideshowViewModel(List<SlideshowFrame> frames) : base("Slideshow", frames.Select(h => h.Status).ToArray())
		{
			this.Frames = frames;
		}

		public SlideshowViewModel(ApiStatus status) : base("Slideshow", status)
		{
		}

		public List<SlideshowFrame> Frames { get; set; }
	}
}
