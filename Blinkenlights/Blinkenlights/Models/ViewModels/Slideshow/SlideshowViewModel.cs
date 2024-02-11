using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Slideshow
{
	public class SlideshowViewModel : ApiResultBase
	{
		public SlideshowViewModel(List<SlideshowFrame> frames, ApiStatus status): base("Slideshow", status)
		{
			this.Frames = frames;
		}

		public SlideshowViewModel(ApiStatus status) : base("Slideshow", status)
		{
		}

		public List<SlideshowFrame> Frames { get; set; }
	}

	public class SlideshowFrame
	{
		public SlideshowFrame(string title, string source, string url)
		{
			Title = title;
			Source = source;
			Url = url;
		}

		public string Title { get; set; }

		public string Source { get; set; }

		public string Url { get; set; }
	}
}
