using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class HeadlinesData : IModuleData
    {
        public string Key() => typeof(HeadlinesData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public List<HeadlinesContainer> Headlines { get; set; }

        public static HeadlinesData Clone(HeadlinesData other)
        {
            return new HeadlinesData()
            {
                TimeStamp = other?.TimeStamp,
                Headlines = other?.Headlines
            };
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

        public HeadlinesContainer() { }
    }

    public class HeadlinesCategory
    {
        public HeadlinesCategory(string title, List<HeadlinesArticle> articles)
        {
            Title = title;
            Articles = articles;
        }

        public HeadlinesCategory() { }

        public string Title { get; set; }

        public List<HeadlinesArticle> Articles { get; set; }
    }

    public class HeadlinesArticle
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public HeadlinesArticle() { }

        public HeadlinesArticle(NewYorkTimesResultsModel article)
        {
            Url = article.url;

            if (!string.IsNullOrWhiteSpace(article.@abstract))
            {
                Title = $"{article.title} - {article.@abstract}";
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