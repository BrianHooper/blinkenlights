using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.FlightStatus
{
	public class FlightStatusViewModel : ModuleViewModelBase
	{
		public List<FlightData> Flights { get; set; }

		public FlightStatusViewModel(params ApiStatus[] statuses) : base("FlightStatus", statuses)
		{
		}
	}
}
