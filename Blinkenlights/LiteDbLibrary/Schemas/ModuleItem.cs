namespace LiteDbLibrary.Schemas
{
    public class ModuleItem : ILiteDbObject
    {
        public int Id { get; set; }

        public string Name { get; init; }
        public string Endpoint { get; init; }
        public int RefreshRateMs { get; init; }
        public int RowStart { get; init; }
        public int RowEnd { get; init; }
        public int ColStart { get; init; }
        public int ColEnd { get; init; }
        public string GridStyle { get; init; }

        public ModuleItem(string name, string endpoint, int refreshRateMs, int row, int col, int colSpan = 1, int rowSpan = 1)
        {
            Name = name;
            Endpoint = endpoint;
            RefreshRateMs = refreshRateMs;

            RowStart = row;
            RowEnd = RowStart + rowSpan;
            ColStart = col;
            ColEnd = ColStart + colSpan;

            GridStyle = $"grid-area: {RowStart} / {ColStart} / {RowEnd} / {ColEnd};";
        }

        public bool SameIndex(ILiteDbObject other)
        {
            return string.Equals(this.Name, (other as ModuleItem).Name);
        }
    }
}
