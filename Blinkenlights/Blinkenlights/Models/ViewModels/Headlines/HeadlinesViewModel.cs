using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.Headlines
{
    public class HeadlinesViewModel : ModuleViewModelBase
    {
        public List<HeadlinesContainer> Headlines { get; set; }

		public HeadlinesViewModel() : base("Headlines")
		{
		}

		public HeadlinesViewModel(List<HeadlinesContainer> headlines) : base("Headlines")
        {
            Headlines = headlines;
        }
    }
}
