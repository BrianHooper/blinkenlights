using Blinkenlights.Data.LiteDb;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Time;
using Blinkenlights.Utilities;
using LiteDbLibrary;
using LiteDbLibrary.Schemas;

namespace Blinkenlights.Transformers
{
    public class TimeTransformer : TransformerBase
	{

		public TimeTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var response = this.ApiHandler.Fetch(ApiType.TimeZone).Result;
			if (response is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.TimeZone.ToString(), "Failed to get local data");
				return new TimeViewModel(errorStatus);
			}

			if (!Helpers.TryDeserialize<TimeViewModel>(response.Data, out var serverModel))
            {
				var errorStatus = ApiStatus.Failed(ApiType.TimeZone.ToString(), "Failed to read data");
				return new TimeViewModel(errorStatus);
            }

			var status = ApiStatus.Success(ApiType.TimeZone.ToString(), response.LastUpdateTime, response.ApiSource);
			this.ApiHandler.TryUpdateCache(response);

			var countdownInfos = new SortedDictionary<string, string>();
			this.LiteDb.Read<CountdownItem>()
				.ForEach(item => countdownInfos.Add(item.Date.ToString("yyyy-MM-dd"), item.Name));

			var viewModel = new TimeViewModel(status)
			{
				TimeZoneInfos = serverModel.TimeZoneInfos,
				CountdownInfos = countdownInfos
			};

			return viewModel;
        }
    }
}
