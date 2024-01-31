using Blinkenlights.Models.Api.ApiInfoTypes;

namespace Blinkenlights.Models.ViewModels.Status
{
	public class StatusViewModel : IModuleViewModel
    {
        public string ModuleName => "Status";

        public IEnumerable<ApiType> ModulesToLoad { get; set; }

		public StatusViewModel(IEnumerable<ApiType> modulesToLoad) : base()
		{
			this.ModulesToLoad = modulesToLoad;
		}
	}
}
