namespace Blinkenlights.Models.ViewModels.Utility
{
    public class MehViewModel
    {
        public string Title { get; set; }

        public string Url { get; set; }

        public string ImageUrl { get; set; }

        public MehViewModel(string title = null, string url = null, string imageUrl = null)
        {
            Title = title;
            Url = url;
            ImageUrl = imageUrl;
        }
    }
}
