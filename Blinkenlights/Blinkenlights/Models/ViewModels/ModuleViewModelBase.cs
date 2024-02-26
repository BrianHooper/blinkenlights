using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels
{
    public class ModuleViewModelBase : IModuleViewModel
    {
        public string ModuleName { get; init; }

        public string Status { get; init; }

        public ModuleViewModelBase(string moduleName, params ApiStatus[] apiStatuses)
        {
            var validStatuses = apiStatuses.Where(s => s != null).ToArray();

            Status = ApiStatusList.Serialize(validStatuses);
            ModuleName = moduleName;
        }
    }
}
