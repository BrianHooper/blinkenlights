using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Models.Api.ApiResult
{
    public class ApiResultBase : IModuleViewModel
    {
        public string ModuleName { get; init; }

        public string Status { get; init; }

        public ApiResultBase(string moduleName, params ApiStatus[] apiStatuses)
        {
            Status = ApiStatusList.Serialize(apiStatuses);
            this.ModuleName = moduleName;
        }
    }
}
