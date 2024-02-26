using System.Text.Json.Serialization;

namespace Blinkenlights.Models.ViewModels.Stock
{
	public partial class CurrencyJsonModel
	{
		[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		[JsonPropertyName("Realtime Currency Exchange Rate")]
		public RealtimeCurrencyExchangeRate RealtimeCurrencyExchangeRate { get; set; }
	}

	public partial class RealtimeCurrencyExchangeRate
	{
		[JsonPropertyName("1. From_Currency Code")]
		public string The1FromCurrencyCode { get; set; }

		[JsonPropertyName("2. From_Currency Name")]
		public string The2FromCurrencyName { get; set; }

		[JsonPropertyName("3. To_Currency Code")]
		public string The3ToCurrencyCode { get; set; }

		[JsonPropertyName("4. To_Currency Name")]
		public string The4ToCurrencyName { get; set; }

		[JsonPropertyName("5. Exchange Rate")]
		public string The5ExchangeRate { get; set; }

		[JsonPropertyName("6. Last Refreshed")]
		public string The6LastRefreshed { get; set; }

		[JsonPropertyName("7. Time Zone")]
		public string The7TimeZone { get; set; }

		[JsonPropertyName("8. Bid Price")]
		public string The8BidPrice { get; set; }

		[JsonPropertyName("9. Ask Price")]
		public string The9AskPrice { get; set; }
	}

}