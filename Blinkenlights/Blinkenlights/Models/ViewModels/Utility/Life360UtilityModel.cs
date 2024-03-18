using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.Utility
{
    public class Life360UtilityModel
    {
        public ApiStatus Status { get; set; }

        public string Distance { get; set; }

        public string TimeDelta { get; set; }
    }
}
