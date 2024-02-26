using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.Time
{
    public class TimeViewModel : ModuleViewModelBase
	{
		public TimeViewModel() : base("Time") { }

		public TimeViewModel(ApiStatus status) : base("Time", status) { }

		public Dictionary<string, int> TimeZoneInfos { get; set; }

        public SortedDictionary<string, string> CountdownInfos { get; set; }
    }
}