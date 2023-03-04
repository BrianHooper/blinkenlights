using Newtonsoft.Json;
using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels
{
    public class GenericApiViewModel : ApiResultBase
    {
        public string ApiData { get; }

        public GenericApiViewModel(string apiData, ApiStatus status) : base(status)
        {
            ApiData = apiData;
        }
    }
}
