namespace Blinkenlights.Dataschemas
{
    public class MehData
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public ApiStatus Status { get; set; }

        public MehData() { }

        public MehData(string title, string url, string imageUrl, ApiStatus status)
        {
            Title = title;
            Url = url;
            ImageUrl = imageUrl;
            Status = status;
        }

        public static MehData Clone(MehData other, ApiStatus status)
        {
            return new MehData()
            {
                Title = other?.Title,
                Url = other?.Url,
                ImageUrl = other?.ImageUrl,
                Status = status
            };
        }
    }
}
