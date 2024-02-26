using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiInfoTypes;
using System.Text;

namespace Blinkenlights.ApiHandlers
{
	public class ApiStatusFactory : IApiStatusFactory
	{
		ILogger<ApiStatusFactory> Logger;

		public ApiStatusFactory(ILogger<ApiStatusFactory> logger)
		{
			this.Logger = logger;
		}

		public ApiStatus Failed(ApiType apiType, string statusMessage, DateTime? lastUpdateTime = null, DateTime? nextValidRequestTime = null)
		{
			this.Logger.LogError($"{apiType} API Failed: {statusMessage}");
			return Response(apiType, lastUpdateTime, ApiSource.Error, ApiState.Failed, statusMessage, nextValidRequestTime);
		}

		private static ApiStatus Response(ApiType apiType, DateTime? lastUpdateTime, ApiSource? apiSource, ApiState? apiState, string statusMessage, DateTime? nextValidRequestTime = null)
		{
			var source = apiSource is null ? ApiSource.Unknown : apiSource.Value;
			var state = apiState is null ? ApiState.Unknown : apiState.Value;
			var lastUpdate = lastUpdateTime is null || state == ApiState.Failed ? string.Empty : lastUpdateTime.Value.ToString("hh:mm tt");

			StringBuilder sb = new StringBuilder();
			sb.Append($"[{apiType}] ");
			sb.Append($"State: {state}, ");
			sb.Append($"Source: {source}, ");
			sb.Append($"LastUpdate: {lastUpdate}, ");
			sb.Append($"StatusMessage: {statusMessage}, ");

			return new ApiStatus()
			{
				ApiType = apiType,
				Name = apiType.ToString(),
				LastUpdate = lastUpdateTime,
				NextValidRequestTime = nextValidRequestTime,
				State = state,
				Source = source,
				Status = statusMessage,
			};
		}

		public ApiStatus Success(ApiType apiType, DateTime? lastUpdateTime, ApiSource? source, string statusMessage = "Success")
		{
			this.Logger.LogInformation($"{apiType} API Success: {statusMessage}");
			return Response(apiType, lastUpdateTime, source, ApiState.Success, statusMessage);
		}
	}
}
