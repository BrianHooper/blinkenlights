using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.Utility
{
    public class UtilityViewModel : ModuleViewModelBase
    {
        public UtilityViewModel(params ApiStatus[] apiStatuses) : base("Utility", apiStatuses)
        {
        }

        public MehData MehData { get; set; }

        public PackageTrackingData PackageTrackingData { get; set; }

        public Life360UtilityModel Life360Data { get; set; }
    }
}
