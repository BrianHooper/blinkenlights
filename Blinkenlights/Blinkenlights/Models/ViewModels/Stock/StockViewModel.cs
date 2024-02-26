using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.Stock
{
    public class StockViewModel : ModuleViewModelBase
	{
		public List<FinanceData> Data { get; set; }

		public List<CurrencyData> CurrencyData { get; set; }

		public StockViewModel(params ApiStatus[] statuses) : base("Stock", statuses)
		{
		}
	}
}
