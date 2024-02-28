using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.FlightStatus;
using Blinkenlights.Models.ViewModels.OuterSpace;

namespace Blinkenlights.Transformers
{
	public class FlightStatusTransformer : TransformerBase
	{
		private IDataFetcher<FlightStatusData> dataFetcher { get; init; }

		public FlightStatusTransformer(IApiHandler apiHandler, IDataFetcher<FlightStatusData> dataFetcher) : base(apiHandler)
		{
			this.dataFetcher = dataFetcher;
		}

		public override IModuleViewModel Transform()
		{
			this.dataFetcher.FetchRemoteData(true);
			var data = this.dataFetcher.GetLocalData();
			if (data == null)
			{
				return new FlightStatusViewModel();
			}

			return new FlightStatusViewModel()
			{
			};
		}
	}
}
