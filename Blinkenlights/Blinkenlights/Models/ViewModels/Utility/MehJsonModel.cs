namespace Blinkenlights.Models.ViewModels.Utility
{
    public partial class MehJsonModel
    {
        public Deal Deal { get; set; }
    }

    public partial class Deal
    {
        public string Features { get; set; }
        public string Id { get; set; }
        public List<Item> Items { get; set; }
        public List<string> Photos { get; set; }
        public PurchaseQuantity PurchaseQuantity { get; set; }
        public string Title { get; set; }
        public string Specifications { get; set; }
        public Story Story { get; set; }
        public Theme Theme { get; set; }
        public string Url { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public Topic Topic { get; set; }
    }

    public partial class Item
    {
        public List<object> Attributes { get; set; }
        public string Condition { get; set; }
        public long Id { get; set; }
        public double Price { get; set; }
        public string Photo { get; set; }
    }

    public partial class PurchaseQuantity
    {
        public long MaximumLimit { get; set; }
        public long MinimumLimit { get; set; }
    }

    public partial class Story
    {
        public string Title { get; set; }
        public string Body { get; set; }
    }

    public partial class Theme
    {
        public string AccentColor { get; set; }
        public string BackgroundColor { get; set; }
        public string BackgroundImage { get; set; }
        public string Foreground { get; set; }
    }

    public partial class Topic
    {
        public long CommentCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public string Id { get; set; }
        public long ReplyCount { get; set; }
        public string Url { get; set; }
        public long VoteCount { get; set; }
    }
}
