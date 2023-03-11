namespace Blinkenlights.Models.Api.ApiHandler
{
    public class InstanceSecret
    {
        public string Secret { get; set; }

        public DateTime DateTimeCreated { get; set; }

        public long TimeToLiveSeconds { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Secret) && DateTimeCreated.AddSeconds(TimeToLiveSeconds) > DateTime.Now;
        }
    }
}
