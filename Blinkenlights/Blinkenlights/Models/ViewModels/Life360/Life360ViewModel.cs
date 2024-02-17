using Blinkenlights.Dataschemas;
using System.Text.Json;

namespace Blinkenlights.Models.ViewModels.Life360
{
    public class Life360ViewModel : ModuleViewModelBase
    {
        public List<string> Models { get; set; }

        public string UpdateTimeStr { get; set; }

        public Life360ViewModel(List<Life360LocationData> models, ApiStatus status) : base("Life360", status)
        {
            this.Models = models?.Select(x => JsonSerializer.Serialize(x))?.ToList();
            this.UpdateTimeStr = models?.FirstOrDefault()?.TimeStr ?? string.Empty;
        }
    }
}
