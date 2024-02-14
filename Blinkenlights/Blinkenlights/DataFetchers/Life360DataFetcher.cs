using Blinkenlights.DatabaseHandler;
using Blinkenlights.Dataschemas;
using Blinkenlights.Models.Api.ApiHandler;
using Blinkenlights.Models.Api.ApiInfoTypes;
using Blinkenlights.Models.ViewModels.Life360;
using Newtonsoft.Json;

namespace Blinkenlights.DataFetchers
{
    public class Life360DataFetcher : DataFetcherBase<Life360Data>
    {
        public Life360DataFetcher(IDatabaseHandler databaseHandler, IApiHandler apiHandler) : base(TimeSpan.FromMinutes(5), databaseHandler, apiHandler)
        {
            Start();
        }

        protected override Life360Data GetRemoteData(Life360Data existingData = null)
        {
            var response = this.ApiHandler.Fetch(ApiType.Life360).Result;
            if (response == null)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), "Api response is null");
                return Life360Data.Clone(existingData, errorStatus);
            }
            else if (response.ResultStatus != ApiResultStatus.Success)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), response.StatusMessage, response.LastUpdateTime);
                return Life360Data.Clone(existingData, errorStatus);
            }

            Life360JsonModel serverModel;
            try
            {
                serverModel = JsonConvert.DeserializeObject<Life360JsonModel>(response.Data);
            }
            catch (JsonException)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), "Exception while deserializing API response");
                return Life360Data.Clone(existingData, errorStatus);
            }

            var models = serverModel?.Members?.Select(m => Parse(m))?.Where(m => m != null)?.ToList();
            if (models?.Any() != true)
            {
                var errorStatus = ApiStatus.Failed(ApiType.Life360.ToString(), "Models list was empty", response.LastUpdateTime);
                return Life360Data.Clone(existingData, errorStatus);
            }

            var status = ApiStatus.Success(ApiType.Life360.ToString(), response.LastUpdateTime, response.ApiSource);
            return new Life360Data()
            {
                Status = status,
                TimeStamp = DateTime.Now,
                Locations = models,
            };
        }

        private static Life360LocationData Parse(Member member)
        {
            return Life360LocationData.Parse(member?.FirstName, member?.Location?.Timestamp?.ToString(), member?.Location?.Latitude?.ToString(), member?.Location?.Longitude?.ToString());
        }
    }
}
