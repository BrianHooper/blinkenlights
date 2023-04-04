using Blinkenlights.Data.LiteDb;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.Api.ApiResult;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Time;
using Blinkenlights.Models.ViewModels.WWII;
using Humanizer;
using LiteDbLibrary;
using Newtonsoft.Json;

namespace Blinkenlights.Transformers
{
    public class WWIITransformer : TransformerBase
	{
		public WWIITransformer(IApiHandler apiHandler, ILiteDbHandler liteDbHandler) : base(apiHandler, liteDbHandler)
		{

		}

		public override IModuleViewModel Transform()
		{
            var response = this.ApiHandler.Fetch(ApiType.WWII).Result;
            if (string.IsNullOrWhiteSpace(response?.Data))
			{
				var status = ApiStatus.Failed(ApiType.WWII, null, "Failed to get local data");
				return new WWIIDayModel(null, null, null, status);
			}

            WWIIJsonModel wWIIViewModel = null;
            try
            {
                wWIIViewModel = JsonConvert.DeserializeObject<WWIIJsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var status = ApiStatus.Failed(ApiType.WWII, null, "Failed to deserialize data");
                return new WWIIDayModel(null, null, null, status);
            }

            var now = DateTime.Now;
            var date = now.AddYears(-80);
            var dateStr = date.ToString("d MMM yyyy");
            if (wWIIViewModel?.Days?.TryGetValue(dateStr, out var wWIIDayJsonModel) == true)
            {
                var dateFormatted = String.Format("{0} {1:MMMM}, {2:yyyy}", date.Day.Ordinalize(), date, date);
                var globalEvents = wWIIDayJsonModel.Events.FirstOrDefault(kv => string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).Value;
                var regionalEvents = wWIIDayJsonModel.Events.Where(kv => !string.Equals(kv.Key, "Global", StringComparison.OrdinalIgnoreCase)).ToList();


				var status = ApiStatus.Success(ApiType.WWII, response);
				this.ApiHandler.TryUpdateCache(response);
                return new WWIIDayModel(dateFormatted, globalEvents, regionalEvents, status);
            }
            else
            {
                var status = ApiStatus.Failed(ApiType.WWII, null, "Failed to get data");
                return new WWIIDayModel(null, null, null, status);
            }
        }
    }
}
