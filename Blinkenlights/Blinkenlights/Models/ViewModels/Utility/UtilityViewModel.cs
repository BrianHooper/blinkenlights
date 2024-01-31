namespace Blinkenlights.Models.ViewModels.Utility
{
	public class UtilityViewModel : IModuleViewModel
	{
		public MehViewModel MehData { get; set; }

		public PackageTrackingViewModel PackageTrackingData { get; set; }

		public string ModuleName => "Utility";
    }
}
