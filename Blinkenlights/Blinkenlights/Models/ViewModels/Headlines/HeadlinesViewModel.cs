using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Headlines
{
    public class HeadlinesViewModel : ApiResultBase
	{
        public List<HeadlinesContainer> Headlines { get; set; }

        public HeadlinesViewModel(params HeadlinesContainer[] headlines) : base("Headlines", headlines.Select(h => h.Status).ToArray())
        {
            Headlines = headlines.ToList();
        }


    }

    public class HeadlinesContainer
    {
        public List<HeadlinesCategory> Categories { get; set; }

        public ApiStatus Status { get; set; }

        public HeadlinesContainer(List<HeadlinesCategory> categories, ApiStatus status)
        {
            Categories = categories;
            Status = status;
        }
    }

    public class HeadlinesCategory
    {
        public HeadlinesCategory(string title, List<HeadlinesArticle> articles)
        {
            Title = title;
            Articles = articles;
        }

        public string Title { get; set; }

        public List<HeadlinesArticle> Articles { get; set; }
    }

    public class HeadlinesArticle
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public HeadlinesArticle(NewYorkTimesResultsModel article)
        {
            Url = article.url;

            if (!string.IsNullOrWhiteSpace(article.article_abstract))
            {
                Title = $"{article.title} - {article.article_abstract}";
            }
            else
            {
                Title = article.title;
            }
        }

        public HeadlinesArticle(Article article)
        {
            Title = article?.title;
            Url = article?.url;
        }

		public HeadlinesArticle(string title, string url)
		{
			Title = title;
			Url = url;
		}
	}
}
