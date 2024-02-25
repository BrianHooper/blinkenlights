using Blinkenlights.Dataschemas;

namespace Blinkenlights.Models.ViewModels.Stock
{
    public class StockViewModel : ModuleViewModelBase
    {
        public List<FinanceData> Data { get; set; }


		public StockViewModel(ApiStatus status) : base("Stock", status)
		{
		}

		public StockViewModel(List<FinanceData> data) : base("Stock", data.Select(h => h.Status).ToArray())
		{
			this.Data = data;
		}
	}
}
