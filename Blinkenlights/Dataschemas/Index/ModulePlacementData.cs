namespace Blinkenlights.Dataschemas
{
    public class ModulePlacementData
    {
        public ModulePlacementData(string name, string endpoint, int refreshRateMs, int rowStart, int rowEnd, int colStart, int colEnd, string gridStyle)
        {
            Name = name;
            Endpoint = endpoint;
            RefreshRateMs = refreshRateMs;
            RowStart = rowStart;
            RowEnd = rowEnd;
            ColStart = colStart;
            ColEnd = colEnd;
            GridStyle = gridStyle;
        }

        public static ModulePlacementData Create(string name, string endpoint, int refreshRateMs, int row, int col, int colSpan = 1, int rowSpan = 1)
        {
            var RowStart = row;
            var RowEnd = RowStart + rowSpan;
            var ColStart = col;
            var ColEnd = ColStart + colSpan;

            var GridStyle = $"grid-area: {RowStart} / {ColStart} / {RowEnd} / {ColEnd};";

            return new ModulePlacementData(name, endpoint, refreshRateMs, RowStart, RowEnd, ColStart, ColEnd, GridStyle);
        }

        public string Name { get; init; }
        public string Endpoint { get; init; }
        public int RefreshRateMs { get; init; }
        public int RowStart { get; init; }
        public int RowEnd { get; init; }
        public int ColStart { get; init; }
        public int ColEnd { get; init; }
        public string GridStyle { get; init; }
    }
}
