using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiInfoTypes;

namespace Blinkenlights.ApiHandlers
{
	public interface IApiStatusFactory
	{
		public ApiStatus Failed(ApiType apiType, string statusMessage, DateTime? lastUpdateTime = null, DateTime? nextValidRequestTime = null);

		public ApiStatus Success(ApiType apiType, DateTime? lastUpdateTime, ApiSource? source, string statusMessage = "Success");
	}
}
