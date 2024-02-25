using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.OuterSpace
{
    public class OuterSpaceViewModel : ModuleViewModelBase
    {
        public string ImagePath { get; set; }

        public string Report { get; set; }

        public OuterSpaceViewModel(ApiStatus status) : base("OuterSpace", status) { }

        public OuterSpaceViewModel(string imagePath, string report, ApiStatus status) : base("OuterSpace", status)
        {
            ImagePath = imagePath;
            Report = report;
        }
    }
}
