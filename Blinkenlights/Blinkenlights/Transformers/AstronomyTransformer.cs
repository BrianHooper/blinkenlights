using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Astronomy;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
	public class AstronomyTransformer : TransformerBase
	{
		public AstronomyTransformer(IApiHandler apiHandler) : base(apiHandler)
		{
		}

		public async override Task<IModuleViewModel> Transform()
		{
			var apiResponse = await this.ApiHandler.Fetch(ApiType.Astronomy);

			if (apiResponse is null)
			{
				var errorStatus = ApiStatus.Failed(ApiType.Astronomy, null, "API Response is null");
				return new AstronomyViewModel(errorStatus);
			}

			if (string.IsNullOrWhiteSpace(apiResponse.Data))
			{
				var errorStatus = ApiStatus.Failed(ApiType.Astronomy, apiResponse, "API Response data is empty");
				return new AstronomyViewModel(errorStatus);
			}

			if (ApiError.IsApiError(apiResponse.Data, out var errorMessage))
			{
				var errorStatus = ApiStatus.Failed(ApiType.Astronomy, apiResponse, errorMessage);
				return new AstronomyViewModel(errorStatus);
			}

			AstronomyJsonModel astronomyData;
			try
			{
				astronomyData = JsonConvert.DeserializeObject<AstronomyJsonModel>(apiResponse.Data);
			}
			catch (JsonException)
			{
				var errorStatus = ApiStatus.Failed(ApiType.Astronomy, apiResponse, "Exception while deserializing API response");
				return new AstronomyViewModel(errorStatus);
			}

			if (string.IsNullOrWhiteSpace(astronomyData?.Title) 
				|| string.IsNullOrWhiteSpace(astronomyData?.Source) 
				|| string.IsNullOrWhiteSpace(astronomyData?.Url))
			{
				var errorStatus = ApiStatus.Failed(ApiType.Astronomy, apiResponse, "Missing required data from api");
				return new AstronomyViewModel(errorStatus);
			}

			var status = ApiStatus.Success(ApiType.Astronomy, apiResponse);
			this.ApiHandler.TryUpdateCache(apiResponse);
			return new AstronomyViewModel($"NASA Picture of the Day: {astronomyData.Title}", astronomyData.Source, astronomyData.Url, status);
		}
	}
}
