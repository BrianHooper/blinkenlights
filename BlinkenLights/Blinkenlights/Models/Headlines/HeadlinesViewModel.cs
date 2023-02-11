namespace BlinkenLights.Models.Headlines
{
    public class HeadlinesViewModel
    {
        public List<HeadlinesArticle> WikipediaInTheNews { get; set; }

        public List<HeadlinesArticle> NewYorkTimesFrontPageUs { get; set; }

        public List<HeadlinesArticle> NewYorkTimesFrontPageWorld { get; set; }
    }

    public class HeadlinesArticle
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public HeadlinesArticle(NewYorkTimesResultsModel article)
        {
            this.Url = article.url;
            
            if (!string.IsNullOrWhiteSpace(article.article_abstract))
            {
                this.Title = $"{article.title} - {article.article_abstract}";
            }
            else
            {
                this.Title = article.title;
            }
        }

        public HeadlinesArticle(string wikipediaArticle)
        {
            this.Title = wikipediaArticle;
        }
    }
}
