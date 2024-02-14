using Blinkenlights.DataFetchers;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels;
using Blinkenlights.Models.ViewModels.Life360;

namespace Blinkenlights.Transformers
{
    public class Life360Transformer : TransformerBase
    {
        private IDataFetcher<Life360Data> DataFetcher { get; init; }

        public Life360Transformer(IApiHandler apiHandler, IDataFetcher<Life360Data> dataFetcher) : base(apiHandler)
        {
            this.DataFetcher = dataFetcher;
        }

        public override IModuleViewModel Transform()
        {
            this.DataFetcher.FetchRemoteData(true);
            var response = this.DataFetcher.GetLocalData();
            if (response == null)
            {
                var status = ApiStatus.Failed(ApiType.Life360.ToString(), "Api response is null");
                return new Life360ViewModel(null, status);
            }

            if (response.Locations?.Any() != true)
            {
                var status = ApiStatus.Failed(ApiType.Life360.ToString(), "Api response is empty");
                return new Life360ViewModel(null, status);
            }

            return new Life360ViewModel(response.Locations, response.Status);
            //var response = this.ApiHandler.Fetch(ApiType.Life360).Result;
            //if (response == null)
            //         {
            //             var status = ApiStatus.Failed(ApiType.Life360.ToString(), "Api response is null");
            //             return new Life360ViewModel(null, status);
            //}
            //         else if (response.ResultStatus != ApiResultStatus.Success)
            //         {
            //	var status = ApiStatus.Failed(ApiType.Life360.ToString(), response.StatusMessage, response.LastUpdateTime);
            //	return new Life360ViewModel(null, status);
            //}

            //         Life360JsonModel serverModel;
            //         try
            //         {
            //             serverModel = JsonConvert.DeserializeObject<Life360JsonModel>(response.Data);
            //         }
            //         catch (JsonException)
            //         {
            //             var status = ApiStatus.Failed(ApiType.Life360.ToString(), "Exception while deserializing API response");
            //	return new Life360ViewModel(null, status);
            //         }

            //         var models = serverModel?.Members?.Select(m => Life360Model.Parse(m))?.Where(m => m != null)?.ToList();
            //         if (models?.Any() == true)
            //         {
            //             var status = ApiStatus.Success(ApiType.Life360.ToString(), response.LastUpdateTime, response.ApiSource);
            //             this.ApiHandler.TryUpdateCache(response);
            //	return new Life360ViewModel(models, status);
            //         }
            //         else
            //         {
            //             var status = ApiStatus.Failed(ApiType.Life360.ToString(), "Models list was empty", response.LastUpdateTime);
            //	return new Life360ViewModel(null, status);
            //         }
        }
    }
}
