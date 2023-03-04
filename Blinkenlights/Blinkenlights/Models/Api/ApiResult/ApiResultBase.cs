using Blinkenlights.Models.ViewModels;

namespace Blinkenlights.Models.Api.ApiResult
{

	public class ApiResultBase : IModuleViewModel
	{
        public string Status { get; init; }

        public ApiResultBase(params ApiStatus[] apiStatuses)
        {
            Status = ApiStatusList.Serialize(apiStatuses);
        }
    }
}
