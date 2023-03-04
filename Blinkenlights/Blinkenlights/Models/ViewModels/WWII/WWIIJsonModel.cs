using Newtonsoft.Json.Linq;

namespace Blinkenlights.Models.ViewModels.WWII
{
    public class WWIIJsonModel
    {
        public Dictionary<string, WWIIDayJsonModel> Days { get; set; }
    }
}
