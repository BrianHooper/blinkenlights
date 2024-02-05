using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Utility
{
	public class UtilityViewModel : ApiResultBase
	{
		public UtilityViewModel(params ApiStatus[] apiStatuses) : base("Utility", apiStatuses)
		{
		}

		public MehViewModel MehData { get; set; }

		public PackageTrackingViewModel PackageTrackingData { get; set; }
    }
}
