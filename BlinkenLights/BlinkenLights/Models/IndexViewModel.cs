namespace BlinkenLights.Models
{
    public class ModuleViewModel
    {
        public string Name { get; init; }
        public int RowStart { get; init; }
        public int RowEnd { get; init; }
        public int ColStart { get; init; }
        public int ColEnd { get; init; }

        public ModuleViewModel(string name, int row, int col, int colSpan = 1, int rowSpan = 1)
        {
            Name = name;
            RowStart = row;
            RowEnd = RowStart + rowSpan;
            ColStart = col;
            ColEnd = ColStart + colSpan;
        }

        public string ToGridArea()
        {
            return $"grid-area: {RowStart} / {ColStart} / {RowEnd} / {ColEnd};";
        }
    }

    public class IndexViewModel
    {
        public List<KeyValuePair<string, string>> ModulePlacementPairs { get; init; }

        public string GridTemplateStyle { get; init; }
        public IndexViewModel()
        {
            List<ModuleViewModel> modulesToLoad = new List<ModuleViewModel>()
            {
                // Column 1
                new ModuleViewModel(name: "WWIIRoot", row: 1, col: 1, rowSpan: 3),

                // Column 2
                new ModuleViewModel(name: "WeatherRoot", row: 1, col: 2, colSpan: 2),
                new ModuleViewModel(name: "WorldClockRoot", row: 2, col: 2),
                new ModuleViewModel(name: "CountdownRoot", row: 3, col: 2),

                // Column 3
                new ModuleViewModel(name: "MehRoot", row: 2, col: 3),
                //new ModuleViewModel(name: "MehRoot", row: 3, col: 3),

                // Column 4
                new ModuleViewModel(name: "Life360Root", row: 1, col: 4),
                new ModuleViewModel(name: "WikipediaRoot", row: 2, col: 4),
            };
            this.ModulePlacementPairs = modulesToLoad.Select(m => new KeyValuePair<string, string>(m.Name, m.ToGridArea())).ToList();

            var numColumns = modulesToLoad.Max(m => m.ColEnd) - 1;
            var numRows = modulesToLoad.Max(m => m.RowEnd) - 1;


            this.GridTemplateStyle = $"grid-template-columns: repeat({numColumns}, minmax(0, 1fr)); grid-template-rows: repeat({numRows}, minmax(0, 1fr));";
        }
    }
}
