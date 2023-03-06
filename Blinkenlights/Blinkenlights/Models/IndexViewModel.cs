namespace Blinkenlights.Models
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
                // Status should always be first
                new ModuleViewModel(name: "StatusRoot", row: 3, col: 3),

                // Row 1
                new ModuleViewModel(name: "TimeRoot", row: 1, col: 1),
                new ModuleViewModel(name: "WeatherRoot", row: 1, col: 2, colSpan: 2),
                new ModuleViewModel(name: "Life360Root", row: 1, col: 4),

                // Row 2
                new ModuleViewModel(name: "WWIIRoot", row: 2, col: 1, colSpan: 2),
                new ModuleViewModel(name: "HeadlinesRoot", row: 2, col: 3, colSpan: 2),

                // Row 3
                new ModuleViewModel(name: "CalendarRoot", row: 3, col: 1),
				new ModuleViewModel(name: "MehRoot", row: 3, col: 2),
				new ModuleViewModel(name: "AstronomyRoot", row: 3, col: 4),
			};

            this.ModulePlacementPairs = modulesToLoad.Select(m => new KeyValuePair<string, string>(m.Name, m.ToGridArea())).ToList();

            var numColumns = modulesToLoad.Max(m => m.ColEnd) - 1;
            var numRows = modulesToLoad.Max(m => m.RowEnd) - 1;


            this.GridTemplateStyle = $"grid-template-columns: repeat({numColumns}, minmax(0, 1fr)); grid-template-rows: repeat({numRows}, minmax(0, 1fr));";
        }
    }
}
