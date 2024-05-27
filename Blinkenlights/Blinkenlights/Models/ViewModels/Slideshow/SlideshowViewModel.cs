using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.Slideshow
{
    public class SlideshowViewModel : ModuleViewModelBase
	{
		public SlideshowViewModel() : base("Slideshow")
		{
		}

		public SlideshowViewModel(List<SlideshowFrame> frames) : base("Slideshow")
		{
			this.Frames = frames;
		}

		public SlideshowViewModel(ApiStatus status) : base("Slideshow", status)
		{
		}

		public List<SlideshowFrame> Frames { get; set; }
	}
}
