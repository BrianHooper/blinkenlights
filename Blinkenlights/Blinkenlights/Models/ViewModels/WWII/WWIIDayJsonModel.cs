namespace Blinkenlights.Models.ViewModels.WWII
{
    public class WWIIDayJsonModel
    {
        public string DateStr { get; set; }

        public Dictionary<string, List<string>> Events { get; set; }
    }
}
