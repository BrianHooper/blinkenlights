namespace Blinkenlights.Models.ViewModels.Astronomy
{
	using Newtonsoft.Json;

	public class AstronomyJsonModel
	{
		[JsonProperty("Title", Required = Required.Always)]
		public string Title { get; set; }

		[JsonProperty("Source", Required = Required.Always)]
		public string Source { get; set; }

		[JsonProperty("Url", Required = Required.Always)]
		public string Url { get; set; }
	}
}