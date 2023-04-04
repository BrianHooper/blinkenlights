using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Models
{
    public class IndexViewModel : IModuleViewModel
    {
        public List<KeyValuePair<string, string>> ModulePlacementPairs { get; init; }

        public string GridTemplateStyle { get; init; }
    }
}
