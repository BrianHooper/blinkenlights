namespace Blinkenlights.Models.ViewModels.Utility
{
    using System;
    using System.Collections.Generic;

    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public partial class MehJsonModel
    {
        [JsonPropertyName("deal")]
        public Deal Deal { get; set; }

        [JsonPropertyName("poll")]
        public Poll Poll { get; set; }

        [JsonPropertyName("video")]
        public Poll Video { get; set; }
    }

    public partial class Deal
    {
        [JsonPropertyName("features")]
        public string Features { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("items")]
        public List<Item> Items { get; set; }

        [JsonPropertyName("photos")]
        public List<string> Photos { get; set; }

        [JsonPropertyName("purchaseQuantity")]
        public PurchaseQuantity PurchaseQuantity { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("specifications")]
        public string Specifications { get; set; }

        [JsonPropertyName("story")]
        public Story Story { get; set; }

        [JsonPropertyName("theme")]
        public Theme Theme { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("launches")]
        public List<Launch> Launches { get; set; }

        [JsonPropertyName("topic")]
        public Topic Topic { get; set; }
    }

    public partial class Item
    {
        [JsonPropertyName("attributes")]
        public List<object> Attributes { get; set; }

        [JsonPropertyName("condition")]
        public string Condition { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("photo")]
        public string Photo { get; set; }
    }

    public partial class Launch
    {
        [JsonPropertyName("soldOutAt")]
        public object SoldOutAt { get; set; }
    }

    public partial class PurchaseQuantity
    {
        [JsonPropertyName("maximumLimit")]
        public long MaximumLimit { get; set; }

        [JsonPropertyName("minimumLimit")]
        public long MinimumLimit { get; set; }
    }

    public partial class Story
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public partial class Theme
    {
        [JsonPropertyName("accentColor")]
        public string AccentColor { get; set; }

        [JsonPropertyName("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonPropertyName("backgroundImage")]
        public string BackgroundImage { get; set; }

        [JsonPropertyName("foreground")]
        public string Foreground { get; set; }
    }

    public partial class Topic
    {
        [JsonPropertyName("commentCount")]
        public long CommentCount { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("replyCount")]
        public long ReplyCount { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("voteCount")]
        public long VoteCount { get; set; }
    }

    public partial class Poll
    {
        [JsonPropertyName("answers")]
        public List<Answer> Answers { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("startDate")]
        public DateTimeOffset StartDate { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("topic")]
        public Topic Topic { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public partial class Answer
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("voteCount")]
        public long VoteCount { get; set; }
    }
}
