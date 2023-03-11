using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Time;
using Blinkenlights.Utilities;

namespace Blinkenlights.Transformers
{
    public class TimeTransformer : TransformerBase
	{

		public TimeTransformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var response = this.ApiHandler.Fetch(ApiType.TimeZone).Result;
			if (response is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.TimeZone, null, "Failed to get local data");
				return new TimeViewModel(errorStatus);
			}

			if (!Helpers.TryDeserialize<TimeViewModel>(response.Data, out var serverModel))
            {
				var errorStatus = ApiStatus.Failed(ApiType.TimeZone, null, "Failed to read data");
				return new TimeViewModel(errorStatus);
            }

			var status = ApiStatus.Success(ApiType.TimeZone, response);
			this.ApiHandler.TryUpdateCache(response);
			var viewModel = new TimeViewModel(status)
			{
				TimeZoneInfos = serverModel.TimeZoneInfos,
				CountdownInfos = new SortedDictionary<string, string>()
			{
				{ "2023-03-22", "Ecuador" },
				{ "2023-06-10", "Wedding" },
				{ "2023-07-03", "Portugal" },
				{ "2023-08-27", "Burning Man" },
			}
			};

			return viewModel;
        }
    }
}
