using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class StockData : IModuleData
    {
        public string Key() => typeof(StockData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public ApiStatus Status { get; init; }

        public static StockData Clone(StockData other, ApiStatus status)
        {
            return new StockData()
            {
                Status = status,
                TimeStamp = other?.TimeStamp
            };
        }
    }
}