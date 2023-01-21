namespace BlinkenLights.Models
{
    public class IndexViewModel
    {
        private List<(string ModuleName, int RowSpan, int ColSpan)> ModulesToLoad = new List<(string ModuleName, int RowSpan, int ColSpan)>()
        {
            ("WWIIRoot" , 2 , 1),
        };

        public List<KeyValuePair<string, string>> ModulePlacementPairs { get; set; }

        public string GridTemplateStyle { get; set; }

        private const int NumColumns = 3;

        private const int NumRows = 2;
        public IndexViewModel()
        {
            this.ModulePlacementPairs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("WWIIRoot", " grid-area: 1 / 1 / 3 / 2;"),
                new KeyValuePair<string, string>("WorldClockRoot", "  grid-area:  1 / 3 / 2 / 4;"),
                //new KeyValuePair<string, string>("life360Root", " grid-area: 2 / 3 / 3 / 4;"),
                new KeyValuePair<string, string>("WeatherRoot", " grid-area: 1 / 2 / 2 / 3;"),
                new KeyValuePair<string, string>("CountdownRoot", " grid-area: 2 / 2 / 3 / 3;")
            };
            this.GridTemplateStyle = $"grid-template-columns: repeat({NumColumns}, 1fr); grid-template-rows: repeat({NumRows}, 1fr);";
        }
    }
}
