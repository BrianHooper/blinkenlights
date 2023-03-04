using Blinkenlights.Models;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Meh;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
    public class MehTransformer : TransformerBase
	{
		public MehTransformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public async override Task<IModuleViewModel> Transform()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.Meh);
			if (apiResponse is null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh, null, "Api response is null");
                return new MehViewModel(errorStatus);
            }
            else if (string.IsNullOrWhiteSpace(apiResponse.Data))
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh, apiResponse, "Api response is empty");
                return new MehViewModel(errorStatus);
            }

            MehJsonModel model;
            try
            {
                model = JsonConvert.DeserializeObject<MehJsonModel>(apiResponse.Data);
            }
            catch(JsonException)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh, apiResponse, "Error deserializing api response");
                return new MehViewModel(errorStatus);
            }

            var title = model?.Deal?.Title;
            var item = model?.Deal?.Items?.FirstOrDefault();
            var url = model?.Deal?.Url;
            var imageUrl = model?.Deal?.Photos?.FirstOrDefault(i => Path.GetExtension(i)?.ToLower()?.Equals(".gif") != true);
            var price = item != null ? item.Price.ToString() : null;
            if (string.IsNullOrWhiteSpace(title) 
                || string.IsNullOrWhiteSpace(url)
                || string.IsNullOrWhiteSpace(imageUrl)
                || string.IsNullOrWhiteSpace(price))
            {
                var errorStatus = ApiStatus.Failed(ApiType.Meh, apiResponse, "Required data missing in api response");
                return new MehViewModel(errorStatus);
            }

            var status = ApiStatus.Success(ApiType.Meh, apiResponse);
            this.ApiHandler.TryUpdateCache(apiResponse);
            return new MehViewModel(status, $"${price} - {title}", url, imageUrl);
        }
    }
}
