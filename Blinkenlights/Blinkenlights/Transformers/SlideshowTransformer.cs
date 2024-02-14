using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Slideshow;

namespace Blinkenlights.Transformers
{
    public class SlideshowTransformer : TransformerBase
    {
        private IDataFetcher<SlideshowData> DataFetcher { get; init; }

        public SlideshowTransformer(IApiHandler apiHandler, IDataFetcher<SlideshowData> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            var response = this.DataFetcher.GetLocalData();
            if (response == null)
            {
                var errorStatus = ApiStatus.Failed("Slideshow data", "Database lookup failed");
                return new SlideshowViewModel(errorStatus);
            }

            if (response.Frames?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed("Slideshow data", "No frames in local data");
                return new SlideshowViewModel(errorStatus);
            }

            return new SlideshowViewModel(response.Frames);
        }
    }
}
