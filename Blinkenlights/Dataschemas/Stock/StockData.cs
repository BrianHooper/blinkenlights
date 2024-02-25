using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class StockData : IDatabaseData
    {
        public string Key() => typeof(StockData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public List<FinanceData> FinanceData { get; init; }
    }
}