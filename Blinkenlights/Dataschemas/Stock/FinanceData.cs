using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class FinanceData
    {
        public string Symbol { get; set; }

        public string Price { get; set; }

        public GraphDataPoint[] DataPoints { get; init; }

        public ApiStatus Status { get; set; }

        public string SerializePoints()
        {
            return JsonSerializer.Serialize(this.DataPoints);
        }

        public static FinanceData Clone(FinanceData other, ApiStatus status)
        {
            return new FinanceData()
            {
                Symbol = other?.Symbol,
                Price = other?.Price,
                DataPoints = other?.DataPoints,
                Status = status
            };
        }
    }
}