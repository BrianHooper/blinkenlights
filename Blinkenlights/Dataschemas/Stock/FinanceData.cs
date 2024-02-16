using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class FinanceData
    {
        public string Symbol { get; set; }

        public string Price { get; set; }

        public GraphDataPoint[] DataPoints { get; init; }

        public string SerializePoints()
        {
            return JsonSerializer.Serialize(this.DataPoints);
        }
    }
}