using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Weather;

namespace Blinkenlights.Transformers
{
    public class WeatherTransformer : TransformerBase
    {
        IDataFetcher<WeatherData> DataFetcher;

        public WeatherTransformer(IApiHandler apiHandler, IDataFetcher<WeatherData> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.GetLocalData();
            if (response == null)
            {
                //var errorStatus = this.ApiStatusFactory.Failed(ApiType.VisualCrossingWeather, "Api response is null");
                return new WeatherViewModel();
            }

            var viewModel = new WeatherViewModel(response.Description, response.DayModels, response.GraphModel, response.CurrentConditions, response.Status);
            return viewModel;
        }
    }
}
