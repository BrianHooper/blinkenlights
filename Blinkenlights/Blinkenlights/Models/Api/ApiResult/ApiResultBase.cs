using Blinkenlights.Dataschemas;
using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Models.Api.ApiResult
{
    public class ApiResultBase : IModuleViewModel
    {
        public string ModuleName { get; init; }

        public string Status { get; init; }

        public ApiResultBase(string moduleName, params ApiStatus[] apiStatuses)
        {
            var validStatuses = apiStatuses.Where(s => !string.IsNullOrEmpty(s?.Name)).ToArray();

            Status = ApiStatusList.Serialize(validStatuses);
            this.ModuleName = moduleName;
        }
    }
}
