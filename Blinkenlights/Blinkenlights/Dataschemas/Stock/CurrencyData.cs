namespace Blinkenlights.Dataschemas
{
	public class CurrencyData
	{
		public string Symbol { get; set; }

		public string Price { get; set; }

		public ApiStatus Status { get; set; }

		public static CurrencyData Clone(CurrencyData other, string symbol, ApiStatus status)
		{
			return new CurrencyData()
			{
				Symbol = symbol,
				Price = other?.Price,
				Status = status
			};
		}
	}
}
