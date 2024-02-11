using System.Text;

namespace Blinkenlights.Dataschemas
{
	public class ApiStatus
	{
		public string Name { get; init; }

		public DateTime? LastUpdate { get; init; }

		public ApiState State { get; init; }

		public ApiSource Source { get; init; }

		public string Status { get; init; }

		private ApiStatus(string apiName, DateTime? lastUpdateTime, ApiSource source, ApiState state, string statusMessage)
		{
			this.Name = apiName;
			this.LastUpdate = lastUpdateTime;
			this.Source = source;
			this.State = state;
			this.Status = statusMessage;
		}

		private static ApiStatus Response(string apiName, DateTime? lastUpdateTime, ApiSource? apiSource, ApiState? apiState, string statusMessage)
		{
			var source = apiSource is null ? ApiSource.Unknown : apiSource.Value;
			var state = apiState is null ? ApiState.Unknown : apiState.Value;
			var lastUpdate = lastUpdateTime is null || state == ApiState.Failed ? string.Empty : lastUpdateTime.Value.ToString("hh:mm tt");

			StringBuilder sb = new StringBuilder();
			sb.Append($"[{apiName}] ");
			sb.Append($"State: {state}, ");
			sb.Append($"Source: {source}, ");
			sb.Append($"LastUpdate: {lastUpdate}, ");
			sb.Append($"StatusMessage: {statusMessage}, ");

			return new ApiStatus(apiName, lastUpdateTime, source, state, sb.ToString());
		}

		public static ApiStatus Success(string apiName, DateTime? lastUpdateTime, ApiSource? source, string statusMessage = "Success")
		{
			return Response(apiName, lastUpdateTime, source, ApiState.Success, statusMessage);
		}

		public static ApiStatus Failed(string apiName, string statusMessage, DateTime? lastUpdateTime = null)
		{
			return Response(apiName, lastUpdateTime, ApiSource.Error, ApiState.Failed, statusMessage);
		}
	}
}
