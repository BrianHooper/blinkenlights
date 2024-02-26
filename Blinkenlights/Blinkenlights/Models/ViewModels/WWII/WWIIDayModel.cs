using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.WWII
{
    public class WWIIDayModel : ModuleViewModelBase
	{
		public WWIIDayModel() : base("WWII")
		{
		}

		public WWIIDayModel(string date, List<string> globalEvents, List<KeyValuePair<string, List<string>>> regionalEvents, ApiStatus status) : base("WWII", status)
		{
			Date = date;
			GlobalEvents = globalEvents;
			RegionalEvents = regionalEvents;
		}

		public string Date { get; set; }
        public List<string> GlobalEvents { get; set; }
        public List<KeyValuePair<string, List<string>>> RegionalEvents { get; set; }
    }
}