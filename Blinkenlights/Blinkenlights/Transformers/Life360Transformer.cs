using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Life360;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
    public class Life360Transformer : TransformerBase
	{
		public Life360Transformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public async override Task<IModuleViewModel> Transform()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.Life360);
			if (apiResponse == null)
            {
                var status = ApiStatus.Failed(ApiType.Life360, null, "Api response is null");
                return new GenericApiViewModel(null, status);
			}

            Life360JsonModel serverModel;
            try
            {
                serverModel = JsonConvert.DeserializeObject<Life360JsonModel>(apiResponse.Data);
            }
            catch (JsonException)
            {
                var status = ApiStatus.Failed(ApiType.Life360, null, "Exception while deserializing API response");
				return new GenericApiViewModel(null, status);
            }

            var models = serverModel?.Members?.Select(m => Life360Model.Parse(m))?.Where(m => m != null)?.ToList();
            if (models?.Any() == true)
            {
                var viewModel = new Life360ViewModel(models);
                var viewModelStr = JsonConvert.SerializeObject(viewModel);
                var status = ApiStatus.Success(ApiType.Life360, apiResponse);
                this.ApiHandler.TryUpdateCache(apiResponse);
				return new GenericApiViewModel(viewModelStr, status);
            }
            else
            {
                var status = ApiStatus.Failed(ApiType.Life360, apiResponse, "Models list was empty");
				return new GenericApiViewModel(null, status);
            }
        }
	}
}
