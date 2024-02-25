namespace Blinkenlights.Dataschemas
{
    public class SlideshowFrame
    {
        public SlideshowFrame()
        {

        }

        public SlideshowFrame(ApiStatus status)
        {
            Status = status;
        }

        public string Title { get; set; }

        public string Source { get; set; }

		public string Url { get; set; }

		public string Subtitle { get; set; }

		public string Key { get; set; }

		public ApiStatus Status { get; set; }
    }
}