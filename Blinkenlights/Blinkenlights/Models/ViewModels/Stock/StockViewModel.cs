using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiResult;

namespace Blinkenlights.Models.ViewModels.Stock
{
    public class StockViewModel : ApiResultBase
    {
        public List<FinanceData> Data { get; set; }

        public StockViewModel(ApiStatus status, List<FinanceData> data = null) : base("Stock", status)
        {
            this.Data = data;
        }
    }
}
