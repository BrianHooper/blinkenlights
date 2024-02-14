namespace Blinkenlights.Dataschemas
{
    public class NewYorkTimesModel
    {
        public List<NewYorkTimesResultsModel> results { get; set; }
    }

    public class NewYorkTimesResultsModel
    {
        public string section { get; set; }
        public string subsection { get; set; }
        public string title { get; set; }
        public string @abstract { get; set; }
        public string url { get; set; }
    }
}
