using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Utility
{
	public class Package
	{
		public string Name { get; set; }

		public string Provider { get; set; }

		public string Status { get; set; }

		public DateTime Eta { get; set; }

		public string Url { get; set; }
	}

	public class PackageTrackingViewModel : ApiResultBase
	{
		public List<Package> Packages { get; set; }

		public PackageTrackingViewModel() : base(null)
		{

		}

		public PackageTrackingViewModel(List<Package> packages) : base(null)
		{
			this.Packages = packages;
		}
	}
}
