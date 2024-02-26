using Blinkenlights.Models.Api.ApiInfoTypes;
using System.Text;

namespace Blinkenlights.Dataschemas
{
    public class ApiStatus
	{
		public ApiType ApiType { get; init; }
		
        public string Name { get; init; }

		public DateTime? LastUpdate { get; init; }

		public DateTime? NextValidRequestTime { get; init; }

		public ApiState State { get; init; }

        public ApiSource Source { get; init; }

        public string Status { get; init; }
    }
}
