using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;
using Newtonsoft.Json;

namespace Blinkenlights.Models.ViewModels.Life360
{
    public class Life360ViewModel : ApiResultBase
    {
        public List<string> Models { get; set; }

        public string UpdateTimeStr { get; set; }

        public Life360ViewModel(List<Life360LocationData> models, ApiStatus status) : base("Life360", status)
        {
            this.Models = models?.Select(x => JsonConvert.SerializeObject(x))?.ToList();
            this.UpdateTimeStr = models?.FirstOrDefault()?.TimeStr ?? string.Empty;
        }
    }
}
