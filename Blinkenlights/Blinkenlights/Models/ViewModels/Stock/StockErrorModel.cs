using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Stock
{
    public class StockErrorModel
    {
        [JsonPropertyName("Information")]
        public string Information { get; set; }
    }
}
