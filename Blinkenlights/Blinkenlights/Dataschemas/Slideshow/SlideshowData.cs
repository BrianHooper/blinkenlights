using System.Text.Json;

namespace Blinkenlights.Dataschemas
{
    public class SlideshowData : IDatabaseData
    {
        public string Key() => typeof(SlideshowData).Name;

        public string Value() => JsonSerializer.Serialize(this);

        public DateTime? TimeStamp { get; init; }

        public List<SlideshowFrame> Frames { get; init; }

        public static SlideshowData Clone(SlideshowData other)
        {
            return new SlideshowData()
            {
                TimeStamp = other?.TimeStamp,
                Frames = other?.Frames
            };
        }
    }
}