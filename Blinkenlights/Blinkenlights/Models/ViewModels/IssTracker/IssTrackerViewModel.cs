using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.IssTracker
{
    public class IssTrackerViewModel : ModuleViewModelBase
    {
        public string ImagePath { get; set; }

        public string Report { get; set; }

        public IssTrackerViewModel(ApiStatus status) : base("IssTracker", status) { }

        public IssTrackerViewModel(string imagePath, string report, ApiStatus status) : base("IssTracker", status)
        {
            ImagePath = imagePath;
            Report = report;
        }
    }
}
