using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Utility;
using LiteDbLibrary;
using LiteDbLibrary.Schemas;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
    public class UtilityTransformer : TransformerBase
	{
		public UtilityTransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var packageTrackingData = GetPackageTrackingData();
			var mehData = GetMehData();
			return new UtilityViewModel() 
			{ 
				MehData = mehData
			};
        }

		private string GetPackageTrackingData()
		{
			var data = this.LiteDb.Read<PackageTrackingItem>();
			var requestBody = new Dictionary<string, List<PackageTrackingItem>>()
			{
				{ "Items", data }
			};
			var requestBodyStr = JsonConvert.SerializeObject(requestBody);
			var response = this.ApiHandler.Fetch(ApiType.PackageTracking, requestBodyStr).Result;
			return null;
		}

        private MehViewModel GetMehData()
        {
			var response = this.ApiHandler.Fetch(ApiType.Meh).Result;
			if (response is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, null, "Api response is null");
				return new MehViewModel(errorStatus);
			}
			else if (string.IsNullOrWhiteSpace(response.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, response, "Api response is empty");
				return new MehViewModel(errorStatus);
			}

			MehJsonModel model;
			try
			{
				model = JsonConvert.DeserializeObject<MehJsonModel>(response.Data);
			}
			catch (JsonException)
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, response, "Error deserializing api response");
				return new MehViewModel(errorStatus);
			}

			var title = model?.Deal?.Title;
			var item = model?.Deal?.Items?.FirstOrDefault();
			var url = model?.Deal?.Url;
			var imageUrl = model?.Deal?.Photos?.FirstOrDefault(i => Path.GetExtension(i)?.ToLower()?.Equals(".gif") != true);
			var price = item?.Price.ToString();
			if (string.IsNullOrWhiteSpace(title)
				|| string.IsNullOrWhiteSpace(url)
				|| string.IsNullOrWhiteSpace(imageUrl)
				|| string.IsNullOrWhiteSpace(price))
			{
				var errorStatus = ApiStatus.Failed(ApiType.Meh, response, "Required data missing in api response");
				return new MehViewModel(errorStatus);
			}

			var status = ApiStatus.Success(ApiType.Meh, response);
			this.ApiHandler.TryUpdateCache(response);
			return new MehViewModel(status, $"Meh - ${price} - {title}", url, imageUrl);
		}
    }
}
