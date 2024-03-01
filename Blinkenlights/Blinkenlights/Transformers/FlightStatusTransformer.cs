using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.FlightStatus;

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
			var data = this.dataFetcher.FetchRemoteData();
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
