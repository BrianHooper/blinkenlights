using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;
using System.Security.Policy;

namespace Blinkenlights.Models.ViewModels.IssTracker
{
	public class IssTrackerViewModel : ApiResultBase
	{
		public string ImagePath { get; set; }

		public string Report { get; set; }

		public IssTrackerViewModel(ApiStatus status) : base("IssTracker", status) { }

		public IssTrackerViewModel(string imagePath, string report, ApiStatus status) : base("IssTracker", status)
		{
			ImagePath = imagePath;
			Report = report;
		}
	}
}
