using Newtonsoft.Json;

namespace Blinkenlights.Models.Api.ApiResult
{
	public class ApiStatusList
    {
        public List<ApiStatus> Items { get; }

        private ApiStatusList(List<ApiStatus> items)
        {
            Items = items;
        }

        public static string Serialize(params ApiStatus[] statusItems)
        {
            if (statusItems?.Any() != true)
            {
                return string.Empty;
            }

            var apiStatusList = new ApiStatusList(statusItems.ToList());
            return JsonConvert.SerializeObject(apiStatusList);
        }
    }
}
