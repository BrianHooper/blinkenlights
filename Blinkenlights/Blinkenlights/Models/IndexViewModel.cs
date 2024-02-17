using Blinkenlights.Dataschemas;
using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Models
{
    public class IndexViewModel : IModuleViewModel
    {
        public string ModuleName => "Index";

        public List<ModulePlacementData> Modules { get; init; }

        public string GridTemplateStyle { get; init; }
    }
}
