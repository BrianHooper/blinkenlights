using Blinkenlights.Models.ViewModels;
using LiteDbLibrary.Schemas;

namespace Blinkenlights.Models
{
    public class IndexViewModel : IModuleViewModel
    {
        public string ModuleName => "Index";

        public List<ModuleItem> Modules { get; init; }

        public string GridTemplateStyle { get; init; }
    }
}
