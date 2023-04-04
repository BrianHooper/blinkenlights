using Blinkenlights.Data.LiteDb;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Life360;
using LiteDbLibrary;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
    public class Life360Transformer : TransformerBase
	{
		public Life360Transformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{
		}

		public override IModuleViewModel Transform()
		{
			var response = this.ApiHandler.Fetch(ApiType.Life360).Result;
			if (response == null)
            {
                var status = ApiStatus.Failed(ApiType.Life360, null, "Api response is null");
                return new Life360ViewModel(null, status);
			}
            else if (response.ResultStatus != ApiResultStatus.Success)
            {
				var status = ApiStatus.Failed(ApiType.Life360, response, response.StatusMessage);
				return new Life360ViewModel(null, status);
			}

            Life360JsonModel serverModel;
            try
            {
                serverModel = JsonConvert.DeserializeObject<Life360JsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var status = ApiStatus.Failed(ApiType.Life360, null, "Exception while deserializing API response");
				return new Life360ViewModel(null, status);
            }

            var models = serverModel?.Members?.Select(m => Life360Model.Parse(m))?.Where(m => m != null)?.ToList();
            if (models?.Any() == true)
            {
                var status = ApiStatus.Success(ApiType.Life360, response);
                this.ApiHandler.TryUpdateCache(response);
				return new Life360ViewModel(models, status);
            }
            else
            {
                var status = ApiStatus.Failed(ApiType.Life360, response, "Models list was empty");
				return new Life360ViewModel(null, status);
            }
        }
	}
}
