namespace Blinkenlights.Dataschemas
{
    public class PageParseApiModel
    {
        public List<Article> stories { get; set; }
    }

    public class Article
    {
        public string title { get; set; }

        public string url { get; set; }
    }
}
